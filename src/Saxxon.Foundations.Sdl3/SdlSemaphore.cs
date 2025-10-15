using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Semaphore"/>.
/// Prefer using <see cref="System.Threading.SemaphoreSlim"/> where possible.
/// </summary>
[PublicAPI]
public static class SdlSemaphore
{
    /// <summary>
    /// Gets the current value of a semaphore.
    /// </summary>
    /// <param name="sem">
    /// The semaphore to query.
    /// </param>
    /// <returns>
    /// The current value of the semaphore.
    /// </returns>
    public static unsafe uint GetValue(
        this IntPtr<SDL_Semaphore> sem
    )
    {
        return SDL_GetSemaphoreValue(sem);
    }

    /// <summary>
    /// Atomically increment a semaphore's value and wake waiting threads.
    /// </summary>
    /// <param name="sem">
    /// The semaphore to increment.
    /// </param>
    public static unsafe void Signal(
        this IntPtr<SDL_Semaphore> sem
    )
    {
        SDL_SignalSemaphore(sem);
    }

    /// <summary>
    /// Waits until a semaphore has a positive value and then decrements it.
    /// </summary>
    /// <param name="sem">
    /// The semaphore to wait on.
    /// </param>
    /// <param name="timeout">
    /// The length of the timeout, or null to wait indefinitely.
    /// </param>
    /// <returns>
    /// True if the wait succeeds or false if the wait times out.
    /// </returns>
    /// <remarks>
    /// This function suspends the calling thread until either the semaphore pointed to by sem has a positive value
    /// or the specified time has elapsed. If the call is successful it will atomically decrement the semaphore value.
    /// </remarks>
    public static unsafe bool WaitTimeout(
        this IntPtr<SDL_Semaphore> sem,
        TimeSpan? timeout = null
    )
    {
        return SDL_WaitSemaphoreTimeout(sem, timeout?.ToMilliseconds() ?? -1);
    }

    /// <summary>
    /// Checks if a semaphore has a positive value and decrement it if it does.
    /// </summary>
    /// <param name="sem">
    /// The semaphore to wait on.
    /// </param>
    /// <returns>
    /// True if the wait succeeds, false if the wait would block.
    /// </returns>
    /// <remarks>
    /// This function checks to see if the semaphore pointed to by sem has a positive value and atomically decrements
    /// the semaphore value if it does. If the semaphore doesn't have a positive value, the function immediately
    /// returns false.
    /// </remarks>
    public static unsafe bool TryWait(
        this IntPtr<SDL_Semaphore> sem
    )
    {
        return SDL_TryWaitSemaphore(sem);
    }

    /// <summary>
    /// Waits until a semaphore has a positive value and then decrements it.
    /// </summary>
    /// <param name="sem">
    /// The semaphore wait on.
    /// </param>
    /// <remarks>
    /// This function suspends the calling thread until the semaphore pointed to by sem has a positive value,
    /// and then atomically decrements the semaphore value.
    /// </remarks>
    public static unsafe void Wait(
        this IntPtr<SDL_Semaphore> sem
    )
    {
        SDL_WaitSemaphore(sem);
    }

    /// <summary>
    /// Destroys a semaphore.
    /// </summary>
    /// <param name="sem">
    /// The semaphore to destroy.
    /// </param>
    /// <remarks>
    /// It is not safe to destroy a semaphore if there are threads currently waiting on it.
    /// </remarks>
    public static unsafe void Destroy(
        this IntPtr<SDL_Semaphore> sem
    )
    {
        SDL_DestroySemaphore(sem);
    }

    /// <summary>
    /// Creates a semaphore.
    /// </summary>
    /// <param name="initialValue">
    /// The starting value of the semaphore.
    /// </param>
    /// <returns>
    /// A new semaphore.
    /// </returns>
    /// <remarks>
    /// This function creates a new semaphore and initializes it with the initial value. Each wait operation on
    /// the semaphore will atomically decrement the semaphore value and potentially block if the semaphore value is 0.
    /// Each post operation will atomically increment the semaphore value and wake waiting threads and allow them to
    /// retry the wait operation.
    /// </remarks>
    public static unsafe IntPtr<SDL_Semaphore> Create(
        uint initialValue
    )
    {
        return ((IntPtr<SDL_Semaphore>)SDL_CreateSemaphore(initialValue))
            .AssertSdlNotNull();
    }
}