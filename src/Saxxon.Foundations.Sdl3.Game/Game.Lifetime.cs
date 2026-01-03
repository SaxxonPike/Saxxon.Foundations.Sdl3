using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Saxxon.Foundations.Sdl3.Game;

public abstract partial class Game
{
    /// <summary>
    /// True if the game is quitting.
    /// </summary>
    private bool _quitting;

    /// <summary>
    /// True if an unhandled exception has occurred.
    /// </summary>
    private bool _crashing;

    /// <summary>
    /// Nonzero only when the unhandled exception handler has been installed.
    /// </summary>
    private static int _crashHandlerInstalled;

    /// <summary>
    /// Queue used for pending game states.
    /// </summary>
    private static readonly ConcurrentQueue<Game> PendingGames = [];

    /// <summary>
    /// Command line arguments that were passed to the game.
    /// </summary>
    private List<string> _args = [];
    
    /// <summary>
    /// Command line arguments that were passed to the game.
    /// </summary>
    protected IReadOnlyList<string> Args => _args;

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
    /// When an unhandled exception bubbles out to the app domain, the app
    /// is crashing in a way that is unrecoverable. This function is called when
    /// this happens, to make sure that SDL is shut down properly.
    /// </summary>
    private void HandleCrash(object sender, UnhandledExceptionEventArgs e)
    {
        Console.WriteLine(e.ExceptionObject);
        SdlLib.Quit();
        OnCrash(e.ExceptionObject as Exception);
    }

    /// <summary>
    /// Called when the application has encountered an unhandled exception, after
    /// which recovery is not possible.
    /// </summary>
    /// <param name="exception">
    /// Exception information.
    /// </param>
    /// <remarks>
    /// SDL_Quit will have been called before this method is called.
    /// </remarks>
    protected virtual void OnCrash(Exception? exception)
    {
    }
}