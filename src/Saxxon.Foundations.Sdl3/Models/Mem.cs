using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

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

    private static ConcurrentDictionary<IntPtr, int[]> _allocations = [];

    private static nuint _allocatedWords;

    public static unsafe void SetDotNetFunctions()
    {
        SDL_SetMemoryFunctions(&Malloc, &Calloc, &Realloc, &Free)
            .AssertSdlSuccess();

        return;

        static IntPtr AllocInternal(nuint size, bool initialize)
        {
            if (size == 0)
                return 0;
            
            // Determine how much memory we will need.

            var count = (int)((size + 3) / 4);
            var mem = initialize
                ? GC.AllocateArray<int>(count, true)
                : GC.AllocateUninitializedArray<int>(count, true);

            _allocatedWords += (UIntPtr)count;

            fixed (int* memPtr = mem)
            {
                _allocations.TryAdd((IntPtr)memPtr, mem);
                return (IntPtr)memPtr;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static IntPtr Malloc(nuint size)
        {
            return AllocInternal(size, false);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static IntPtr Calloc(nuint count, nuint elementSize)
        {
            return AllocInternal(count * elementSize, true);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static IntPtr Realloc(IntPtr mem, UIntPtr newSize)
        {
            // Shortcut: if it isn't memory we allocated, act like Malloc.

            if (!_allocations.Remove(mem, out var array))
                return AllocInternal(newSize, false);
            _allocatedWords -= (UIntPtr)array.Length;

            // Shortcut: if the new size is zero, don't allocate anything new
            // (but freeing the old pointer is fine.)

            if (newSize == 0)
                return 0;

            // Shortcut: if the new size matches the existing size, there
            // is no need to allocate anything.

            var newCount = (int)((newSize + 3) / 4);
            if (array.Length == newCount)
            {
                _allocations.TryAdd(mem, array);
                _allocatedWords += (UIntPtr)array.Length;
                return mem;
            }

            var newMem = AllocInternal(newSize, false);

            var copyCount = Math.Min(newCount, array.Length);
            new Span<int>((void*)mem, copyCount)
                .CopyTo(new Span<int>((void*)newMem, copyCount));

            return newMem;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static void Free(IntPtr mem)
        {
            if (_allocations.Remove(mem, out var array))
                _allocatedWords -= (UIntPtr)array.Length;
        }
    }

    public static long GetTotalAllocated()
    {
        return unchecked((long)(_allocatedWords * 4));
    }
}