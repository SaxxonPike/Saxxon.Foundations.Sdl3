using System.Diagnostics;
using Saxxon.Foundations.Sdl3.Delegates;

namespace Saxxon.Foundations.Sdl3.Game;

public abstract partial class Game
{
    /// <summary>
    /// Handler for the update timer interval.
    /// </summary>
    private SdlTimer.TimerCallback? _updateCallback;

    /// <summary>
    /// Mutex that indicates when updates are in progress.
    /// </summary>
    private Mutex _updateMutex = new();

    /// <summary>
    /// Interval of the update timer. This can be thought of as the amount of
    /// time between "game ticks".
    /// </summary>
    private TimeSpan _updateInterval = TimeSpan.FromSeconds(1) / 200;

    /// <summary>
    /// Holds the <see cref="CancellationToken"/> used for signaling the game closing.
    /// </summary>
    private CancellationTokenSource _closeToken = new();

    /// <summary>
    /// Raw timestamp of the last draw event.
    /// </summary>
    private ulong _lastDraw;

    /// <summary>
    /// Raw timestamp of the last video frame presentation.
    /// </summary>
    private ulong _lastPresent;

    /// <summary>
    /// Raw timestamp of the last update event.
    /// </summary>
    private ulong _lastUpdate;

    /// <summary>
    /// True while drawing is suspended - when the window is occluded fully, for instance.
    /// </summary>
    private bool _suspendDraw;

    /// <summary>
    /// Number of times per second that game state updates should be performed.
    /// </summary>
    protected virtual double UpdatesPerSecond
    {
        get => TimeSpan.FromSeconds(1) / _updateInterval;
        set => _updateInterval = TimeSpan.FromSeconds(1) / value;
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

        PendingGames.Enqueue(this);

        var main = new AppMain<Game>
        {
            CancellationToken = _closeToken.Token,
            OnEvent = MainEvent,
            OnInit = MainInit,
            OnIterate = MainIter,
            OnQuit = MainQuit
        };

        if (Interlocked.Increment(ref _crashHandlerInstalled) == 1)
            AppDomain.CurrentDomain.UnhandledException += HandleCrash;

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

            if (Interlocked.Decrement(ref _crashHandlerInstalled) == 0)
                AppDomain.CurrentDomain.UnhandledException -= HandleCrash;
        }
    }

    /// <summary>
    /// Handles setting up the game before the main loop.
    /// </summary>
    private static (SDL_AppResult, Game?) MainInit(string[] args)
    {
        if (!PendingGames.TryDequeue(out var game))
            return (SDL_AppResult.SDL_APP_FAILURE, null!);

        game._args = args.ToList();
        game._logOutputFunction = new LogOutputFunction(game.OnLogging);

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
            Log.Error("Failed to initialize gamepad subsystem.");
        }

        try
        {
            if (game.ShouldInitMixer)
                MixerLib.Init();
        }
        catch
        {
            Log.Error("Failed to initialize mixer subsystem.");
        }

        try
        {
            TtfLib.Init();
        }
        catch
        {
            Log.Error("Failed to initialize font subsystem.");
        }

        //
        // Create the game window and renderer.
        //

        (game.Window, game.Renderer) = Sdl3.Window
            .CreateWithRenderer(game.Title, 1280, 720, SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

        game.SetLetterboxOn();

        Hint.Set(SDL_HINT_RENDER_VSYNC, "1");
        Hint.Set(SDL_HINT_EVENT_LOGGING, "1");
        Hint.Set(SDL_HINT_QUIT_ON_LAST_WINDOW_CLOSE, "1");

        //
        // Create the backbuffer texture.
        //

        var backBufferFormat = game.Window.GetPixelFormat();
        var (canvasWidth, canvasHeight) = game.CanvasSize;

        game.Backbuffer = Texture.Create(
            game.Renderer, backBufferFormat, SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, canvasWidth, canvasHeight
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

        if (game.ShouldInitMixer)
        {
            game.Mixer = Sdl3.Mixer
                .CreateDevice(SDL_AUDIO_DEVICE_DEFAULT_PLAYBACK, null);
        }

        //
        // Initialize game state.
        //

        game._lastDraw = game._lastPresent = Time.GetNowNanoseconds();
        game.OnStarting();

        //
        // Initialize update timer.
        //

        game._lastUpdate = Time.GetNowNanoseconds();
        game._updateInterval = TimeSpan.FromSeconds(1) / game.UpdatesPerSecond;
        game._updateCallback = (_, _) =>
        {
            //
            // Cancellation will stop the update timer.
            //

            if (game._closeToken.IsCancellationRequested)
                return TimeSpan.Zero;

            //
            // While this callback may run on a different thread, it can be
            // assumed that the letterbox mode is on when entering the mutex.
            //

            game._updateMutex.WaitOne();
            var now = Time.GetNowNanoseconds();
            game.OnUpdating(Time.GetFromNanoseconds(now - game._lastUpdate));
            game._lastUpdate = now;
            game._updateMutex.ReleaseMutex();

            return game._updateInterval;
        };

        SdlTimer.Create(game._updateCallback, game._updateInterval);
        game._messageEventType = Events.Register(1);

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
    /// Processes one iteration of the main loop.
    /// </summary>
    private static SDL_AppResult MainIter(Game game)
    {
        //
        // If the game window is occluded, no point in trying to render anything.
        // Cancellation will also prevent drawing.
        //

        if (game._closeToken.IsCancellationRequested)
            return SDL_AppResult.SDL_APP_SUCCESS;

        if (game._suspendDraw)
            return SDL_AppResult.SDL_APP_CONTINUE;

        game._updateMutex.WaitOne();

        //
        // Render game objects.
        //

        var renderer = game.Renderer;
        game.SetLetterboxOff();
        renderer.SetTarget(game.Backbuffer);

        var clearColor = game.ClearColor;
        renderer.SetColorFloat(clearColor.r, clearColor.g, clearColor.b, clearColor.a);
        renderer.Clear();

        var thisDraw = Time.GetNowNanoseconds();
        game.OnDrawing(Time.GetFromNanoseconds(thisDraw - game._lastDraw));
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
        game.OnPresenting(Time.GetFromNanoseconds(thisPresent - game._lastPresent));
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
}