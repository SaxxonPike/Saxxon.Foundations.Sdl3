using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Extensions;

/// <summary>
/// Extensions for <see cref="IntPtr"/> and <see cref="IntPtr{T}"/>.
/// </summary>
[PublicAPI]
public static class IntPtrExtensions
{
    /// <summary>
    /// Throws an exception if the value is a null pointer after an SDL operation.
    /// </summary>
    internal static IntPtr AssertSdlNotNull(this IntPtr value)
    {
        if (value == IntPtr.Zero)
            throw new SdlException();
        return value;
    }

    /// <summary>
    /// Throws an exception if the value is a null pointer after an SDL operation.
    /// </summary>
    internal static IntPtr<T> AssertSdlNotNull<T>(this IntPtr value) where T : unmanaged
    {
        return AssertSdlNotNull(value);
    }

    /// <summary>
    /// Throws an exception if the value is a null pointer after an SDL operation.
    /// </summary>
    internal static IntPtr<T> AssertSdlNotNull<T>(this IntPtr<T> value) where T : unmanaged
    {
        return AssertSdlNotNull((IntPtr)value);
    }

    /// <summary>
    /// Casts one type of <see cref="IntPtr{T}"/> to another.
    /// </summary>
    internal static IntPtr<T> As<T>(this IntPtr value) where T : unmanaged
    {
        return new IntPtr<T>(value);
    }

    /// <summary>
    /// Interprets data reference by a byte pointer as a null-terminated UTF8 encoded string.
    /// </summary>
    internal static string? GetString(this IntPtr value)
    {
        return Marshal.PtrToStringUTF8(value);
    }

    /// <summary>
    /// Interprets data reference by a byte pointer as a null-terminated UTF8 encoded string.
    /// </summary>
    internal static string GetString(this IntPtr value, int byteCount)
    {
        return Marshal.PtrToStringUTF8(value, byteCount);
    }

    /// <summary>
    /// Interprets data reference by a byte pointer as a null-terminated UTF8 encoded string.
    /// </summary>
    public static string? GetString(this IntPtr<byte> value)
    {
        return Marshal.PtrToStringUTF8(value);
    }

    /// <summary>
    /// Interprets data reference by a byte pointer as a null-terminated UTF8 encoded string.
    /// </summary>
    public static string GetString(this IntPtr<byte> value, int byteCount)
    {
        return Marshal.PtrToStringUTF8(value, byteCount);
    }

    /// <summary>
    /// Interprets data reference by a byte pointer as a null-terminated wchar_t encoded string.
    /// </summary>
    internal static unsafe string? GetWString(this IntPtr value)
    {
        // There is no cozy way to determine the size of wchar_t.
        // We are operating on the assumption that Windows will always use 2-byte, whereas all
        // other platforms will use 4-byte...

        var platform = ((IntPtr<byte>)Unsafe_SDL_GetPlatform()).GetUtf8Span();
        if (platform.StartsWith("Windows"u8))
        {
            return Marshal.PtrToStringUni(value);
        }

        var ptr = (int*)value;
        var length = 0;
        while (*ptr++ != 0)
            length++;
        return Encoding.UTF32.GetString((byte*)value, length * sizeof(int));
    }

    /// <summary>
    /// Interprets data reference by a byte pointer as a null-terminated UTF8 encoded string.
    /// </summary>
    public static unsafe ReadOnlySpan<byte> GetUtf8Span(this IntPtr value)
    {
        var ptr = (byte*)value;
        return new ReadOnlySpan<byte>(ptr, (int)SDL_strlen(ptr));
    }

    /// <summary>
    /// Interprets data reference by a byte pointer as a null-terminated UTF8 encoded string.
    /// </summary>
    public static unsafe ReadOnlySpan<byte> GetUtf8Span(this IntPtr<byte> value)
    {
        var ptr = (byte*)value.Ptr;
        return new ReadOnlySpan<byte>(ptr, (int)SDL_strlen(ptr));
    }
}