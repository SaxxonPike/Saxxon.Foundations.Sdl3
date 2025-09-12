using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3.Interop;

[PublicAPI]
internal readonly ref struct Utf8SpanArray
{
    private readonly ReadOnlySpan<IntPtr> _pointers;

    public int Count => _pointers.Length;

    public Utf8SpanArray(ReadOnlySpan<IntPtr> pointers)
    {
        _pointers = pointers;
    }

    public Utf8SpanArray(IntPtr<IntPtr> pointers)
    {
        var length = 0;
        while (pointers[length] != IntPtr.Zero)
            length++;

        _pointers = pointers.AsReadOnlySpan(length);
    }
    
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
            result[i] = _pointers[i].GetString();
        return result;
    }

    public List<string> ToList()
    {
        var result = new List<string>(Count);
        for (var i = 0; i < Count; i++)
            result.Add(_pointers[i].GetString()!);
        return result;
    }
}