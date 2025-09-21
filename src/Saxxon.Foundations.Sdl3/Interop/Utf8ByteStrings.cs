using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Represents an unmanaged list of string values, where the values are from a managed source.
/// </summary>
[PublicAPI]
[MustDisposeResource]
internal sealed unsafe class Utf8ByteStrings : IDisposable
{
    /// <summary>
    /// Pointer to the list of string pointers representing each value.
    /// </summary>
    public IntPtr Ptr { get; }

    /// <summary>
    /// Number of strings in the list.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Creates a list of string values in unmanaged memory.
    /// </summary>
    /// <param name="values"></param>
    public Utf8ByteStrings(params ReadOnlySpan<string> values)
    {
        Span<int> byteSizes = stackalloc int[values.Length];

        var count = values.Length;
        var tableSize = (count + 1) * IntPtr.Size;
        var dataSize = 1;

        for (var i = 0; i < count; i++)
        {
            var size = Encoding.UTF8.GetByteCount(values[i]);
            byteSizes[i] = size;
            dataSize += size + 1;
        }

        var totalSize = tableSize + dataSize;
        Ptr = Marshal.AllocCoTaskMem(totalSize);
        var dataBlock = new Span<byte>((void*)(Ptr + tableSize), dataSize);
        var tableBlock = new Span<IntPtr>((void*)Ptr, values.Length);
        dataBlock.Clear();
        tableBlock.Clear();

        for (var i = 0; i < count; i++)
        {
            var byteSize = byteSizes[i];

            fixed (byte* dataPtr = dataBlock)
            fixed (char* s = values[i])
            {
                tableBlock[i] = (IntPtr)dataPtr;
                Encoding.UTF8.GetBytes(
                    s,
                    values[i].Length,
                    dataPtr,
                    byteSize
                );
            }

            dataBlock = dataBlock[byteSize..];
        }

        Count = count;
    }

    /// <summary>
    /// Gets the pointer table as a span.
    /// </summary>
    public ReadOnlySpan<IntPtr> AsSpan() =>
        new((void*)Ptr, Count);

    /// <inheritdoc />
    public void Dispose()
    {
        Marshal.FreeCoTaskMem(Ptr);
    }
}