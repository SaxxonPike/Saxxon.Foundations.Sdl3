using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Mutex"/>. Prefer
/// using <see cref="System.Threading.Mutex"/> where possible.
/// </summary>
[PublicAPI]
public static class Mutex
{
    public static unsafe void Destroy(
        this IntPtr<SDL_Mutex> mutex
    )
    {
        SDL_DestroyMutex(mutex);
    }

    public static unsafe void Unlock(
        this IntPtr<SDL_Mutex> mutex
    )
    {
        SDL_UnlockMutex(mutex);
    }

    public static unsafe bool TryLock(
        this IntPtr<SDL_Mutex> mutex
    )
    {
        return SDL_TryLockMutex(mutex);
    }

    public static unsafe void Lock(
        this IntPtr<SDL_Mutex> mutex
    )
    {
        SDL_LockMutex(mutex);
    }

    public static unsafe IntPtr<SDL_Mutex> Create()
    {
        return SDL_CreateMutex();
    }
}