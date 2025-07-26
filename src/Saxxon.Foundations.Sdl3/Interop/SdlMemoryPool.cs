using System.Buffers;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Manages memory within the SDL memory space.
/// </summary>
/// <typeparam name="T">
/// Type of memory to manage.
/// </typeparam>
[PublicAPI]
internal sealed unsafe class SdlMemoryPool<T> : MemoryPool<T>
    where T : unmanaged
{
    private readonly int _elementSize;
    private readonly HashSet<SdlMemoryManager<T>> _managers;

    /// <summary>
    /// Create a memory pool of type T.
    /// </summary>
    private SdlMemoryPool()
    {
        // Janky SizeOf<T>.
        _elementSize = (int)(IntPtr)(&((T*)0)[1]);

        MaxBufferSize = int.MaxValue / _elementSize;
        _managers = [];
    }

    /// <summary>
    /// Retrieve a shared memory pool of type T.
    /// </summary>
    public new static SdlMemoryPool<T> Shared =>
        (SdlMemoryPool<T>)SdlMemory.Pools.GetOrAdd(
            typeof(T),
            _ => new SdlMemoryPool<T>()
        );

    /// <summary>
    /// Free all owned memory in the SDL memory space.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        foreach (var manager in _managers)
            manager.Dispose();
    }

    /// <summary>
    /// Take ownership of memory in SDL memory space.
    /// </summary>
    /// <param name="ptr">
    /// Pointer to the memory block.
    /// </param>
    /// <param name="length">
    /// Number of elements in the memory block.
    /// </param>
    /// <returns>
    /// A handle to the memory block. Calling Dispose() on the
    /// returned object will call SDL_free().
    /// </returns>
    public IMemoryOwner<T> Own(void* ptr, int length)
    {
        var manager = new SdlMemoryManager<T>(
            this,
            ptr,
            length
        );
        _managers.Add(manager);
        return manager;
    }

    /// <summary>
    /// Allocates a block of memory in the SDL memory space.
    /// </summary>
    /// <param name="minBufferSize">
    /// Minimum number of elements to allocate. The default
    /// value of -1 will default to a total buffer size of
    /// 4096 bytes.
    /// </param>
    /// <returns>
    /// A handle to the memory block. Calling Dispose() on the
    /// returned object will call SDL_free().
    /// </returns>
    public override IMemoryOwner<T> Rent(int minBufferSize = -1)
    {
        if (minBufferSize < 0)
            minBufferSize = Math.Max(4096 / _elementSize, 16);

        var size = (UIntPtr)(_elementSize * minBufferSize);
        var ptr = SDL_malloc(size);
        var manager = new SdlMemoryManager<T>(
            this,
            (void*)ptr,
            minBufferSize
        );
        _managers.Add(manager);
        return manager;
    }

    /// <inheritdoc />
    public override int MaxBufferSize { get; }

    /// <summary>
    /// Calls SDL_free() on the memory block if it was either
    /// allocated or owned by this pool.
    /// </summary>
    /// <param name="manager"></param>
    public void Return(SdlMemoryManager<T> manager)
    {
        if (_managers.Remove(manager))
            SDL_free(manager.Ptr);
    }
}