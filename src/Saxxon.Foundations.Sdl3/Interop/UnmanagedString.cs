using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Fast UTF8 string interop functions, including formatting.
/// </summary>
[PublicAPI]
public readonly ref struct UnmanagedString : IDisposable
{
    private static readonly ObjectPool<StringBuilder> StringBuilderPool =
        new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());

    private readonly byte[] _gcRef;

    /// <summary>
    /// Retrieves the Utf8Span as a Utf8String for use with SDL3-CS.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static unsafe implicit operator Utf8String(UnmanagedString value)
    {
        return new ReadOnlySpan<byte>(value.Ptr, (int)SDL_strlen(value.Ptr));
    }

    public static unsafe implicit operator ReadOnlySpan<byte>(UnmanagedString value)
    {
        return new ReadOnlySpan<byte>(value.Ptr, (int)SDL_strlen(value.Ptr));
    }

    public static unsafe implicit operator Span<byte>(UnmanagedString value)
    {
        return new Span<byte>(value.Ptr, (int)SDL_strlen(value.Ptr));
    }

    // /// <summary>
    // /// Retrieves the Utf8Span as a byte pointer.
    // /// </summary>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static unsafe implicit operator byte*(Utf8Span value) => value.Ptr;

    /// <summary>
    /// Retrieves the unmanaged address where the UTF8 bytes reside.
    /// </summary>
    public IntPtr Address { get; }

    /// <summary>
    /// Retrieves <see cref="Address"/> as a byte pointer.
    /// </summary>
    public unsafe byte* Ptr => (byte*)Address;

    /// <summary>
    /// Allocates a memory buffer and converts the given character span into
    /// UTF8 bytes. A null terminator is added if it is absent.
    /// </summary>
    private static unsafe (IntPtr, byte[]) Alloc(ReadOnlySpan<char> chars)
    {
        var cLen = chars.Length;
        var bMaxLen = Encoding.UTF8.GetMaxByteCount(cLen) + (chars.EndsWith('\0') ? 0 : 1);
        var gcRef = GC.AllocateUninitializedArray<byte>(bMaxLen, pinned: true);
        var ptr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(gcRef, 0);
        var bytes = new Span<byte>(ptr, bMaxLen);
        var byteCount = Encoding.UTF8.GetBytes(chars, bytes);
        bytes[byteCount] = 0;
        return ((IntPtr)ptr, gcRef);
    }

    /// <summary>
    /// Allocates a memory buffer for the UTF8 byte span. A null terminator is
    /// added if it is absent.
    /// </summary>
    private static unsafe (IntPtr, byte[]) Alloc(ReadOnlySpan<byte> bytes)
    {
        var byteCount = bytes.Length + (bytes.EndsWith((byte)0) ? 0 : 1);
        var gcRef = GC.AllocateUninitializedArray<byte>(byteCount, pinned: true);
        var ptr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(gcRef, 0);
        var buffer = new Span<byte>(ptr, byteCount);
        buffer.Clear();
        bytes.CopyTo(buffer);
        return ((IntPtr)ptr, gcRef);
    }

    /// <summary>
    /// Frees a previously allocated memory buffer.
    /// </summary>
    private static void Free(IntPtr ptr)
    {
        Marshal.FreeHGlobal(ptr);
    }

    /// <summary>
    /// Allocates a <see cref="UnmanagedString"/> from a format string and arguments.
    /// Use this instead of string.Format to avoid unnecessary allocations when
    /// constructing Utf8Span instances.
    /// </summary>
    [StringFormatMethod(nameof(format))]
    public static UnmanagedString Format(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        StringBuilder? sb = null;

        try
        {
            sb = StringBuilderPool.Get();
            sb.AppendFormat(format, args);
            return new UnmanagedString(sb);
        }
        finally
        {
            if (sb != null)
                StringBuilderPool.Return(sb);
        }
    }

    /// <summary>
    /// Allocates a <see cref="UnmanagedString"/> from a string.
    /// </summary>
    public UnmanagedString(string? value)
    {
        (Address, _gcRef) = Alloc(value);
    }

    /// <summary>
    /// Allocates a character span.
    /// </summary>
    public UnmanagedString(ReadOnlySpan<char> value)
    {
        (Address, _gcRef) = Alloc(value);
    }

    /// <summary>
    /// Allocates a <see cref="UnmanagedString"/> from a <see cref="StringBuilder"/>.
    /// The characters in the StringBuilder are copied.
    /// </summary>
    public UnmanagedString(StringBuilder value)
    {
        Span<char> chars = stackalloc char[value.Length];
        value.CopyTo(0, chars, chars.Length);
        (Address, _gcRef) = Alloc(chars);
    }

    /// <summary>
    /// Allocates a byte span.
    /// </summary>
    public UnmanagedString(ReadOnlySpan<byte> value)
    {
        (Address, _gcRef) = Alloc(value);
    }

    /// <summary>
    /// Allocates a byte span over a pointer, zero terminated.
    /// </summary>
    /// <param name="value"></param>
    public unsafe UnmanagedString(byte* value)
    {
        var span = new Span<byte>(value, (int)SDL_strlen(value));
        (Address, _gcRef) = Alloc(span);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Free(Address);
    }

    public override string? ToString()
    {
        return Address.GetString();
    }
}