using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Represents a pointer to an unmanaged structure.
/// </summary>
[PublicAPI]
public readonly record struct IntPtr<T>(IntPtr Ptr)
    : IEquatable<IntPtr>, IEquatable<IntPtr?>
    where T : unmanaged
{
    /// <summary>
    /// Converts a reference to a structure to a pointer.
    /// </summary>
    public IntPtr(scoped ref T target) 
        : this(FromRef(ref target))
    {
    }

    /// <summary>
    /// Converts this structure to an <see cref="IntPtr"/>.
    /// </summary>
    public static implicit operator IntPtr(IntPtr<T> other) =>
        other.Ptr;

    /// <summary>
    /// Converts an <see cref="IntPtr"/> to a typed pointer.
    /// </summary>
    public static implicit operator IntPtr<T>(IntPtr other) =>
        new(other);

    /// <summary>
    /// Converts a typed pointer to a raw pointer for use in interop.
    /// </summary>
    public static unsafe implicit operator T*(IntPtr<T> other) =>
        (T*)other.Ptr;

    /// <summary>
    /// Converts a typed pointer to a raw void pointer for use in interop.
    /// </summary>
    public static unsafe implicit operator void*(IntPtr<T> other) =>
        (void*)other.Ptr;

    /// <summary>
    /// Converts a raw pointer to a typed pointer.
    /// </summary>
    public static unsafe implicit operator IntPtr<T>(T* other) =>
        new((IntPtr)other);

    /// <summary>
    /// Converts a raw void pointer to a typed pointer.
    /// </summary>
    public static unsafe explicit operator IntPtr<T>(void* other) =>
        new((IntPtr)other);

    /// <summary>
    /// Compares equality between pointer values.
    /// </summary>
    public static bool operator ==(IntPtr<T> left, IntPtr right) =>
        left.Ptr == right;

    /// <summary>
    /// Compares inequality between pointer values.
    /// </summary>
    public static bool operator !=(IntPtr<T> left, IntPtr right) =>
        left.Ptr != right;

    /// <summary>
    /// Compares equality between pointer values.
    /// </summary>
    public static bool operator ==(IntPtr left, IntPtr<T> right) =>
        left == right.Ptr;

    /// <summary>
    /// Compares inequality between pointer values.
    /// </summary>
    public static bool operator !=(IntPtr left, IntPtr<T> right) =>
        left != right.Ptr;

    /// <summary>
    /// A typed null pointer.
    /// </summary>
    public static IntPtr<T> Zero =>
        default;

    /// <summary>
    /// Wraps a <see cref="Span{T}"/> around memory pointed to.
    /// </summary>
    /// <param name="length">
    /// Number of elements in the span.
    /// </param>
    public unsafe Span<T> AsSpan(int length) =>
        new((T*)Ptr, length);

    /// <summary>
    /// Wraps a <see cref="ReadOnlySpan{T}"/> around memory pointed to.
    /// </summary>
    /// <param name="length">
    /// Number of elements in the span.
    /// </param>
    public unsafe ReadOnlySpan<T> AsReadOnlySpan(int length) =>
        new((T*)Ptr, length);

    /// <summary>
    /// Converts a reference to a structure into a typed pointer.
    /// </summary>
    public static unsafe IntPtr<T> FromRef(scoped ref T target) =>
        (IntPtr<T>)Unsafe.AsPointer(ref target);

    /// <summary>
    /// Converts the typed pointer into a structure reference.
    /// </summary>
    public unsafe ref T AsRef() =>
        ref Unsafe.AsRef<T>((void*)Ptr);

    /// <summary>
    /// Converts the typed pointer into a read only structure reference.
    /// </summary>
    public unsafe ref readonly T AsReadOnlyRef() =>
        ref Unsafe.AsRef<T>((void*)Ptr);

    /// <summary>
    /// Returns true if the value represents a null pointer.
    /// </summary>
    public bool IsNull =>
        Ptr == IntPtr.Zero;

    /// <summary>
    /// Dereference the pointed data at the specified index.
    /// </summary>
    public unsafe T this[int index]
    {
        get => ((T*)Ptr)[index];
        set => ((T*)Ptr)[index] = value;
    }

    /// <inheritdoc />
    public bool Equals(IntPtr other) =>
        Ptr == other;

    /// <inheritdoc />
    public bool Equals(IntPtr? other) =>
        Ptr == other;

    /// <inheritdoc />
    public override int GetHashCode() =>
        Ptr.GetHashCode();
}