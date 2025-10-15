using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Interop;
using Saxxon.Foundations.Sdl3.Models;
using SDL;
using static SDL.SDL3;
using Mutex = System.Threading.Mutex;

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

    /// <summary>
    /// Number of times per second that game state updates should be performed.
    /// </summary>
    protected virtual double UpdatesPerSecond
    {
        get => TimeSpan.FromSeconds(1) / _updateInterval;
        set => _updateInterval = TimeSpan.FromSeconds(1) / value;
    }

    private CancellationTokenSource _closeToken = new();
    private ulong _lastDraw;
    private ulong _lastPresent;
    private Dictionary<SDL_JoystickID, IntPtr<SDL_Gamepad>> _gamepads = [];
    private HashSet<SDL_KeyboardID> _keyboards = [];
    private HashSet<SDL_MouseID> _mice = [];
    private IntPtr<SDL_Texture> _scaleTexture;
    private LogOutputFunction? _logOutputFunction;
    private string _title = "";
    private bool _quitting;
    private bool _crashing;
    private SdlTimer.TimerCallback? _updateCallback;
    private Mutex _updateMutex = new();
    private TimeSpan _updateInterval = TimeSpan.FromSeconds(1) / 200;

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
            _crashing = true;
            if (Debugger.IsAttached)
                throw;
            return -1;
        }
        finally
        {
            SdlLib.Quit();
        }
    }

    /// <summary>
    /// Handles setting up the game before the main loop.
    /// </summary>
    private static (SDL_AppResult, Game?) MainInit(string[] args)
    {
        if (!PendingGames.TryDequeue(out var game))
            return (SDL_AppResult.SDL_APP_FAILURE, null!);

        game._logOutputFunction = new LogOutputFunction(game.OnLogMessage);

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

        game.SetLetterboxOn();

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

        game._lastDraw = game._lastPresent = Time.GetNowNanoseconds();
        game.OnInit();

        //
        // Initialize update timer.
        //

        game._updateInterval = TimeSpan.FromSeconds(1) / game.UpdatesPerSecond;
        game._updateCallback = (_, elapsed) =>
        {
            if (game._closeToken.IsCancellationRequested)
                return TimeSpan.Zero;

            // While this callback may run on a different thread, it can be
            // assumed that the letterbox mode is on when entering the mutex.

            game._updateMutex.WaitOne();
            game.OnUpdate(elapsed);
            game._updateMutex.ReleaseMutex();

            return game._updateInterval;
        };

        SdlTimer.Create(game._updateCallback, game._updateInterval);

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
        game._updateMutex.WaitOne();

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

        var result = SDL_AppResult.SDL_APP_CONTINUE;

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
                game.OnMouseButtonDown(e.button.which, (SDLButton)e.button.button);
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
            {
                game.OnMouseButtonUp(e.button.which, (SDLButton)e.button.button);
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
            {
                game.OnMouseMove(
                    e.motion.which,
                    (int)Math.Truncate(e.motion.x),
                    (int)Math.Truncate(e.motion.y),
                    e.motion.xrel,
                    e.motion.yrel
                );
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
                if (!game._quitting)
                {
                    game._quitting = true;
                    game.OnQuitting();
                }

                game._closeToken.Cancel();
                game.OnQuit();

                result = game._crashing
                    ? SDL_AppResult.SDL_APP_FAILURE
                    : SDL_AppResult.SDL_APP_SUCCESS;
                break;
            }
        }

        game.OnEvent(e);
        game._updateMutex.ReleaseMutex();
        return result;
    }

    /// <summary>
    /// Processes one iteration of the main loop.
    /// </summary>
    private static SDL_AppResult MainIter(Game game)
    {
        if (game._closeToken.IsCancellationRequested)
            return SDL_AppResult.SDL_APP_SUCCESS;

        var renderer = game.Renderer;
        game._updateMutex.WaitOne();

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

        game._updateMutex.ReleaseMutex();
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
    /// Handler for when SDL is logging a message.
    /// </summary>
    public delegate void LogMessageDelegate(int category, SDL_LogPriority priority, ReadOnlySpan<char> message);

    /// <summary>
    /// Raised when SDL is logging a message.
    /// </summary>
    public event LogMessageDelegate? LogMessage;

    /// <inheritdoc cref="LogMessageDelegate"/>
    protected virtual void OnLogMessage(int category, SDL_LogPriority priority, ReadOnlySpan<char> message)
    {
        LogMessage?.Invoke(category, priority, message);
    }

    /// <summary>
    /// Handler for when the game has first started.
    /// </summary>
    public delegate void InitDelegate();

    /// <summary>
    /// Raised when the game first starts.
    /// </summary>
    public event Action? Init;

    /// <inheritdoc cref="InitDelegate"/>
    protected virtual void OnInit()
    {
        Init?.Invoke();
    }

    /// <summary>
    /// Handles updating game state.
    /// </summary>
    /// <param name="delta">
    /// Amount of time that has passed since the last time game state was
    /// updated.
    /// </param>
    public delegate void UpdateDelegate(TimeSpan delta);

    /// <summary>
    /// Raised when the game logic should update.
    /// </summary>
    public event UpdateDelegate? Update;

    /// <inheritdoc cref="UpdateDelegate"/>
    protected virtual void OnUpdate(TimeSpan delta)
    {
        Update?.Invoke(delta);
    }

    /// <summary>
    /// Handles rendering game objects.
    /// </summary>
    /// <param name="delta">
    /// Amount of time that has passed since the last time game objects were
    /// rendered.
    /// </param>
    public delegate void DrawDelegate(TimeSpan delta);

    /// <summary>
    /// Raised when game objects should be rendered.
    /// </summary>
    public event DrawDelegate? Draw;

    /// <inheritdoc cref="DrawDelegate"/>
    protected virtual void OnDraw(TimeSpan delta)
    {
        Draw?.Invoke(delta);
    }

    /// <summary>
    /// Handles raw events from SDL.
    /// </summary>
    /// <param name="e">
    /// Event data.
    /// </param>
    /// <remarks>
    /// Modifications to event data will be reflected when the event is
    /// actually broadcast.
    /// </remarks>
    public delegate void PreEventDelegate(ref SDL_Event e);

    /// <summary>
    /// Raised when an SDL event is about to be handled.
    /// </summary>
    /// <remarks>
    /// Modifications to event data will be reflected when the event is
    /// actually broadcast.
    /// </remarks>
    public event PreEventDelegate? PreEvent;

    /// <inheritdoc cref="PreEventDelegate"/>
    protected virtual void OnPreEvent(ref SDL_Event e)
    {
        PreEvent?.Invoke(ref e);
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
    public delegate void EventDelegate(SDL_Event e);

    /// <summary>
    /// Raised when an SDL event was received and has been pre-processed.
    /// </summary>
    public event EventDelegate? Event;

    /// <inheritdoc cref="EventDelegate"/>
    protected virtual void OnEvent(SDL_Event e)
    {
        Event?.Invoke(e);
    }

    /// <summary>
    /// Handler for when a request to quit the game is made.
    /// </summary>
    public delegate void QuittingDelegate();

    /// <summary>
    /// Raised when the game is about to quit.
    /// </summary>
    public event QuittingDelegate? Quitting;

    /// <inheritdoc cref="QuittingDelegate"/>
    protected virtual void OnQuitting()
    {
        Quitting?.Invoke();
    }

    /// <summary>
    /// Handler for when the game has quit.
    /// </summary>
    public delegate void QuitDelegate();

    /// <summary>
    /// Raised when the game has quit.
    /// </summary>
    public event QuitDelegate? Quit;

    /// <inheritdoc cref="QuitDelegate"/>
    protected virtual void OnQuit()
    {
        Quit?.Invoke();
    }

    /// <summary>
    /// Handler for when a gamepad has been connected.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that was connected.
    /// </param>
    public delegate void GamepadConnectedDelegate(IntPtr<SDL_Gamepad> gamepad);

    /// <summary>
    /// Raised when a gamepad has been connected.
    /// </summary>
    public event GamepadConnectedDelegate? GamepadConnected;

    /// <inheritdoc cref="GamepadConnectedDelegate"/>
    protected virtual void OnGamepadConnected(IntPtr<SDL_Gamepad> gamepad)
    {
        GamepadConnected?.Invoke(gamepad);
    }

    /// <summary>
    /// Handler for when a gamepad has been disconnected.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that was disconnected.
    /// </param>
    public delegate void GamepadDisconnectedDelegate(IntPtr<SDL_Gamepad> gamepad);

    /// <summary>
    /// Raised when a gamepad has been disconnected.
    /// </summary>
    public event GamepadDisconnectedDelegate? GamepadDisconnected;

    /// <inheritdoc cref="GamepadDisconnectedDelegate"/>
    protected virtual void OnGamepadDisconnected(IntPtr<SDL_Gamepad> gamepad)
    {
        GamepadDisconnected?.Invoke(gamepad);
    }

    /// <summary>
    /// Handler for when a button on a gamepad has been pressed.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the button was pressed on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was pressed.
    /// </param>
    public delegate void GamepadButtonDownDelegate(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadButton button);

    /// <summary>
    /// Raised when a button on a gamepad has been pressed.
    /// </summary>
    public event GamepadButtonDownDelegate? GamepadButtonDown;

    /// <inheritdoc cref="GamepadButtonDownDelegate"/>
    protected virtual void OnGamepadButtonDown(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadButton button)
    {
        GamepadButtonDown?.Invoke(gamepad, button);
    }

    /// <summary>
    /// Handler for when a button on a gamepad has been released.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the button was released on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was released.
    /// </param>
    public delegate void GamepadButtonUpDelegate(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadButton button);

    /// <summary>
    /// Raised when a button on a gamepad has been released.
    /// </summary>
    public event GamepadButtonUpDelegate? GamepadButtonUp;

    /// <inheritdoc cref="GamepadButtonUpDelegate"/>
    protected virtual void OnGamepadButtonUp(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadButton button)
    {
        GamepadButtonUp?.Invoke(gamepad, button);
    }

    /// <summary>
    /// Handler for when a gamepad axis has changed.
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
    public delegate void GamepadAxisChangedDelegate(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadAxis axis, float value);

    /// <summary>
    /// Raised when a gamepad axis has changed.
    /// </summary>
    public event GamepadAxisChangedDelegate? GamepadAxisChanged;

    /// <inheritdoc cref="GamepadAxisChangedDelegate"/>
    protected virtual void OnGamepadAxisChanged(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadAxis axis, float value)
    {
        GamepadAxisChanged?.Invoke(gamepad, axis, value);
    }

    /// <summary>
    /// Handler for when a gamepad touchpad has been pressed.
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
    public delegate void GamepadTouchDownDelegate(IntPtr<SDL_Gamepad> gamepad, int padIndex, int fingerIndex,
        float pressure);

    /// <summary>
    /// Raised when a gamepad touchpad has been pressed.
    /// </summary>
    public event GamepadTouchDownDelegate? GamepadTouchDown;

    /// <inheritdoc cref="GamepadTouchDownDelegate"/>
    protected virtual void OnGamepadTouchDown(IntPtr<SDL_Gamepad> gamepad, int padIndex, int fingerIndex,
        float pressure)
    {
        GamepadTouchDown?.Invoke(gamepad, padIndex, fingerIndex, pressure);
    }

    /// <summary>
    /// Handler for when a gamepad touchpad has been moved.
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
    public delegate void GamepadTouchMoveDelegate(IntPtr<SDL_Gamepad> gamepad, int padIndex, int fingerIndex,
        float pressure);

    /// <summary>
    /// Raised when a gamepad touchpad has been moved.
    /// </summary>
    public event GamepadTouchMoveDelegate? GamepadTouchMove;

    /// <inheritdoc cref="GamepadTouchMoveDelegate"/>
    protected virtual void OnGamepadTouchMove(IntPtr<SDL_Gamepad> gamepad, int padIndex, int fingerIndex,
        float pressure)
    {
        GamepadTouchMove?.Invoke(gamepad, padIndex, fingerIndex, pressure);
    }

    /// <summary>
    /// Handler for when a gamepad touchpad has been released.
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
    public delegate void GamepadTouchUpDelegate(IntPtr<SDL_Gamepad> gamepad, int padIndex, int fingerIndex,
        float pressure);

    /// <summary>
    /// Raised when a gamepad touchpad has been released.
    /// </summary>
    public event GamepadTouchUpDelegate? GamepadTouchUp;

    /// <inheritdoc cref="GamepadTouchUpDelegate"/>
    protected virtual void OnGamepadTouchUp(IntPtr<SDL_Gamepad> gamepad, int padIndex, int fingerIndex, float pressure)
    {
        GamepadTouchUp?.Invoke(gamepad, padIndex, fingerIndex, pressure);
    }

    /// <summary>
    /// Handler for when a mouse has been connected.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that was connected.
    /// </param>
    public delegate void MouseConnectedDelegate(SDL_MouseID mouse);

    /// <summary>
    /// Raised when a mouse has been connected.
    /// </summary>
    public event MouseConnectedDelegate? MouseConnected;

    /// <inheritdoc cref="MouseConnectedDelegate"/>
    protected virtual void OnMouseConnected(SDL_MouseID mouse)
    {
        MouseConnected?.Invoke(mouse);
    }

    /// <summary>
    /// Handler for when a mouse has been disconnected.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that was disconnected.
    /// </param>
    public delegate void MouseDisconnectedDelegate(SDL_MouseID mouse);

    /// <summary>
    /// Raised when a mouse has been disconnected.
    /// </summary>
    public event MouseDisconnectedDelegate? MouseDisconnected;

    /// <inheritdoc cref="MouseDisconnectedDelegate"/>
    protected virtual void OnMouseDisconnected(SDL_MouseID mouse)
    {
        MouseDisconnected?.Invoke(mouse);
    }

    /// <summary>
    /// Handler for when a mouse button has been pressed.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that the button was pressed on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was pressed.
    /// </param>
    public delegate void MouseButtonDownDelegate(SDL_MouseID mouse, SDLButton button);

    /// <summary>
    /// Raised when a mouse button has been pressed.
    /// </summary>
    public event MouseButtonDownDelegate? MouseButtonDown;

    /// <inheritdoc cref="MouseButtonDownDelegate"/>
    protected virtual void OnMouseButtonDown(SDL_MouseID mouse, SDLButton button)
    {
        MouseButtonDown?.Invoke(mouse, button);
    }

    /// <summary>
    /// Handler for when a mouse button has been released.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that the button was released on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was released.
    /// </param>
    public delegate void MouseButtonUpDelegate(SDL_MouseID mouse, SDLButton button);

    /// <summary>
    /// Raised when a mouse button has been released.
    /// </summary>
    public event MouseButtonUpDelegate? MouseButtonUp;

    /// <inheritdoc cref="MouseButtonUpDelegate"/>
    protected virtual void OnMouseButtonUp(SDL_MouseID mouse, SDLButton button)
    {
        MouseButtonUp?.Invoke(mouse, button);
    }

    /// <summary>
    /// Handler for when a mouse has moved.
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
    public delegate void MouseMoveDelegate(SDL_MouseID mouse, int x, int y, float dx, float dy);

    /// <summary>
    /// Raised when a mouse has moved.
    /// </summary>
    public event MouseMoveDelegate? MouseMove;

    /// <inheritdoc cref="MouseMoveDelegate"/>
    protected virtual void OnMouseMove(SDL_MouseID mouse, int x, int y, float dx, float dy)
    {
        MouseMove?.Invoke(mouse, x, y, dx, dy);
    }

    /// <summary>
    /// Handler for when a mouse wheel has moved.
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
    public delegate void MouseWheelDelegate(SDL_MouseID mouse, float x, float y, SDL_MouseWheelDirection direction);

    /// <summary>
    /// Raised when a mouse wheel has moved.
    /// </summary>
    public event MouseWheelDelegate? MouseWheel;

    /// <inheritdoc cref="MouseWheelDelegate"/>
    protected virtual void OnMouseWheel(SDL_MouseID mouse, float x, float y, SDL_MouseWheelDirection direction)
    {
        MouseWheel?.Invoke(mouse, x, y, direction);
    }

    /// <summary>
    /// Handler for when a key has been pressed.
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
    public delegate void KeyDownDelegate(SDL_KeyboardID keyboard, SDL_Keycode code, SDL_Scancode scan, SDL_Keymod mod);

    /// <summary>
    /// Raised when a keyboard key has been pressed.
    /// </summary>
    public event KeyDownDelegate? KeyDown;

    /// <inheritdoc cref="KeyDownDelegate"/>
    protected virtual void OnKeyDown(SDL_KeyboardID keyboard, SDL_Keycode code, SDL_Scancode scan, SDL_Keymod mod)
    {
        KeyDown?.Invoke(keyboard, code, scan, mod);
    }

    /// <summary>
    /// Handler for when a key has been released.
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
    public delegate void KeyUpDelegate(SDL_KeyboardID keyboard, SDL_Keycode code, SDL_Scancode scan, SDL_Keymod mod);

    /// <summary>
    /// Raised when a keyboard key has been released.
    /// </summary>
    public event KeyUpDelegate? KeyUp;

    /// <inheritdoc cref="KeyUpDelegate"/>
    protected virtual void OnKeyUp(SDL_KeyboardID keyboard, SDL_Keycode code, SDL_Scancode scan, SDL_Keymod mod)
    {
        KeyUp?.Invoke(keyboard, code, scan, mod);
    }

    /// <summary>
    /// Handler for when a keyboard has been connected.
    /// </summary>
    /// <param name="keyboard">
    /// ID of the keyboard that was connected.
    /// </param>
    public delegate void KeyboardConnectedDelegate(SDL_KeyboardID keyboard);

    /// <summary>
    /// Raised when a keyboard has been connected.
    /// </summary>
    public event KeyboardConnectedDelegate? KeyboardConnected;

    /// <inheritdoc cref="KeyboardConnectedDelegate"/>
    protected virtual void OnKeyboardConnected(SDL_KeyboardID keyboard)
    {
        KeyboardConnected?.Invoke(keyboard);
    }

    /// <summary>
    /// Handler for when a keyboard has been disconnected.
    /// </summary>
    /// <param name="keyboard">
    /// ID of the keyboard that was disconnected.
    /// </param>
    public delegate void KeyboardDisconnectedDelegate(SDL_KeyboardID keyboard);

    /// <summary>
    /// Raised when a keyboard has been disconnected.
    /// </summary>
    public event KeyboardDisconnectedDelegate? KeyboardDisconnected;

    /// <inheritdoc cref="KeyboardDisconnectedDelegate"/>
    protected virtual void OnKeyboardDisconnected(SDL_KeyboardID keyboard)
    {
        KeyboardDisconnected?.Invoke(keyboard);
    }

    /// <summary>
    /// Handler for when the game window gains focus.
    /// </summary>
    public delegate void FocusGainedDelegate();

    /// <summary>
    /// Raised when the game window gains focus.
    /// </summary>
    public event FocusGainedDelegate? FocusGained;

    /// <inheritdoc cref="FocusGainedDelegate"/>
    protected virtual void OnFocusGained()
    {
        FocusGained?.Invoke();
    }

    /// <summary>
    /// Handler for when the game window loses focus.
    /// </summary>
    public delegate void FocusLostDelegate();

    /// <summary>
    /// Raised when the game window loses focus.
    /// </summary>
    public event FocusLostDelegate? FocusLost;

    /// <inheritdoc cref="FocusLostDelegate"/>
    protected virtual void OnFocusLost()
    {
        FocusLost?.Invoke();
    }

    /// <summary>
    /// Handler for when <see cref="Dispose"/> is called.
    /// </summary>
    public delegate void DisposedDelegate();

    /// <summary>
    /// Raised when <see cref="Dispose"/> is called.
    /// </summary>
    public event DisposedDelegate? Disposed;

    /// <inheritdoc cref="DisposedDelegate"/>
    protected virtual void OnDispose()
    {
        Disposed?.Invoke();
    }

    /// <summary>
    /// Handler for when the game scene is about to be presented to the window.
    /// </summary>
    public delegate void PresentDelegate(TimeSpan delta);

    /// <summary>
    /// Raised when the game scene is about to be presented to the window.
    /// </summary>
    public event PresentDelegate? Present;

    /// <inheritdoc cref="PresentDelegate"/>
    protected virtual void OnPresent(TimeSpan delta)
    {
        Present?.Invoke(delta);
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