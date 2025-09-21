using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;
using Saxxon.Foundations.Sdl3.Models;

namespace Saxxon.Foundations.Sdl3.Helpers;

/// <summary>
/// Represents a list of file filters for when file or folder dialogs are opened.
/// </summary>
[PublicAPI]
internal sealed class DialogFileFilterList : IDisposable
{
    private readonly Utf8ByteStrings? _stringData;
    private readonly IntPtr<SDL_DialogFileFilter> _filterData;

    /// <summary>
    /// Raw pointer for the unmanaged data.
    /// </summary>
    public IntPtr<SDL_DialogFileFilter> Ptr => _filterData;
    
    /// <summary>
    /// Number of file filters.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Creates an unmanaged list of file filters for use with SDL calls.
    /// </summary>
    public unsafe DialogFileFilterList(IReadOnlyList<(string Name, string Pattern)> filters)
    {
        Count = filters.Count;

        // Build the string table.

        var stringList = new List<string>();

        foreach (var filter in filters)
        {
            stringList.Add(filter.Name);
            stringList.Add(filter.Pattern);
        }

        _stringData = new Utf8ByteStrings(CollectionsMarshal.AsSpan(stringList));

        // Populate the filter table with pointers from the string table.

        _filterData = Mem.Alloc<SDL_DialogFileFilter>(filters.Count);
        var stringSpan = _stringData.AsSpan();

        for (int i = 0, j = 0; j < stringList.Count; i++)
        {
            _filterData[i] = new SDL_DialogFileFilter
            {
                name = (byte*)stringSpan[j++],
                pattern = (byte*)stringSpan[j++]
            };
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        _stringData?.Dispose();
        SDL_free(_filterData.Ptr);
    }
}