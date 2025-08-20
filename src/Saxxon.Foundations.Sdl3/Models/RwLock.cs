using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_RWLock"/>.
/// </summary>
[PublicAPI]
public static class RwLock
{
    public static unsafe void Destroy(
        this IntPtr<SDL_RWLock> @lock
    )
    {
        SDL_DestroyRWLock(@lock);
    }

    public static unsafe void Unlock(
        this IntPtr<SDL_RWLock> @lock
    )
    {
        SDL_UnlockRWLock(@lock);
    }

    public static unsafe bool TryLockForWriting(
        this IntPtr<SDL_RWLock> @lock
    )
    {
        return SDL_TryLockRWLockForWriting(@lock);
    }
    
    public static unsafe bool TryLockForReading(
        this IntPtr<SDL_RWLock> @lock
    )
    {
        return SDL_TryLockRWLockForReading(@lock);
    }

    public static unsafe IntPtr<SDL_RWLock> Create()
    {
        return SDL_CreateRWLock();
    }
}