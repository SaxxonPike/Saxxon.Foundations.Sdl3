using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for atomic operations that deal with
/// <see cref="SDL_AtomicInt"/>, <see cref="SDL_AtomicU32"/> and pointers.
/// Prefer using C#'s built-in <see cref="Interlocked"/> API where possible.
/// </summary>
[PublicAPI]
public static class Atomic
{
    public static unsafe int Add(
        this IntPtr<SDL_AtomicInt> atomic,
        int value
    )
    {
        return SDL_AddAtomicInt(atomic, value);
    }

    public static unsafe uint Add(
        this IntPtr<SDL_AtomicU32> atomic,
        int value
    )
    {
        return SDL_AddAtomicU32(atomic, value);
    }

    public static unsafe bool CompareAndSwap(
        this IntPtr<SDL_AtomicInt> atomic,
        int oldValue,
        int newValue
    )
    {
        return SDL_CompareAndSwapAtomicInt(atomic, oldValue, newValue);
    }

    public static unsafe bool CompareAndSwap(
        this IntPtr<IntPtr> ptr,
        IntPtr oldValue,
        IntPtr newValue
    )
    {
        return SDL_CompareAndSwapAtomicPointer(ptr, oldValue, newValue);
    }

    public static unsafe bool CompareAndSwap(
        this IntPtr<SDL_AtomicU32> atomic,
        uint oldValue,
        uint newValue
    )
    {
        return SDL_CompareAndSwapAtomicU32(atomic, oldValue, newValue);
    }

    public static unsafe int GetValue(
        this IntPtr<SDL_AtomicInt> atomic
    )
    {
        return SDL_GetAtomicInt(atomic);
    }

    public static unsafe IntPtr GetValue(
        this IntPtr<IntPtr> atomic
    )
    {
        return SDL_GetAtomicPointer(atomic);
    }

    public static unsafe uint GetValue(
        this IntPtr<SDL_AtomicU32> atomic
    )
    {
        return SDL_GetAtomicU32(atomic);
    }
    
    public static unsafe int SetValue(
        this IntPtr<SDL_AtomicInt> atomic,
        int value
    )
    {
        return SDL_SetAtomicInt(atomic, value);
    }

    public static unsafe IntPtr SetValue(
        this IntPtr<IntPtr> atomic,
        IntPtr value
    )
    {
        return SDL_SetAtomicPointer(atomic, value);
    }

    public static unsafe uint SetValue(
        this IntPtr<SDL_AtomicU32> atomic,
        uint value
    )
    {
        return SDL_SetAtomicU32(atomic, value);
    }
}