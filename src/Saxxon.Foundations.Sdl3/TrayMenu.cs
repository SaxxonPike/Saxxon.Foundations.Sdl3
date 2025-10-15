using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_TrayMenu"/>.
/// </summary>
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
        using var labelStr = new UnmanagedString(label);
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

    /// <summary>
    /// Returns entries in the menu, in order.
    /// </summary>
    /// <param name="menu">
    /// The menu to get entries from.
    /// </param>
    /// <returns>
    /// A span of entries within the given menu.
    /// </returns>
    /// <remarks>
    /// The data becomes invalid when any function that inserts or deletes
    /// entries in the menu is called.
    /// </remarks>
    public static unsafe ReadOnlySpan<IntPtr<SDL_TrayEntry>> GetEntries(
        this IntPtr<SDL_TrayMenu> menu
    )
    {
        int count;
        return ((IntPtr<IntPtr<SDL_TrayEntry>>)SDL_GetTrayEntries(menu, &count))
            .AsNullTerminatedSpan();
    }
}