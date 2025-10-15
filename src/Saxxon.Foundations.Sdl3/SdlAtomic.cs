using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for atomic operations that deal with
/// <see cref="SDL_AtomicInt"/>, <see cref="SDL_AtomicU32"/> and pointers.
/// Prefer using C#'s built-in <see cref="Interlocked"/> API where possible.
/// </summary>
[PublicAPI]
public static class SdlAtomic
{
    /// <summary>
    /// Adds to an atomic variable.
    /// </summary>
    /// <param name="atomic">
    /// Variable to be modified.
    /// </param>
    /// <param name="value">
    /// Desired value to add.
    /// </param>
    /// <returns>
    /// Previous value of the atomic variable.
    /// </returns>
    public static unsafe int Add(
        this IntPtr<SDL_AtomicInt> atomic,
        int value
    )
    {
        return SDL_AddAtomicInt(atomic, value);
    }

    /// <summary>
    /// Adds to an atomic variable.
    /// </summary>
    /// <param name="atomic">
    /// Variable to be modified.
    /// </param>
    /// <param name="value">
    /// Desired value to add.
    /// </param>
    /// <returns>
    /// Previous value of the atomic variable.
    /// </returns>
    public static unsafe uint Add(
        this IntPtr<SDL_AtomicU32> atomic,
        int value
    )
    {
        return SDL_AddAtomicU32(atomic, value);
    }

    /// <summary>
    /// Sets an atomic variable to a new value if it is currently an old value.
    /// </summary>
    /// <param name="atomic">
    /// Atomic variable to modify.
    /// </param>
    /// <param name="oldValue">
    /// The value to compare prior to modification.
    /// </param>
    /// <param name="newValue">
    /// The new value.
    /// </param>
    /// <returns>
    /// True if the atomic variable was set, false otherwise.
    /// </returns>
    public static unsafe bool CompareAndSwap(
        this IntPtr<SDL_AtomicInt> atomic,
        int oldValue,
        int newValue
    )
    {
        return SDL_CompareAndSwapAtomicInt(atomic, oldValue, newValue);
    }

    /// <summary>
    /// Sets an atomic variable to a new value if it is currently an old value.
    /// </summary>
    /// <param name="atomic">
    /// Atomic variable to modify.
    /// </param>
    /// <param name="oldValue">
    /// The value to compare prior to modification.
    /// </param>
    /// <param name="newValue">
    /// The new value.
    /// </param>
    /// <returns>
    /// True if the atomic variable was set, false otherwise.
    /// </returns>
    public static unsafe bool CompareAndSwap(
        this IntPtr<IntPtr> atomic,
        IntPtr oldValue,
        IntPtr newValue
    )
    {
        return SDL_CompareAndSwapAtomicPointer(atomic, oldValue, newValue);
    }

    /// <summary>
    /// Sets an atomic variable to a new value if it is currently an old value.
    /// </summary>
    /// <param name="atomic">
    /// Atomic variable to modify.
    /// </param>
    /// <param name="oldValue">
    /// The value to compare prior to modification.
    /// </param>
    /// <param name="newValue">
    /// The new value.
    /// </param>
    /// <returns>
    /// True if the atomic variable was set, false otherwise.
    /// </returns>
    public static unsafe bool CompareAndSwap(
        this IntPtr<SDL_AtomicU32> atomic,
        uint oldValue,
        uint newValue
    )
    {
        return SDL_CompareAndSwapAtomicU32(atomic, oldValue, newValue);
    }

    /// <summary>
    /// Gets the value of an atomic variable.
    /// </summary>
    /// <param name="atomic">
    /// An atomic variable.
    /// </param>
    /// <returns>
    /// The current value of an atomic variable.
    /// </returns>
    public static unsafe int GetValue(
        this IntPtr<SDL_AtomicInt> atomic
    )
    {
        return SDL_GetAtomicInt(atomic);
    }

    /// <summary>
    /// Gets the value of an atomic variable.
    /// </summary>
    /// <param name="atomic">
    /// An atomic variable.
    /// </param>
    /// <returns>
    /// The current value of an atomic variable.
    /// </returns>
    public static unsafe IntPtr GetValue(
        this IntPtr<IntPtr> atomic
    )
    {
        return SDL_GetAtomicPointer(atomic);
    }

    /// <summary>
    /// Gets the value of an atomic variable.
    /// </summary>
    /// <param name="atomic">
    /// An atomic variable.
    /// </param>
    /// <returns>
    /// The current value of an atomic variable.
    /// </returns>
    public static unsafe uint GetValue(
        this IntPtr<SDL_AtomicU32> atomic
    )
    {
        return SDL_GetAtomicU32(atomic);
    }

    /// <summary>
    /// Sets an atomic variable to a value.
    /// </summary>
    /// <param name="atomic">
    /// An atomic variable to be modified.
    /// </param>
    /// <param name="value">
    /// The desired value.
    /// </param>
    /// <returns>
    /// The previous value of the atomic variable.
    /// </returns>
    public static unsafe int SetValue(
        this IntPtr<SDL_AtomicInt> atomic,
        int value
    )
    {
        return SDL_SetAtomicInt(atomic, value);
    }

    /// <summary>
    /// Sets an atomic variable to a value.
    /// </summary>
    /// <param name="atomic">
    /// An atomic variable to be modified.
    /// </param>
    /// <param name="value">
    /// The desired value.
    /// </param>
    /// <returns>
    /// The previous value of the atomic variable.
    /// </returns>
    public static unsafe IntPtr SetValue(
        this IntPtr<IntPtr> atomic,
        IntPtr value
    )
    {
        return SDL_SetAtomicPointer(atomic, value);
    }

    /// <summary>
    /// Sets an atomic variable to a value.
    /// </summary>
    /// <param name="atomic">
    /// An atomic variable to be modified.
    /// </param>
    /// <param name="value">
    /// The desired value.
    /// </param>
    /// <returns>
    /// The previous value of the atomic variable.
    /// </returns>
    public static unsafe uint SetValue(
        this IntPtr<SDL_AtomicU32> atomic,
        uint value
    )
    {
        return SDL_SetAtomicU32(atomic, value);
    }
}