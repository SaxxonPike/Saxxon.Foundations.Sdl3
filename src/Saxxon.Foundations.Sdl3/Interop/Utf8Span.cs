using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Fast UTF8 string interop functions, including formatting.
/// </summary>
[PublicAPI]
public readonly ref struct Utf8Span : IDisposable
{
    private static readonly ObjectPool<StringBuilder> StringBuilderPool =
        new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());

    /// <summary>
    /// Retrieves the Utf8Span as a Utf8String for use with SDL3-CS.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static unsafe implicit operator Utf8String(Utf8Span value)
    {
        return new ReadOnlySpan<byte>(value.Ptr, (int)SDL_utf8strlen(value.Ptr));
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
    private static unsafe IntPtr Alloc(ReadOnlySpan<char> chars)
    {
        var cLen = chars.Length;
        var bMaxLen = Encoding.UTF8.GetMaxByteCount(cLen) + (chars.EndsWith('\0') ? 0 : 1);
        var ptr = Marshal.AllocHGlobal(bMaxLen);
        var bytes = new Span<byte>((void*)ptr, bMaxLen);
        var byteCount = Encoding.UTF8.GetBytes(chars, bytes);
        bytes[byteCount] = 0;
        return ptr;
    }

    /// <summary>
    /// Allocates a memory buffer for the UTF8 byte span. A null terminator is
    /// added if it is absent.
    /// </summary>
    private static unsafe IntPtr Alloc(ReadOnlySpan<byte> bytes)
    {
        var byteCount = bytes.Length + (bytes.EndsWith((byte)0) ? 0 : 1);
        var ptr = Marshal.AllocHGlobal(byteCount);
        var buffer = new Span<byte>((void*)ptr, byteCount);
        buffer.Clear();
        bytes.CopyTo(buffer);
        return ptr;
    }

    /// <summary>
    /// Frees a previously allocated memory buffer.
    /// </summary>
    private static void Free(IntPtr ptr)
    {
        Marshal.FreeHGlobal(ptr);
    }

    /// <summary>
    /// Allocates a <see cref="Utf8Span"/> from a format string and arguments.
    /// Use this instead of string.Format to avoid unnecessary allocations when
    /// constructing Utf8Span instances.
    /// </summary>
    [StringFormatMethod(nameof(format))]
    public static Utf8Span Format(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        StringBuilder? sb = null;

        try
        {
            sb = StringBuilderPool.Get();
            sb.AppendFormat(format, args);
            return new Utf8Span(sb);
        }
        finally
        {
            if (sb != null)
                StringBuilderPool.Return(sb);
        }
    }

    /// <summary>
    /// Allocates a <see cref="Utf8Span"/> from a string.
    /// </summary>
    public Utf8Span(string? value)
    {
        Address = Alloc(value);
    }

    /// <summary>
    /// Allocates a character span.
    /// </summary>
    public Utf8Span(ReadOnlySpan<char> value)
    {
        Address = Alloc(value);
    }

    /// <summary>
    /// Allocates a <see cref="Utf8Span"/> from a <see cref="StringBuilder"/>.
    /// The characters in the StringBuilder are copied.
    /// </summary>
    public Utf8Span(StringBuilder value)
    {
        Span<char> chars = stackalloc char[value.Length];
        value.CopyTo(0, chars, chars.Length);
        Address = Alloc(chars);
    }

    /// <summary>
    /// Allocates a byte span.
    /// </summary>
    public Utf8Span(ReadOnlySpan<byte> value)
    {
        Address = Alloc(value);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Free(Address);
    }
}