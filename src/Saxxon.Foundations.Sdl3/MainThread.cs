using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for the main SDL thread.
/// </summary>
[PublicAPI]
public static class MainThread
{
    /// <summary>
    /// Calls a function on the main thread during event processing.
    /// </summary>
    /// <param name="func">
    /// The callback to call on the main thread.
    /// </param>
    /// <param name="waitComplete">
    /// True to wait for the callback to complete, false to return immediately.
    /// </param>
    public static unsafe void Run(Action func, bool waitComplete)
    {
        var userData = UserDataStore.Add(func);

        SDL_RunOnMainThread(&Execute, userData, waitComplete)
            .AssertSdlSuccess();

        return;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static void Execute(IntPtr userData)
        {
            UserDataStore.Remove<Action>(userData, out var target);
            target?.Invoke();
        }
    }

    /// <summary>
    /// Determines whether the currently executing thread is the main thread.
    /// </summary>
    /// <returns>
    /// True if this thread is the main thread, or false otherwise.
    /// </returns>
    /// <remarks>
    /// On Apple platforms, the main thread is the thread that runs your program's main() entry point.
    /// On other platforms, the main thread is the one that calls SDL_Init(SDL_INIT_VIDEO), which should usually
    /// be the one that runs your program's main() entry point. If you are using the main callbacks, SDL_AppInit(),
    /// SDL_AppIterate(), and SDL_AppQuit() are all called on the main thread.
    /// </remarks>
    public static bool IsCurrent()
    {
        return SDL_IsMainThread();
    }
}