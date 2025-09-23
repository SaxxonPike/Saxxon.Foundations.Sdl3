using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Pointer operations.
/// </summary>
[PublicAPI]
internal static class Ptr
{
    public static unsafe string? ToUtf8String(byte* ptr)
    {
        return ((IntPtr)ptr).GetString();
    }

    public static unsafe IntPtr<IntPtr<T>> FromArray<T>(T** array) where T : unmanaged
    {
        return (IntPtr<IntPtr<T>>)array;
    }

    public static unsafe ref T ToRef<T>(this IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
            return ref Unsafe.NullRef<T>();
        return ref Unsafe.AsRef<T>((void*)ptr);
    }

    public static unsafe Span<IntPtr<T>> AsNullTerminatedSpan<T>(this IntPtr<IntPtr<T>> ptr)
        where T : unmanaged
    {
        if (ptr.IsNull)
            return Span<IntPtr<T>>.Empty;

        var count = 0;
        while (!ptr[count].IsNull)
            count++;

        return new Span<IntPtr<T>>(ptr, count);
    }
}