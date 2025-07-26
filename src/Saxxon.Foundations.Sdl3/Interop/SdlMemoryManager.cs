using System.Buffers;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Contains factories for <see cref="SdlMemoryManager{T}"/> instances.
/// </summary>
[PublicAPI]
internal static unsafe class SdlMemoryManager
{
    public static SdlMemoryManager<T> Const<T>(T* ptr, int length) where T : unmanaged
    {
        return new SdlMemoryManager<T>(null, ptr, length);
    }

    public static SdlMemoryManager<IntPtr<T>> Const<T>(T** ptr, int length) where T : unmanaged
    {
        return new SdlMemoryManager<IntPtr<T>>(null, ptr, length);
    }

    public static SdlMemoryManager<T> Owned<T>(T* ptr, int length) where T : unmanaged
    {
        return new SdlMemoryManager<T>(SdlMemoryPool<T>.Shared, ptr, length);
    }

    public static SdlMemoryManager<IntPtr<T>> Owned<T>(T** ptr, int length) where T : unmanaged
    {
        return new SdlMemoryManager<IntPtr<T>>(SdlMemoryPool<IntPtr<T>>.Shared, ptr, length);
    }
}

/// <summary>
/// Represents a block of memory in the SDL memory space.
/// </summary>
/// <param name="pool">
/// Pool that owns this memory block.
/// </param>
/// <param name="ptr">
/// Pointer to the start of the memory block.
/// </param>
/// <param name="length">
/// Number of elements in the memory block.
/// </param>
/// <typeparam name="T">
/// Type of elements.
/// </typeparam>
[PublicAPI]
internal sealed unsafe class SdlMemoryManager<T>(SdlMemoryPool<T>? pool, void* ptr, int length)
    : MemoryManager<T>, IDisposable where T : unmanaged
{
    /// <summary>
    /// Gets the pointer to the start of the memory block.
    /// </summary>
    public void* Ptr => ptr;

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        pool?.Return(this);
    }

    /// <inheritdoc />
    public override Span<T> GetSpan()
    {
        return new Span<T>(ptr, length);
    }

    /// <inheritdoc />
    public override MemoryHandle Pin(int elementIndex = 0)
    {
        if (elementIndex < 0 || elementIndex >= length)
            throw new ArgumentOutOfRangeException(nameof(elementIndex));

        var result = &((T*)ptr)[elementIndex];
        return new MemoryHandle(result, default, this);
    }

    /// <summary>
    /// Since allocated memory does not move within the SDL
    /// memory space, this method does nothing.
    /// </summary>
    public override void Unpin()
    {
    }
}