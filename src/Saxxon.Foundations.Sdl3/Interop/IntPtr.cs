using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Represents a pointer to an unmanaged structure.
/// </summary>
[PublicAPI]
public readonly record struct IntPtr<T>(IntPtr Ptr)
    where T : unmanaged
{
    public IntPtr(scoped ref T target) : this(FromRef(ref target))
    {
    }

    public static implicit operator IntPtr(IntPtr<T> other)
    {
        return other.Ptr;
    }

    public static implicit operator IntPtr<T>(IntPtr other)
    {
        return new IntPtr<T>(other);
    }

    public static unsafe implicit operator T*(IntPtr<T> other)
    {
        return (T*)other.Ptr;
    }

    public static unsafe implicit operator void*(IntPtr<T> other)
    {
        return (void*)other.Ptr;
    }

    public static unsafe implicit operator IntPtr<T>(T* other)
    {
        return new IntPtr<T>((IntPtr)other);
    }

    public static unsafe explicit operator IntPtr<T>(void* other)
    {
        return new IntPtr<T>((IntPtr)other);
    }

    public static bool operator ==(IntPtr<T> left, IntPtr right)
    {
        return left.Ptr == right;
    }

    public static bool operator !=(IntPtr<T> left, IntPtr right)
    {
        return left.Ptr != right;
    }

    public static bool operator ==(IntPtr left, IntPtr<T> right)
    {
        return left == right.Ptr;
    }

    public static bool operator !=(IntPtr left, IntPtr<T> right)
    {
        return left != right.Ptr;
    }

    public static IntPtr<T> Zero => default;

    public unsafe Span<T> AsSpan(int length)
    {
        return new Span<T>((T*)Ptr, length);
    }

    public unsafe ReadOnlySpan<T> AsReadOnlySpan(int length)
    {
        return new ReadOnlySpan<T>((T*)Ptr, length);
    }

    public static unsafe IntPtr<T> FromRef(scoped ref T target)
    {
        return (IntPtr<T>)Unsafe.AsPointer(ref target);
    }

    public unsafe ref T AsRef()
    {
        return ref Unsafe.AsRef<T>((void*)Ptr);
    }

    public unsafe ref readonly T AsReadOnlyRef()
    {
        return ref Unsafe.AsRef<T>((void*)Ptr);
    }

    public bool IsNull => Ptr == IntPtr.Zero;

    public unsafe T this[int index]
    {
        get => *(T*)Ptr;
        set => *(T*)Ptr = value;
    }
}