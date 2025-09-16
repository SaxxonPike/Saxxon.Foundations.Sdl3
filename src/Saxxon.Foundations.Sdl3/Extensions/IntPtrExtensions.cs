using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Extensions;

[PublicAPI]
public static class IntPtrExtensions
{
    internal static IntPtr AssertSdlNotNull(this IntPtr value)
    {
        if (value == IntPtr.Zero)
            throw new SdlException();
        return value;
    }

    internal static IntPtr<T> AssertSdlNotNull<T>(this IntPtr value) where T : unmanaged
    {
        return AssertSdlNotNull(value);
    }

    internal static IntPtr<T> AssertSdlNotNull<T>(this IntPtr<T> value) where T : unmanaged
    {
        return AssertSdlNotNull((IntPtr)value);
    }

    internal static IntPtr<T> As<T>(this IntPtr value) where T : unmanaged
    {
        return new IntPtr<T>(value);
    }

    internal static string? GetString(this IntPtr value)
    {
        return Marshal.PtrToStringUTF8(value);
    }

    internal static string? GetString(this IntPtr value, int byteCount)
    {
        return Marshal.PtrToStringUTF8(value, byteCount);
    }

    public static string? GetString(this IntPtr<byte> value)
    {
        return Marshal.PtrToStringUTF8(value);
    }

    public static string? GetString(this IntPtr<byte> value, int byteCount)
    {
        return Marshal.PtrToStringUTF8(value, byteCount);
    }

    public static unsafe ReadOnlySpan<byte> GetUtf8Span(this IntPtr value)
    {
        var ptr = (byte*)value;
        return new ReadOnlySpan<byte>(ptr, (int)SDL_strlen(ptr));
    }

    public static unsafe ReadOnlySpan<byte> GetUtf8Span(this IntPtr<byte> value)
    {
        var ptr = (byte*)value.Ptr;
        return new ReadOnlySpan<byte>(ptr, (int)SDL_strlen(ptr));
    }
}