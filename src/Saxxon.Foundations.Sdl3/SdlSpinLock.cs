using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_SpinLock"/>.
/// Prefer using <see cref="System.Threading.SpinLock"/> where possible.
/// </summary>
[PublicAPI]
public static class SdlSpinLock
{
    /// <summary>
    /// Allocates a spin lock.
    /// </summary>
    public static IntPtr<SDL_SpinLock> Create(bool locked = false)
    {
        var result = Mem.Alloc<SDL_SpinLock>(1);
        result[0] = locked ? (SDL_SpinLock)1 : 0;
        return result;
    }

    /// <summary>
    /// Frees an allocated spin lock.
    /// </summary>
    public static void Destroy(this IntPtr<SDL_SpinLock> spinlock)
    {
        Mem.Free(spinlock);
    }

    /// <summary>
    /// Lock a spin lock by setting it to a non-zero value.
    /// </summary>
    /// <param name="spinlock">
    /// Reference to a spin lock.
    /// </param>
    public static unsafe void Lock(this IntPtr<SDL_SpinLock> spinlock)
    {
        SDL_LockSpinlock((int*)spinlock.Ptr);
    }

    /// <summary>
    /// Tries to lock a spin lock by setting it to a non-zero value.
    /// </summary>
    /// <param name="spinlock">
    /// Spin lock to try locking.
    /// </param>
    /// <returns>
    /// True only if the lock was successful, or false if the spin lock is
    /// already locked.
    /// </returns>
    public static unsafe bool TryLock(this IntPtr<SDL_SpinLock> spinlock)
    {
        return SDL_TryLockSpinlock((int*)spinlock);
    }

    /// <summary>
    /// Unlocks a spin lock by setting it to 0.
    /// </summary>
    /// <param name="spinlock">
    /// Spin lock to unlock.
    /// </param>
    public static unsafe void Unlock(this IntPtr<SDL_SpinLock> spinlock)
    {
        SDL_UnlockSpinlock((int*)spinlock);
    }
}