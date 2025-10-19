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
    /// True to wait for the callback to complete, or false to return immediately.
    /// </param>
    /// <remarks>
    /// If this is called on the main thread, the callback is executed immediately. If this is called on another
    /// thread, this callback is queued for execution on the main thread during event processing.
    ///
    /// Be careful of deadlocks when using this functionality. You should not have the main thread wait for the current
    /// thread while this function is being called with waitComplete = true.
    /// </remarks>
    public static unsafe void Run(Action func, bool waitComplete = false)
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
    /// Returns whether this is the main thread.
    /// </summary>
    /// <remarks>
    /// On Apple platforms, the main thread is the thread that runs your program's main() entry point.
    /// On other platforms, the main thread is the one that calls <see cref="Sdl.Init"/> with
    /// <see cref="SDL_InitFlags.SDL_INIT_VIDEO"/>, which should usually be the one that runs your program's main()
    /// entry point. If you are using <see cref="AppMain"/>, Init, Iterate, and Quit functions are all
    /// called on the main thread.
    /// </remarks>
    public static bool IsCurrent()
    {
        return SDL_IsMainThread();
    }
}