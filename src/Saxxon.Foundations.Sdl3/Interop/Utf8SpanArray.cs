using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Represents an unmanaged list of string values, where the values are from an unmanaged source.
/// </summary>
[PublicAPI]
internal readonly ref struct Utf8SpanArray
{
    private readonly ReadOnlySpan<IntPtr> _pointers;

    /// <summary>
    /// Number of items in the list.
    /// </summary>
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

    /// <summary>
    /// Converts the pointer table to a managed array.
    /// </summary>
    public string?[] ToArray()
    {
        var result = new string?[Count];
        for (var i = 0; i < Count; i++)
            result[i] = _pointers[i].GetString();
        return result;
    }

    /// <summary>
    /// Converts the pointer table to a managed list.
    /// </summary>
    public List<string> ToList()
    {
        var result = new List<string>(Count);
        for (var i = 0; i < Count; i++)
            result.Add(_pointers[i].GetString()!);
        return result;
    }
}