using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Interop;
using Saxxon.Foundations.Sdl3.Models;
using SDL;
using static SDL.SDL3;

namespace Saxxon.Foundations.Sdl3.Game;


/// <summary>
/// Base class for game, window and render operations.
/// </summary>
[PublicAPI]
public abstract class Game : IDisposable
{
    /// <summary>
    /// Queue used for pending game states.
    /// </summary>
    private static readonly ConcurrentQueue<Game> PendingGames = [];

    /// <summary>
    /// True only when the unhandled exception handler has been installed.
    /// </summary>
    private static bool _crashHandlerInstalled;

    /// <summary>
    /// SDL renderer for the game window.
    /// </summary>
    public IntPtr<SDL_Renderer> Renderer { get; private set; }

    /// <summary>
    /// SDL window for the game.
    /// </summary>
    public IntPtr<SDL_Window> Window { get; private set; }

    /// <summary>
    /// Audio mixer.
    /// </summary>
    public IntPtr<MIX_Mixer> Mixer { get; private set; }

    /// <summary>
    /// Command line arguments that were passed to the game.
    /// </summary>
    protected IReadOnlyList<string> Args { get; private set; } = [];

    /// <summary>
    /// Color that will be used to clear the backbuffer each frame.
    /// </summary>
    protected SDL_FColor ClearColor { get; set; } = new() { a = 1 };

    /// <summary>
    /// Backbuffer that game objects will be rendered to.
    /// </summary>
    protected IntPtr<SDL_Texture> Backbuffer { get; private set; }

    /// <summary>
    /// Gets the IDs of all currently connected keyboards.
    /// </summary>
    protected IEnumerable<SDL_KeyboardID> Keyboards => _keyboards;

    /// <summary>
    /// Gets the IDs of all currently connected mice.
    /// </summary>
    protected IEnumerable<SDL_MouseID> Mice => _mice;

    /// <summary>
    /// Gets the IDs of all currently connected gamepads.
    /// </summary>
    protected IEnumerable<IntPtr<SDL_Gamepad>> Gamepads => _gamepads.Values;

    /// <summary>
    /// Returns whether the game window is currently focused.
    /// </summary>
    public bool IsFocused { get; private set; }

    private CancellationTokenSource _closeToken = new();
    private ulong _lastUpdate;
    private ulong _lastDraw;
    private ulong _lastPresent;
    private Dictionary<SDL_JoystickID, IntPtr<SDL_Gamepad>> _gamepads = [];
    private HashSet<SDL_KeyboardID> _keyboards = [];
    private HashSet<SDL_MouseID> _mice = [];
    private IntPtr<SDL_Texture> _scaleTexture;
    private LogOutputFunction? _logOutputFunction;
    private string _title = "";

    /// <summary>
    /// Gets or sets the title of the game window.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            if (Window != IntPtr.Zero)
                Window.SetTitle(_title);
        }
    }

    /// <summary>
    /// Runs the game.
    /// </summary>
    /// <param name="args">
    /// Command line arguments.
    /// </param>
    /// <returns>
    /// The exit code of the game. This will be 0 if the game exited normally,
    /// or some other value that represents the error that occurred.
    /// </returns>
    public int Run(params string[] args)
    {
        _closeToken = new CancellationTokenSource();

        Args = args;
        PendingGames.Enqueue(this);

        var main = new AppMain<Game>
        {
            CancellationToken = _closeToken.Token,
            OnEvent = MainEvent,
            OnInit = MainInit,
            OnIterate = MainIter,
            OnQuit = MainQuit
        };

        if (!_crashHandlerInstalled)
        {
            AppDomain.CurrentDomain.UnhandledException += HandleCrash;
            _crashHandlerInstalled = true;
        }

        try
        {
            return main.Enter(args);
        }
        catch
        {
            if (Debugger.IsAttached)
                throw;
            return -1;
        }
        finally
        {
            SdlLib.Quit();
        }

        //
        // The quit function is called when the game is about to exit.
        //
    }

    /// <summary>
    /// Handles setting up the game before the main loop.
    /// </summary>
    private static (SDL_AppResult, Game?) MainInit(string[] args)
    {
        if (!PendingGames.TryDequeue(out var game))
            return (SDL_AppResult.SDL_APP_FAILURE, null!);

        game._logOutputFunction = new LogOutputFunction(game.OnLog);

        //
        // Configure SDL to use .NET memory management. This allows SDL to
        // take advantage of the .NET garbage collector.
        //

        Mem.SetDotNetFunctions();

        //
        // Initialize required subsystems.
        //

        SdlLib.Init(SDL_InitFlags.SDL_INIT_VIDEO |
                    SDL_InitFlags.SDL_INIT_EVENTS);

        //
        // Initialize optional subsystems.
        //

        try
        {
            if (game.ShouldInitGamepads)
                SdlLib.Init(SDL_InitFlags.SDL_INIT_GAMEPAD);
        }
        catch
        {
            Log.Error("");
        }

        TtfLib.Init();
        MixerLib.Init();

        //
        // Create the game window and renderer.
        //

        (game.Window, game.Renderer) = Saxxon.Foundations.Sdl3.Models.Window
            .CreateWithRenderer(game.Title, 1280, 720, SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

        Hint.Set(SDL_HINT_RENDER_VSYNC, "1");
        Hint.Set(SDL_HINT_EVENT_LOGGING, "1");
        Hint.Set(SDL_HINT_QUIT_ON_LAST_WINDOW_CLOSE, "1");

        //
        // Create the backbuffer texture.
        //

        var backBufferFormat = game.Window.GetPixelFormat();
        game.Backbuffer = Texture.Create(
            game.Renderer, backBufferFormat, SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 640, 360
        );
        game.Backbuffer.SetScaleMode(SDL_ScaleMode.SDL_SCALEMODE_PIXELART);

        //
        // Create the scale texture.
        //

        game._scaleTexture = Texture.Create(
            game.Renderer, backBufferFormat, SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 1920, 1080
        );

        //
        // Initialize the audio mixer.
        //

        game.Mixer = Saxxon.Foundations.Sdl3.Models.Mixer
            .CreateDevice(SDL_AUDIO_DEVICE_DEFAULT_PLAYBACK, new SDL_AudioSpec
            {
                format = SDL_AUDIO_S16,
                channels = 2,
                freq = 48000
            });

        //
        // Initialize game state.
        //

        game._lastDraw = game._lastUpdate = game._lastPresent = Time.GetNowNanoseconds();
        game.OnInit();

        return (SDL_AppResult.SDL_APP_CONTINUE, game);
    }

    /// <summary>
    /// Handles when the game is shutting down.
    /// </summary>
    private static void MainQuit(Game game, SDL_AppResult result)
    {
        // Shut down subsystems.

        TtfLib.Quit();
        MixerLib.Quit();
        SdlLib.Quit();
    }

    /// <summary>
    /// Processes an event from SDL.
    /// </summary>
    private static SDL_AppResult MainEvent(Game game, ref SDL_Event e)
    {
        //
        // Handle the raw event.
        //

        game.SetLetterboxOff();
        game.OnPreEvent(ref e);
        game.SetLetterboxOn();

        //
        // Convert coordinates to the renderer's coordinate system.
        //

        game.Renderer.ConvertEventCoordinates(ref e);

        //
        // Handle the event.
        //

        switch (e.Type)
        {
            //
            // Keyboard events
            //

            case SDL_EventType.SDL_EVENT_KEYBOARD_ADDED:
            {
                game._keyboards.Add(e.kdevice.which);
                game.OnKeyboardConnected(e.kdevice.which);
                break;
            }
            case SDL_EventType.SDL_EVENT_KEYBOARD_REMOVED:
            {
                game._keyboards.Remove(e.kdevice.which);
                game.OnKeyboardDisconnected(e.kdevice.which);
                break;
            }
            case SDL_EventType.SDL_EVENT_KEY_DOWN:
            {
                game.OnKeyDown(e.key.which, e.key.key, e.key.scancode, e.key.mod);
                break;
            }
            case SDL_EventType.SDL_EVENT_KEY_UP:
            {
                game.OnKeyUp(e.key.which, e.key.key, e.key.scancode, e.key.mod);
                break;
            }

            //
            // Mouse events
            //

            case SDL_EventType.SDL_EVENT_MOUSE_ADDED:
            {
                game._mice.Add(e.mdevice.which);
                game.OnMouseConnected(e.mdevice.which);
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_REMOVED:
            {
                game._mice.Remove(e.mdevice.which);
                game.OnMouseDisconnected(e.mdevice.which);
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
            {
                game.OnMouseDown(e.button.which, (SDLButton)e.button.button);
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
            {
                game.OnMouseUp(e.button.which, (SDLButton)e.button.button);
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
            {
                game.OnMouseMove(e.motion.which, (int)Math.Truncate(e.motion.x), (int)Math.Truncate(e.motion.y));
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_WHEEL:
            {
                game.OnMouseWheel(e.wheel.which, e.wheel.x, e.wheel.y, e.wheel.direction);
                break;
            }

            //
            // Gamepad events
            //

            case SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
            {
                var gamepad = e.gdevice.which.OpenGamepad();
                game._gamepads.Add(e.gdevice.which, gamepad);
                game.OnGamepadConnected(gamepad);
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
            {
                game._gamepads.Remove(e.gdevice.which, out var gamepad);
                gamepad.Close();
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
            {
                game.OnGamepadAxisChanged(e.gaxis.which.GetGamepad(), e.gaxis.Axis, e.gaxis.value / 32767f);
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN:
            {
                game.OnGamepadButtonDown(e.gaxis.which.GetGamepad(), e.gbutton.Button);
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP:
            {
                game.OnGamepadButtonUp(e.gaxis.which.GetGamepad(), e.gbutton.Button);
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_MOTION:
            {
                game.OnGamepadTouchMove(
                    e.gtouchpad.which.GetGamepad(),
                    e.gtouchpad.touchpad,
                    e.gtouchpad.finger,
                    e.gtouchpad.pressure
                );
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_DOWN:
            {
                game.OnGamepadTouchDown(
                    e.gtouchpad.which.GetGamepad(),
                    e.gtouchpad.touchpad,
                    e.gtouchpad.finger,
                    e.gtouchpad.pressure
                );
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_UP:
            {
                game.OnGamepadTouchUp(
                    e.gtouchpad.which.GetGamepad(),
                    e.gtouchpad.touchpad,
                    e.gtouchpad.finger,
                    e.gtouchpad.pressure
                );
                break;
            }

            //
            // Window events
            //

            case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_GAINED:
            {
                game.IsFocused = true;
                game.OnFocusGained();
                break;
            }
            case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_LOST:
            {
                game.IsFocused = false;
                game.OnFocusLost();
                break;
            }
            case SDL_EventType.SDL_EVENT_QUIT:
            {
                game.OnQuit();
                return game._closeToken.IsCancellationRequested
                    ? SDL_AppResult.SDL_APP_SUCCESS
                    : SDL_AppResult.SDL_APP_CONTINUE;
            }
        }

        game.OnEvent(e);
        return SDL_AppResult.SDL_APP_CONTINUE;
    }

    /// <summary>
    /// Processes one iteration of the main loop.
    /// </summary>
    private static SDL_AppResult MainIter(Game game)
    {
        if (game._closeToken.IsCancellationRequested)
            return SDL_AppResult.SDL_APP_SUCCESS;

        var renderer = game.Renderer;

        //
        // Perform game logic updates.
        //

        game.SetLetterboxOff();
        var thisUpdate = Time.GetNowNanoseconds();
        game.OnUpdate(Time.GetFromNanoseconds(thisUpdate - game._lastUpdate));
        game._lastUpdate = thisUpdate;

        //
        // Render game objects.
        //

        game.SetLetterboxOff();
        renderer.SetTarget(game.Backbuffer);

        var clearColor = game.ClearColor;
        renderer.SetColorFloat(clearColor.r, clearColor.g, clearColor.b, clearColor.a);
        renderer.Clear();

        var thisDraw = Time.GetNowNanoseconds();
        game.OnDraw(Time.GetFromNanoseconds(thisDraw - game._lastDraw));
        game._lastDraw = thisDraw;

        //
        // Prescale the backbuffer. This makes pixel art crisp.
        //

        game.SetLetterboxOff();
        renderer.SetTarget(game._scaleTexture);
        renderer.Texture(game.Backbuffer, null, null);

        //
        // Clear the scene.
        //

        renderer.SetTarget(null);
        renderer.SetColorFloat(0, 0, 0, 1);
        renderer.Clear();

        //
        // Blit the scene. This actually puts the rendered game scene
        // onto the window.
        //

        game.SetLetterboxOn();
        renderer.Texture(game.Backbuffer, null, null);

        //
        // Present the scene to the window.
        //

        game.SetLetterboxOff();
        var thisPresent = Time.GetNowNanoseconds();
        game.OnPresent(Time.GetFromNanoseconds(thisPresent - game._lastPresent));
        game._lastPresent = thisPresent;
        game.SetLetterboxOn();

        //
        // This function blocks until the frame is ready to be shown in
        // accordance with Vsync.
        //

        renderer.Present();
        return SDL_AppResult.SDL_APP_CONTINUE;
    }

    /// <summary>
    /// When an unhandled exception bubbles out to the app domain, the app
    /// is crashing in a way that is unrecoverable. This function is called when
    /// this happens, to make sure that SDL is shut down properly.
    /// </summary>
    private void HandleCrash(object sender, UnhandledExceptionEventArgs e)
    {
        Console.WriteLine(e.ExceptionObject);
        SDL_Quit();
    }

    /// <summary>
    /// Enables the letterboxed coordinate system.
    /// </summary>
    private void SetLetterboxOn() =>
        Renderer.SetLogicalPresentation(
            640, 360, SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_LETTERBOX
        );

    /// <summary>
    /// Disables the letterboxed coordinate system.
    /// </summary>
    private void SetLetterboxOff() =>
        Renderer.SetLogicalPresentation(
            0, 0, SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_DISABLED
        );

    /// <summary>
    /// Gets whether the SDL gamepad subsystem should be enabled.
    /// This is true by default, but if your game does not use gamepads,
    /// this can be disabled.
    /// </summary>
    protected virtual bool ShouldInitGamepads => true;

    /// <summary>
    /// Gets whether the SDL haptics subsystem should be enabled.
    /// This is true by default, but if your game does not use haptics,
    /// this can be disabled.
    /// </summary>
    protected virtual bool ShouldInitHaptics => true;

    /// <summary>
    /// Gets whether the SDL audio subsystem should be enabled.
    /// This is true by default, but if your game does not use audio,
    /// this can be disabled.
    /// </summary>
    protected virtual bool ShouldInitMixer => true;
    
    /// <summary>
    /// Handles when SDL is logging a message. By default, messages are logged
    /// to the console.
    /// </summary>
    protected virtual void OnLog(int category, SDL_LogPriority priority, ReadOnlySpan<char> message)
    {
        Console.Write("[{0}] ", priority);
        Console.WriteLine(message);
    }

    /// <summary>
    /// Handles when the game has first started.
    /// </summary>
    protected virtual void OnInit()
    {
    }

    /// <summary>
    /// Handles updating game state.
    /// </summary>
    /// <param name="delta">
    /// Amount of time that has passed since the last time game state was
    /// updated.
    /// </param>
    protected virtual void OnUpdate(TimeSpan delta)
    {
    }

    /// <summary>
    /// Handles drawing game objects.
    /// </summary>
    /// <param name="delta">
    /// Amount of time that has passed since the last time game objects were
    /// rendered.
    /// </param>
    protected virtual void OnDraw(TimeSpan delta)
    {
    }

    /// <summary>
    /// Handles raw events from SDL.
    /// </summary>
    /// <param name="e">
    /// Event data. This can be modified before it is replicated to other
    /// event handlers.
    /// </param>
    protected virtual void OnPreEvent(ref SDL_Event e)
    {
    }

    /// <summary>
    /// Handles incoming events from SDL.
    /// </summary>
    /// <param name="e">
    /// Event data.
    /// </param>
    /// <remarks>
    /// For event types that contain coordinate data, the coordinates will
    /// have been converted to the renderer's coordinate system prior to the
    /// call.
    /// </remarks>
    protected virtual void OnEvent(SDL_Event e)
    {
    }

    /// <summary>
    /// Handles when a request to quit the game is made.
    /// </summary>
    protected virtual void OnQuit()
    {
        _closeToken.Cancel();
    }

    /// <summary>
    /// Handles when a gamepad has been connected.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that was connected.
    /// </param>
    protected virtual void OnGamepadConnected(IntPtr<SDL_Gamepad> gamepad)
    {
    }

    /// <summary>
    /// Handles when a gamepad has been disconnected.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that was disconnected.
    /// </param>
    protected virtual void OnGamepadDisconnected(IntPtr<SDL_Gamepad> gamepad)
    {
    }

    /// <summary>
    /// Handles when a button on a gamepad has been pressed.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the button was pressed on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was pressed.
    /// </param>
    protected virtual void OnGamepadButtonDown(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadButton button)
    {
    }

    /// <summary>
    /// Handles when a button on a gamepad has been released.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the button was released on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was released.
    /// </param>
    protected virtual void OnGamepadButtonUp(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadButton button)
    {
    }

    /// <summary>
    /// Handles when a gamepad axis has changed.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the axis was changed on.
    /// </param>
    /// <param name="axis">
    /// ID of the axis that was changed.
    /// </param>
    /// <param name="value">
    /// The new value of the axis.
    /// </param>
    protected virtual void OnGamepadAxisChanged(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadAxis axis, float value)
    {
    }

    /// <summary>
    /// Handles when a gamepad touchpad has been pressed.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the touchpad was pressed on.
    /// </param>
    /// <param name="padIndex">
    /// Touchpad that was pressed.
    /// </param>
    /// <param name="fingerIndex">
    /// Finger that was pressed.
    /// </param>
    /// <param name="pressure">
    /// Pressure of the touch.
    /// </param>
    protected virtual void OnGamepadTouchDown(IntPtr<SDL_Gamepad> gamepad, int padIndex, int fingerIndex,
        float pressure)
    {
    }

    /// <summary>
    /// Handles when a gamepad touchpad has been moved.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the touchpad was moved on.
    /// </param>
    /// <param name="padIndex">
    /// Touchpad that was moved.
    /// </param>
    /// <param name="fingerIndex">
    /// Finger that was moved.
    /// </param>
    /// <param name="pressure">
    /// Pressure of the touch.
    /// </param>
    protected virtual void OnGamepadTouchMove(IntPtr<SDL_Gamepad> gamepad, int padIndex, int fingerIndex,
        float pressure)
    {
    }

    /// <summary>
    /// Handles when a gamepad touchpad has been released.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the touchpad was released on.
    /// </param>
    /// <param name="padIndex">
    /// Touchpad that was released.
    /// </param>
    /// <param name="fingerIndex">
    /// Finger that was released.
    /// </param>
    /// <param name="pressure">
    /// Pressure of the touch.
    /// </param>
    protected virtual void OnGamepadTouchUp(IntPtr<SDL_Gamepad> gamepad, int padIndex, int fingerIndex, float pressure)
    {
    }

    /// <summary>
    /// Handles when a mouse has been connected.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that was connected.
    /// </param>
    protected virtual void OnMouseConnected(SDL_MouseID mouse)
    {
    }

    /// <summary>
    /// Handles when a mouse has been disconnected.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that was disconnected.
    /// </param>
    protected virtual void OnMouseDisconnected(SDL_MouseID mouse)
    {
    }

    /// <summary>
    /// Handles when a mouse button has been pressed.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that the button was pressed on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was pressed.
    /// </param>
    protected virtual void OnMouseDown(SDL_MouseID mouse, SDLButton button)
    {
    }

    /// <summary>
    /// Handles when a mouse button has been released.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that the button was released on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was released.
    /// </param>
    protected virtual void OnMouseUp(SDL_MouseID mouse, SDLButton button)
    {
    }

    /// <summary>
    /// Handles when a mouse has moved.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that moved.
    /// </param>
    /// <param name="x">
    /// New x-coordinate of the mouse.
    /// </param>
    /// <param name="y">
    /// New y-coordinate of the mouse.
    /// </param>
    protected virtual void OnMouseMove(SDL_MouseID mouse, int x, int y)
    {
    }

    /// <summary>
    /// Handles when a mouse wheel has moved.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that the wheel was moved on.
    /// </param>
    /// <param name="x">
    /// Horizontal scroll amount.
    /// </param>
    /// <param name="y">
    /// Vertical scroll amount.
    /// </param>
    /// <param name="direction">
    /// Direction of the scroll.
    /// </param>
    protected virtual void OnMouseWheel(SDL_MouseID mouse, float x, float y, SDL_MouseWheelDirection direction)
    {
    }

    /// <summary>
    /// Handles when a key has been pressed.
    /// </summary>
    /// <param name="keyboard">
    /// ID of the keyboard that the key was pressed on.
    /// </param>
    /// <param name="code">
    /// Key code for the key that was pressed.
    /// </param>
    /// <param name="scan">
    /// Scan code for the key that was pressed.
    /// </param>
    /// <param name="mod">
    /// Modifier keys.
    /// </param>
    protected virtual void OnKeyDown(SDL_KeyboardID keyboard, SDL_Keycode code, SDL_Scancode scan, SDL_Keymod mod)
    {
    }

    /// <summary>
    /// Handles when a key has been released.
    /// </summary>
    /// <param name="keyboard">
    /// ID of the keyboard that the key was released on.
    /// </param>
    /// <param name="code">
    /// Key code for the key that was released.
    /// </param>
    /// <param name="scan">
    /// Scan code for the key that was released.
    /// </param>
    /// <param name="mod">
    /// Modifier keys.
    /// </param>
    protected virtual void OnKeyUp(SDL_KeyboardID keyboard, SDL_Keycode code, SDL_Scancode scan, SDL_Keymod mod)
    {
    }

    /// <summary>
    /// Handles when a keyboard has been connected.
    /// </summary>
    /// <param name="keyboard">
    /// ID of the keyboard that was connected.
    /// </param>
    protected virtual void OnKeyboardConnected(SDL_KeyboardID keyboard)
    {
    }

    /// <summary>
    /// Handles when a keyboard has been disconnected.
    /// </summary>
    /// <param name="keyboard">
    /// ID of the keyboard that was disconnected.
    /// </param>
    protected virtual void OnKeyboardDisconnected(SDL_KeyboardID keyboard)
    {
    }

    /// <summary>
    /// Handles when the game window gains focus.
    /// </summary>
    protected virtual void OnFocusGained()
    {
    }

    /// <summary>
    /// Handles when the game window loses focus.
    /// </summary>
    protected virtual void OnFocusLost()
    {
    }

    /// <summary>
    /// Handles when <see cref="Dispose"/> is called.
    /// </summary>
    protected virtual void OnDispose()
    {
    }

    /// <summary>
    /// Handles when the game scene is about to be presented to the window.
    /// </summary>
    protected virtual void OnPresent(TimeSpan delta)
    {
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _closeToken.Dispose();
        _logOutputFunction?.Dispose();
        OnDispose();
    }
}