using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Helpers;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Memory functions, including structure size measurement and
/// SDL malloc/free.
/// </summary>
[PublicAPI]
public static class Mem
{
    /// <summary>
    /// Calculates the size of struct T. This will work for
    /// generic types, whereas <see cref="Marshal.SizeOf{T}()"/> will not.
    /// </summary>
    public static unsafe int SizeOf<T>() where T : unmanaged
    {
        return (int)(IntPtr)(&((T*)0)[1]);
    }

    /// <summary>
    /// Allocates a block of memory.
    /// </summary>
    /// <param name="size">
    /// Number of bytes to allocate.
    /// </param>
    /// <returns>
    /// A pointer to the allocated memory.
    /// </returns>
    public static IntPtr Alloc(int size)
    {
        return SDL_malloc((UIntPtr)size)
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Allocates memory for a structure.
    /// </summary>
    /// <typeparam name="T">
    /// Type of structure to allocate memory for.
    /// </typeparam>
    /// <returns>
    /// A pointer to the allocated structure.
    /// </returns>
    public static IntPtr<T> Alloc<T>() where T : unmanaged
    {
        return SDL_malloc((UIntPtr)SizeOf<T>())
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Allocates memory for a span of structures.
    /// </summary>
    /// <param name="length">
    /// Number of structures to allocate memory for.
    /// </param>
    /// <typeparam name="T">
    /// Type of structure to allocate memory for.
    /// </typeparam>
    /// <returns>
    /// A pointer to the allocated span of structures.
    /// </returns>
    public static IntPtr<T> Alloc<T>(int length) where T : unmanaged
    {
        return SDL_malloc((UIntPtr)(length * SizeOf<T>()))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Clears memory to zero.
    /// </summary>
    /// <param name="ptr">
    /// Pointer to the memory to clear.
    /// </param>
    /// <param name="size">
    /// Number of bytes to clear.
    /// </param>
    public static void Clear(IntPtr ptr, int size)
    {
        SDL_memset(ptr, 0, (UIntPtr)size)
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Copies a structure to newly allocated memory.
    /// </summary>
    /// <param name="item">
    /// Structure to copy.
    /// </param>
    /// <typeparam name="T">
    /// Type of structure to copy.
    /// </typeparam>
    /// <returns>
    /// A pointer to the copied structure.
    /// </returns>
    public static IntPtr<T> Copy<T>(T item) where T : unmanaged
    {
        var result = Alloc<T>();
        result.AsRef() = item;
        return result;
    }

    /// <summary>
    /// Copies a span of structures to newly allocated memory.
    /// </summary>
    /// <param name="item">
    /// Span of structures to copy.
    /// </param>
    /// <typeparam name="T">
    /// Type of structure to copy.
    /// </typeparam>
    /// <returns>
    /// A pointer to the copied span of structures.
    /// </returns>
    public static IntPtr<T> Copy<T>(ReadOnlySpan<T> item) where T : unmanaged
    {
        var result = Alloc<T>(item.Length);
        item.CopyTo(result.AsSpan(item.Length));
        return result;
    }

    /// <summary>
    /// Frees allocated memory within SDL.
    /// </summary>
    /// <param name="ptr">
    /// Pointer to the memory to free.
    /// </param>
    public static void Free<T>(
        IntPtr<T> ptr
    ) where T : unmanaged
    {
        SDL_free(ptr.Ptr);
    }

    /// <summary>
    /// Replace SDL's memory allocation functions with a custom set.
    /// </summary>
    /// <param name="malloc">
    /// Custom <see cref="SDL_malloc"/> function.
    /// </param>
    /// <param name="calloc">
    /// Custom <see cref="SDL_calloc"/> function.
    /// </param>
    /// <param name="realloc">
    /// Custom <see cref="SDL_realloc"/> function.
    /// </param>
    /// <param name="free">
    /// Custom <see cref="SDL_free(IntPtr)"/> function.
    /// </param>
    /// <remarks>
    /// It is not safe to call this function once any allocations have been made, as future
    /// calls to <see cref="SDL_free(IntPtr)"/> will use the new allocator, even if they came from
    /// an <see cref="SDL_malloc"/> made with the old one! If used, usually this needs to be the first call made
    /// into the SDL library, if not the very first thing done at program startup time.
    /// </remarks>
    public static unsafe void SetFunctions(
        Func<UIntPtr, IntPtr> malloc,
        Func<UIntPtr, UIntPtr, IntPtr> calloc,
        Func<IntPtr, UIntPtr, IntPtr> realloc,
        Action<IntPtr> free
    )
    {
        var mallocPtr = (delegate* unmanaged[Cdecl]<UIntPtr, IntPtr>)
            Marshal.GetFunctionPointerForDelegate(malloc);

        var callocPtr = (delegate* unmanaged[Cdecl]<UIntPtr, UIntPtr, IntPtr>)
            Marshal.GetFunctionPointerForDelegate(calloc);

        var reallocPtr = (delegate* unmanaged[Cdecl]<IntPtr, UIntPtr, IntPtr>)
            Marshal.GetFunctionPointerForDelegate(realloc);

        var freePtr = (delegate* unmanaged[Cdecl]<IntPtr, void>)
            Marshal.GetFunctionPointerForDelegate(free);

        SDL_SetMemoryFunctions(mallocPtr, callocPtr, reallocPtr, freePtr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Restore SDL's memory allocation functions to the built-in set.
    /// </summary>
    public static unsafe void SetOriginalFunctions()
    {
        delegate* unmanaged[Cdecl]<UIntPtr, IntPtr> malloc;
        delegate* unmanaged[Cdecl]<UIntPtr, UIntPtr, IntPtr> calloc;
        delegate* unmanaged[Cdecl]<IntPtr, UIntPtr, IntPtr> realloc;
        delegate* unmanaged[Cdecl]<IntPtr, void> free;

        SDL_GetOriginalMemoryFunctions(
            &malloc,
            &calloc,
            &realloc,
            &free
        );

        SDL_SetMemoryFunctions(
            malloc,
            calloc,
            realloc,
            free
        );
    }

    /// <summary>
    /// Replace SDL's memory allocation functions with ones that use the
    /// .NET allocator.
    /// </summary>
    public static unsafe void SetDotNetFunctions()
    {
        SDL_SetMemoryFunctions(&SdlMalloc, &SdlCalloc, &SdlRealloc, &SdlFree)
            .AssertSdlSuccess();

        return;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static IntPtr SdlMalloc(nuint size) =>
            Allocator.Malloc(size);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static IntPtr SdlCalloc(nuint count, nuint elementSize) =>
            Allocator.Calloc(count, elementSize);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static IntPtr SdlRealloc(IntPtr mem, UIntPtr newSize) =>
            Allocator.Realloc(mem, newSize);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static void SdlFree(IntPtr mem) =>
            Allocator.Free(mem);
    }

    /// <summary>
    /// Determines the total amount of memory allocated.
    /// </summary>
    /// <returns>
    /// The number of bytes.
    /// </returns>
    /// <remarks>
    /// This only tracks memory allocated during the time which SDL is using
    /// the .NET allocator.
    /// </remarks>
    public static long GetTotalAllocated() =>
        Allocator.Total;
}