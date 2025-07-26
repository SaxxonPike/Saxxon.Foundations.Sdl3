using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class TrayEntry
{
    public static unsafe IntPtr<SDL_TrayMenu> GetParentTrayMenu(this IntPtr<SDL_TrayEntry> entry)
    {
        return SDL_GetTrayEntryParent(entry);
    }

    public static unsafe void Click(this IntPtr<SDL_TrayEntry> entry)
    {
        SDL_ClickTrayEntry(entry);
    }

    private static readonly Dictionary<IntPtr<SDL_TrayEntry>, IntPtr> CallbackData = [];

    public static unsafe void SetCallback(this IntPtr<SDL_TrayEntry> entry, Action? callback)
    {
        if (CallbackData.Remove(entry, out var existing))
            UserDataStore.Remove(existing);

        if (callback == null)
            return;

        var userData = UserDataStore.Add(callback);
        CallbackData.Add(entry, userData);
        SDL_SetTrayEntryCallback(entry, &Execute, userData);

        return;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static void Execute(IntPtr userData, SDL_TrayEntry* entry)
        {
            if (UserDataStore.TryGet<Action>(userData, out var target))
                target!.Invoke();
        }
    }

    public static unsafe void SetEnabled(this IntPtr<SDL_TrayEntry> entry, bool enabled)
    {
        SDL_SetTrayEntryEnabled(entry, enabled);
    }

    public static unsafe bool GetEnabled(this IntPtr<SDL_TrayEntry> entry)
    {
        return SDL_GetTrayEntryEnabled(entry);
    }

    public static unsafe void SetChecked(this IntPtr<SDL_TrayEntry> entry, bool @checked)
    {
        SDL_SetTrayEntryChecked(entry, @checked);
    }

    public static unsafe bool GetChecked(this IntPtr<SDL_TrayEntry> entry)
    {
        return SDL_GetTrayEntryChecked(entry);
    }

    public static unsafe string? GetLabel(this IntPtr<SDL_TrayEntry> entry)
    {
        return SDL_GetTrayEntryLabel(entry);
    }

    public static unsafe void SetLabel(this IntPtr<SDL_TrayEntry> entry, string? label)
    {
        SDL_SetTrayEntryLabel(entry, label);
    }

    public static unsafe void Remove(this IntPtr<SDL_TrayEntry> entry)
    {
        SDL_RemoveTrayEntry(entry);
    }

    public static unsafe IntPtr<SDL_TrayMenu> GetSubMenu(this IntPtr<SDL_TrayEntry> entry)
    {
        return SDL_GetTraySubmenu(entry);
    }

    public static unsafe IntPtr<SDL_TrayMenu> CreateSubMenu(this IntPtr<SDL_TrayEntry> entry)
    {
        return SDL_CreateTraySubmenu(entry);
    }
}