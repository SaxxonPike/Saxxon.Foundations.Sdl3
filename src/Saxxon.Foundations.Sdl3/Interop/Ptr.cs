using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Interop;

[PublicAPI]
public static class Ptr
{
    public static unsafe IntPtr<T> FromArray<T>(T* array) where T : unmanaged
    {
        return (IntPtr)array;
    }
    
    public static unsafe IntPtr<IntPtr<T>> FromArray<T>(T** array) where T : unmanaged
    {
        return (IntPtr)array;
    }

    public static unsafe ref T ToRef<T>(this IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
            return ref Unsafe.NullRef<T>();
        return ref Unsafe.AsRef<T>((void*)ptr);
    }
}