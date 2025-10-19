using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
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
    /// Sets SDL's memory allocation functions.
    /// </summary>
    /// <param name="malloc">
    /// Function to use in place of <see cref="SDL_malloc"/>.
    /// </param>
    /// <param name="calloc">
    /// Function to use in place of <see cref="SDL_calloc"/>.
    /// </param>
    /// <param name="realloc">
    /// Function to use in place of <see cref="SDL_realloc"/>.
    /// </param>
    /// <param name="free">
    /// Function to use in place of <see cref="SDL_free(IntPtr)"/>.
    /// </param>
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
    /// Restores SDL's built-in memory allocation functions.
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
    /// Tracks allocations made using the .NET functions.
    /// </summary>
    private static ConcurrentDictionary<IntPtr, (GCHandle Handle, int[] Array)> _allocations = [];

    /// <summary>
    /// Amount of memory allocated, measured in native ints.
    /// </summary>
    private static nuint _allocatedWords;

    /// <summary>
    /// Size of a native int, in bytes. This will differ between architectures and is used for size alignment.
    /// </summary>
    private static unsafe nuint _sizeOfNint = (nuint)sizeof(nint);

    /// <summary>
    /// Allocates memory using .NET.
    /// </summary>
    /// <param name="size">
    /// Number of bytes to allocate.
    /// </param>
    /// <param name="initialize">
    /// If true, the allocated memory is zeroed.
    /// </param>
    /// <returns>
    /// A pointer to the allocated memory.
    /// </returns>
    internal static unsafe IntPtr AllocInternal(nuint size, bool initialize)
    {
        if (size == 0)
            return 0;

        // Determine how much memory we will need.

        var count = (int)((size + _sizeOfNint - 1) / _sizeOfNint);
        var mem = ArrayPool<int>.Shared.Rent(count);
        var handle = GCHandle.Alloc(mem, GCHandleType.Pinned);

        if (initialize)
            mem.AsSpan().Clear();

        _allocatedWords += (UIntPtr)mem.Length;

        fixed (int* memPtr = mem)
        {
            _allocations.TryAdd((IntPtr)memPtr, (handle, mem));
            return (IntPtr)memPtr;
        }
    }

    /// <summary>
    /// .NET implementation of <see cref="SDL_free(IntPtr)"/>.
    /// </summary>
    internal static void FreeInternal(IntPtr mem)
    {
        if (!_allocations.Remove(mem, out var alloc))
            return;

        alloc.Handle.Free();
        _allocatedWords -= (UIntPtr)alloc.Array.Length;
    }

    /// <summary>
    /// .NET implementation of <see cref="SDL_malloc"/>.
    /// </summary>
    internal static IntPtr MallocInternal(nuint size) =>
        AllocInternal(size, false);

    /// <summary>
    /// .NET implementation of <see cref="SDL_calloc"/>.
    /// </summary>
    internal static IntPtr CallocInternal(nuint count, nuint elementSize) =>
        AllocInternal(count * elementSize, true);

    /// <summary>
    /// .NET implementation of <see cref="SDL_realloc"/>.
    /// </summary>
    internal static unsafe IntPtr ReallocInternal(IntPtr mem, UIntPtr newSize)
    {
        // Shortcut: if it isn't memory we allocated, act like Malloc.

        if (!_allocations.Remove(mem, out var alloc))
            return AllocInternal(newSize, false);

        alloc.Handle.Free();
        _allocatedWords -= (UIntPtr)alloc.Array.Length;

        // Shortcut: if the new size is zero, don't allocate anything new
        // (but freeing the old pointer is fine.)

        if (newSize == 0)
            return 0;

        // Shortcut: if the new size matches the existing size, there
        // is no need to allocate anything.

        var newCount = (int)((newSize + 3) / 4);
        if (alloc.Array.Length == newCount)
        {
            var handle = GCHandle.Alloc(alloc.Array, GCHandleType.Pinned);
            _allocations.TryAdd(mem, (handle, alloc.Array));
            _allocatedWords += (UIntPtr)alloc.Array.Length;
            return mem;
        }

        var newMem = AllocInternal(newSize, false);

        var copyCount = Math.Min(newCount, alloc.Array.Length);
        new Span<int>((void*)mem, copyCount)
            .CopyTo(new Span<int>((void*)newMem, copyCount));

        return newMem;
    }

    /// <summary>
    /// Configures memory allocation functions within SDL so that the memory is allocated within .NET and
    /// managed by the .NET garbage collector.
    /// </summary>
    public static unsafe void SetDotNetFunctions()
    {
        SDL_SetMemoryFunctions(&SdlMalloc, &SdlCalloc, &SdlRealloc, &SdlFree)
            .AssertSdlSuccess();

        return;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static IntPtr SdlMalloc(nuint size) =>
            MallocInternal(size);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static IntPtr SdlCalloc(nuint count, nuint elementSize) =>
            CallocInternal(count, elementSize);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static IntPtr SdlRealloc(IntPtr mem, UIntPtr newSize) =>
            ReallocInternal(mem, newSize);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static void SdlFree(IntPtr mem) =>
            FreeInternal(mem);
    }

    /// <summary>
    /// Returns the amount of memory allocated while using .NET memory allocation functions.
    /// </summary>
    public static long GetTotalAllocated()
    {
        return unchecked((long)(_allocatedWords * 4));
    }
}