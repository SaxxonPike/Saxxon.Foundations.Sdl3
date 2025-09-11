using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Handles interaction with SDL's application lifecycle.
/// </summary>
[PublicAPI]
internal sealed unsafe class AppMain
{
    /// <summary>
    /// Delegate that represents SDL_AppInit_func.
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate SDL_AppResult AppInitDelegate(
        IntPtr* appState,
        int argc,
        IntPtr argv
    );

    // A reference to this delegate needs to be kept, otherwise the GC could
    // potentially free it and crash the app.
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly AppInitDelegate _appInitDelegate;

    /// <summary>
    /// Holds a pointer to the app state (named UserData for SDL callbacks.)
    /// </summary>
    private IntPtr _appState;

    /// <summary>
    /// Holds the unmanaged function pointer to the init handler.
    /// </summary>
    private readonly delegate* unmanaged[Cdecl]<IntPtr*, int, byte**, SDL_AppResult> _appInit;

    /// <summary>
    /// Callback for initialization of the app.
    /// </summary>
    public required Func<Utf8SpanArray, (SDL_AppResult, object?)> OnInit { get; init; }

    /// <summary>
    /// Callback for one iteration of the main loop.
    /// </summary>
    public required Func<object?, SDL_AppResult> OnIter { get; init; }

    /// <summary>
    /// Callback for when an event is received.
    /// </summary>
    public required Func<object?, IntPtr, SDL_AppResult> OnEvent { get; init; }

    /// <summary>
    /// Callback for when the application is shutting down.
    /// </summary>
    public required Action<object?, SDL_AppResult> OnQuit { get; init; }

    public AppMain()
    {
        var appMainState = UserDataStore.Add(this);

        // App state has to be initialized in here due to how we need to map
        // UserData.

        _appInitDelegate = (mainState, argc, argv) =>
        {
            var (result, appStateObj) = OnInit!(
                new Utf8SpanArray(
                    new ReadOnlySpan<IntPtr>(
                        (IntPtr*)argv,
                        argc
                    )
                )
            );

            _appState = UserDataStore.Add(appStateObj);
            *mainState = appMainState;
            return result;
        };

        _appInit = (delegate* unmanaged[Cdecl]<IntPtr*, int, byte**, SDL_AppResult>)
            Marshal.GetFunctionPointerForDelegate(_appInitDelegate);
    }

    /// <summary>
    /// SDL_AppIterate_func.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDL_AppResult HandleIter(
        IntPtr mainState
    )
    {
        var main = UserDataStore.Get<AppMain>(mainState);
        return main!.OnIter(UserDataStore.Get(main._appState));
    }

    /// <summary>
    /// SDL_AppEvent_func.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDL_AppResult HandleEvent(
        IntPtr mainState,
        SDL_Event* @event
    )
    {
        var main = UserDataStore.Get<AppMain>(mainState);
        return main!.OnEvent(UserDataStore.Get(main._appState), (IntPtr)@event);
    }

    /// <summary>
    /// SDL_AppQuit_func.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void HandleQuit(
        IntPtr mainState,
        SDL_AppResult result
    )
    {
        var main = UserDataStore.Get<AppMain>(mainState);
        main!.OnQuit(UserDataStore.Get(main._appState), result);
        UserDataStore.Remove(main._appState);
    }

    /// <summary>
    /// Instruct SDL to begin the application lifetime.
    /// </summary>
    public int Enter(int argc, IntPtr argv)
    {
        return SDL_EnterAppMainCallbacks(
            argc,
            (byte**)argv,
            _appInit,
            &HandleIter,
            &HandleEvent,
            &HandleQuit
        );
    }
}

/// <summary>
/// Handles interaction with SDL's application lifecycle.
/// </summary>
[PublicAPI]
public sealed unsafe class AppMain<TState> where TState : class
{
    /// <summary>
    /// The underlying <see cref="AppMain"/> instance which receives the
    /// actual low-level requests from SDL.
    /// </summary>
    private AppMain _appMain;

    /// <summary>
    /// Describes a function that will receive events from SDL.
    /// </summary>
    public delegate SDL_AppResult EventDelegate(
        TState appState,
        ref SDL_Event @event
    );

    /// <summary>
    /// Describes a function that represents one iteration of the main loop.
    /// </summary>
    public delegate SDL_AppResult IterateDelegate(
        TState appState
    );

    /// <summary>
    /// Describes a function that will initialize app state and receive command
    /// line parameters.
    /// </summary>
    public delegate (SDL_AppResult, TState?) InitDelegate(
        string[] args
    );

    /// <summary>
    /// Describes a function that will close the app and free allocated
    /// resources.
    /// </summary>
    public delegate void QuitDelegate(
        TState appState,
        SDL_AppResult result
    );

    /// <summary>
    /// Handler that will receive SDL events.
    /// </summary>
    public EventDelegate? OnEvent { get; init; }

    /// <summary>
    /// Handler that will be invoked once per iteration of the main loop.
    /// </summary>
    public IterateDelegate? OnIterate { get; init; }

    /// <summary>
    /// Handler that will be invoked when the application is first starting.
    /// </summary>
    public InitDelegate? OnInit { get; init; }

    /// <summary>
    /// Handler that will be invoked when the application is shutting down.
    /// </summary>
    public QuitDelegate? OnQuit { get; init; }

    /// <summary>
    /// When cancellation is requested via this token, the application will be
    /// signalled to shut down immediately.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }

    /// <summary>
    /// Build an app instance.
    /// </summary>
    public AppMain()
    {
        _appMain = new AppMain
        {
            OnInit = args =>
                OnInit?.Invoke(args.ToArray()) ??
                (SDL_AppResult.SDL_APP_CONTINUE, null),
            OnIter = state =>
                OnIterate?.Invoke((TState)state!) ??
                SDL_AppResult.SDL_APP_CONTINUE,
            OnEvent = (state, @event) =>
                OnEvent?.Invoke((TState)state!, ref Unsafe.AsRef<SDL_Event>((void*)@event)) ??
                SDL_AppResult.SDL_APP_CONTINUE,
            OnQuit = (state, result) =>
                OnQuit?.Invoke((TState)state!, result)
        };
    }

    /// <summary>
    /// Begin executing the app instance using the SDL app lifecycle.
    /// </summary>
    public int Enter(params string[] args)
    {
        using var argMem = new Utf8ByteStrings(args);
        return _appMain.Enter(args.Length, argMem.Ptr);
    }
}