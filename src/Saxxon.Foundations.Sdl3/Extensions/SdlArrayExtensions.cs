using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Extensions;

[PublicAPI]
internal static class SdlArrayExtensions
{
    public static List<T> ToList<T>(this SDLArray<T>? array)
        where T : unmanaged
    {
        if (array == null)
            return [];

        var result = new List<T>(array.Count);

        foreach (var t in array)
            result.Add(t);

        return result;
    }

    public static List<string?> ToUtf8StringList(this SDLArray<IntPtr>? array)
    {
        if (array == null)
            return [];

        var result = new List<string?>(array.Count);

        foreach (var t in array)
            result.Add(Marshal.PtrToStringUTF8(t));

        return result;
    }

    public static unsafe List<IntPtr<T>> ToList<T>(this SDLConstOpaquePointerArray<T>? array)
        where T : unmanaged
    {
        if (array == null)
            return [];

        var result = new List<IntPtr<T>>(array.Count);

        foreach (var t in array)
            result.Add(t != null ? t : default(IntPtr<T>));

        return result;
    }

    public static unsafe List<IntPtr<T>> ToList<T>(this SDLOpaquePointerArray<T>? array)
        where T : unmanaged
    {
        if (array == null)
            return [];

        var result = new List<IntPtr<T>>(array.Count);
        foreach (var t in array)
            result.Add(t != null ? t : default(IntPtr<T>));

        return result;
    }
}