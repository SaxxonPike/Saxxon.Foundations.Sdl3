using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Extensions;

[PublicAPI]
internal static class IntPtrExtensions
{
    public static IntPtr AssertSdlNotNull(this IntPtr value)
    {
        if (value == IntPtr.Zero)
            throw new SdlException();
        return value;
    }

    public static IntPtr<T> AssertSdlNotNull<T>(this IntPtr value) where T : unmanaged
    {
        return AssertSdlNotNull(value);
    }

    public static IntPtr<T> AssertSdlNotNull<T>(this IntPtr<T> value) where T : unmanaged
    {
        return AssertSdlNotNull((IntPtr)value);
    }

    public static IntPtr<T> As<T>(this IntPtr value) where T : unmanaged
    {
        return new IntPtr<T>(value);
    }

    public static string? GetString(this IntPtr value)
    {
        return Marshal.PtrToStringUTF8(value);
    }

    public static string? GetString(this IntPtr value, int byteCount)
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
}