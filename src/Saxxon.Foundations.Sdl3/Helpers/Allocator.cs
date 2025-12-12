using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Saxxon.Foundations.Sdl3.Helpers;

internal class Allocator
{
    /// <summary>
    /// Tracks SDL-initiated memory allocations.
    /// </summary>
    private static readonly ConcurrentDictionary<IntPtr, (GCHandle Handle, int[] Array)> Allocations = [];

    /// <summary>
    /// Total number of native words allocated.
    /// </summary>
    private static nuint _allocatedWords;

    /// <summary>
    /// Allocates a pinned block of memory.
    /// </summary>
    /// <param name="size">
    /// Number of bytes to allocate.
    /// </param>
    /// <param name="initialize">
    /// True to initialize the memory to zero.
    /// </param>
    /// <returns>
    /// A pointer to the allocated memory.
    /// </returns>
    public static unsafe IntPtr AllocInternal(nuint size, bool initialize)
    {
        if (size == 0)
            return 0;

        // Determine how much memory we will need.

        var count = (int)((size + 3) / 4);
        var mem = ArrayPool<int>.Shared.Rent(count);
        var handle = GCHandle.Alloc(mem, GCHandleType.Pinned);

        if (initialize)
            mem.AsSpan().Clear();

        _allocatedWords += (UIntPtr)mem.Length;

        fixed (int* memPtr = mem)
        {
            Allocations.TryAdd((IntPtr)memPtr, (handle, mem));
            return (IntPtr)memPtr;
        }
    }

    public static void Free(IntPtr mem)
    {
        if (!Allocations.Remove(mem, out var alloc))
            return;

        alloc.Handle.Free();
        _allocatedWords -= (UIntPtr)alloc.Array.Length;
    }

    public static IntPtr Malloc(nuint size) =>
        AllocInternal(size, false);

    public static IntPtr Calloc(nuint count, nuint elementSize) =>
        AllocInternal(count * elementSize, true);

    public static unsafe IntPtr Realloc(IntPtr mem, UIntPtr newSize)
    {
        // Shortcut: if it isn't memory we allocated, act like Malloc.

        if (!Allocations.Remove(mem, out var alloc))
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
            Allocations.TryAdd(mem, (handle, alloc.Array));
            _allocatedWords += (UIntPtr)alloc.Array.Length;
            return mem;
        }

        var newMem = AllocInternal(newSize, false);

        var copyCount = Math.Min(newCount, alloc.Array.Length);
        new Span<int>((void*)mem, copyCount)
            .CopyTo(new Span<int>((void*)newMem, copyCount));

        return newMem;
    }

    public static long Total =>
        unchecked((long)(_allocatedWords * 4));
}