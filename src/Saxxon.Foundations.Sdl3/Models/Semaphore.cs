using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Semaphore"/>.
/// Prefer using <see cref="System.Threading.SemaphoreSlim"/> where possible.
/// </summary>
[PublicAPI]
public static class Semaphore
{
    public static unsafe uint GetValue(
        this IntPtr<SDL_Semaphore> sem
    )
    {
        return SDL_GetSemaphoreValue(sem);
    }

    public static unsafe void Signal(
        this IntPtr<SDL_Semaphore> sem
    )
    {
        SDL_SignalSemaphore(sem);
    }

    public static unsafe bool WaitTimeout(
        this IntPtr<SDL_Semaphore> sem,
        TimeSpan timeout
    )
    {
        return SDL_WaitSemaphoreTimeout(sem, timeout.ToMilliseconds());
    }

    public static unsafe bool TryWait(
        this IntPtr<SDL_Semaphore> sem
    )
    {
        return SDL_TryWaitSemaphore(sem);
    }

    public static unsafe void Wait(
        this IntPtr<SDL_Semaphore> sem
    )
    {
        SDL_WaitSemaphore(sem);
    }

    public static unsafe void Destroy(
        this IntPtr<SDL_Semaphore> sem
    )
    {
        SDL_DestroySemaphore(sem);
    }

    public static unsafe IntPtr<SDL_Semaphore> Create(
        uint initialValue
    )
    {
        return SDL_CreateSemaphore(initialValue);
    }
}