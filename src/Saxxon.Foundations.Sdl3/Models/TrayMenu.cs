using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class TrayMenu
{
    public static unsafe IntPtr<SDL_Tray> GetParentTray(this IntPtr<SDL_TrayMenu> menu)
    {
        return SDL_GetTrayMenuParentTray(menu);
    }

    public static unsafe IntPtr<SDL_TrayEntry> GetParentEntry(this IntPtr<SDL_TrayMenu> menu)
    {
        return SDL_GetTrayMenuParentEntry(menu);
    }

    public static unsafe IntPtr<SDL_TrayEntry> InsertEntry(
        this IntPtr<SDL_TrayMenu> menu,
        int pos,
        ReadOnlySpan<char> label,
        SDL_TrayEntryFlags flags
    )
    {
        using var labelStr = new Utf8Span(label);
        return ((IntPtr<SDL_TrayEntry>)SDL_InsertTrayEntryAt(menu, pos, labelStr, flags))
            .AssertSdlNotNull();
    }

    public static IntPtr<SDL_TrayEntry> AddEntry(
        this IntPtr<SDL_TrayMenu> menu,
        ReadOnlySpan<char> label,
        SDL_TrayEntryFlags flags
    )
    {
        return InsertEntry(menu, -1, label, flags);
    }

    public static unsafe IMemoryOwner<IntPtr<SDL_TrayEntry>> GetEntries(
        this IntPtr<SDL_TrayMenu> menu
    )
    {
        int count;
        var entries = SDL_GetTrayEntries(menu, &count);
        return SdlMemoryManager.Const(entries, count);
    }
}