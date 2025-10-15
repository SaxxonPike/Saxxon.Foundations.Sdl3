using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Condition"/>.
/// See <see cref="System.Threading.Channels.Channel"/> for a fully managed
/// alternative.
/// </summary>
[PublicAPI]
public static class Condition
{
    /// <summary>
    /// Waits until a condition variable is signaled or a certain time has
    /// passed.
    /// </summary>
    /// <param name="cond">
    /// The condition variable to wait on.
    /// </param>
    /// <param name="mutex">
    /// The mutex used to coordinate thread access.
    /// </param>
    /// <param name="timeout">
    /// The maximum time to wait, or null to wait indefinitely.
    /// </param>
    /// <returns>
    /// True if the condition variable is signaled, false if the condition is
    /// not signaled in the allotted time.
    /// </returns>
    public static unsafe bool WaitTimeout(
        this IntPtr<SDL_Condition> cond,
        IntPtr<SDL_Mutex> mutex,
        TimeSpan? timeout
    )
    {
        return SDL_WaitConditionTimeout(cond, mutex, timeout?.ToMilliseconds() ?? -1);
    }

    /// <summary>
    /// Waits until a condition variable is signaled.
    /// </summary>
    /// <param name="cond">
    /// The condition variable to wait on.
    /// </param>
    /// <param name="mutex">
    /// The mutex used to coordinate thread access.
    /// </param>
    public static unsafe void Wait(
        this IntPtr<SDL_Condition> cond,
        IntPtr<SDL_Mutex> mutex
    )
    {
        SDL_WaitCondition(cond, mutex);
    }

    public static unsafe void Broadcast(
        this IntPtr<SDL_Condition> cond
    )
    {
        SDL_BroadcastCondition(cond);
    }

    public static unsafe void Signal(
        this IntPtr<SDL_Condition> cond
    )
    {
        SDL_SignalCondition(cond);
    }

    public static unsafe void Destroy(
        this IntPtr<SDL_Condition> cond
    )
    {
        SDL_DestroyCondition(cond);
    }

    public static unsafe IntPtr<SDL_Condition> Create()
    {
        return SDL_CreateCondition();
    }
}