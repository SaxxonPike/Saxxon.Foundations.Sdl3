using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Interop;

[PublicAPI]
[MustDisposeResource]
internal sealed unsafe class Utf8ByteStrings : IDisposable
{
    public IntPtr Ptr { get; }

    public int Count { get; }

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

    public ReadOnlySpan<IntPtr> AsSpan() =>
        new((void*)Ptr, Count);

    public void Dispose()
    {
        Marshal.FreeCoTaskMem(Ptr);
    }
}