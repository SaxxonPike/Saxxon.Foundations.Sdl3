using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Interop;

[PublicAPI]
internal readonly ref struct Utf8SpanArray(ReadOnlySpan<IntPtr> pointers)
{
    private readonly ReadOnlySpan<IntPtr> _pointers = pointers;

    public int Count => _pointers.Length;

    public unsafe ReadOnlySpan<byte> this[int index]
    {
        get
        {
            var ptr = _pointers[index];
            return ptr == IntPtr.Zero
                ? ReadOnlySpan<byte>.Empty
                : new ReadOnlySpan<byte>((void*)ptr, (int)SDL_strlen((byte*)ptr));
        }
    }

    public string?[] ToArray()
    {
        var result = new string?[Count];
        for (var i = 0; i < Count; i++)
            result[i] = Marshal.PtrToStringUTF8(_pointers[i]);
        return result;
    }

    public List<string> ToList()
    {
        var result = new List<string>(Count);
        for (var i = 0; i < Count; i++)
            result.Add(Marshal.PtrToStringUTF8(_pointers[i])!);
        return result;
    }
}