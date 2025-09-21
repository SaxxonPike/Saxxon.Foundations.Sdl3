using System.Buffers;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Helpers;

/// <summary>
/// Represents a list of HID device results.
/// </summary>
internal sealed class HidDeviceList : IMemoryOwner<IntPtr<SDL_hid_device_info>>
{
    private readonly unsafe SDL_hid_device_info* _list;
    private readonly IMemoryOwner<IntPtr<SDL_hid_device_info>> _memory;
    private readonly int _count;

    internal unsafe HidDeviceList(SDL_hid_device_info* list)
    {
        _list = list;

        // Extract a pointer array from the linked list.

        var temp = list;

        while (temp != null)
        {
            _count++;
            temp = temp->next;
        }

        _memory = SdlMemoryPool<IntPtr<SDL_hid_device_info>>.Shared.Rent(_count);
        var span = _memory.Memory.Span;
        var index = 0;
        temp = list;

        while (temp != null)
        {
            span[index++] = (IntPtr)temp;
            temp = temp->next;
        }
    }

    /// <inheritdoc />
    public unsafe void Dispose()
    {
        _memory.Dispose();
        SDL_hid_free_enumeration(_list);
    }

    /// <inheritdoc />
    public Memory<IntPtr<SDL_hid_device_info>> Memory => _memory.Memory;
}