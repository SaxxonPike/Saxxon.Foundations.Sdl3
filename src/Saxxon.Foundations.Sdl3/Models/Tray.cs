using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class Tray
{
    public static void UpdateAll()
    {
        SDL_UpdateTrays();
    }

    public static unsafe void Destroy(this IntPtr<SDL_Tray> tray)
    {
        SDL_DestroyTray(tray);
    }

    public static unsafe IntPtr<SDL_TrayMenu> GetMenu(this IntPtr<SDL_Tray> tray)
    {
        return SDL_GetTrayMenu(tray);
    }

    public static unsafe IntPtr<SDL_TrayMenu> CreateMenu(this IntPtr<SDL_Tray> tray)
    {
        return SDL_CreateTrayMenu(tray);
    }

    public static unsafe void SetToolTip(this IntPtr<SDL_Tray> tray, ReadOnlySpan<char> toolTip)
    {
        using var toolTipStr = new Utf8Span(toolTip);
        SDL_SetTrayTooltip(tray, toolTipStr);
    }

    public static unsafe void SetIcon(this IntPtr<SDL_Tray> tray, IntPtr<SDL_Surface> icon)
    {
        SDL_SetTrayIcon(tray, icon);
    }

    public static unsafe IntPtr<SDL_Tray> Create(IntPtr<SDL_Surface> icon, ReadOnlySpan<char> toolTip)
    {
        using var toolTipStr = new Utf8Span(toolTip);
        return SDL_CreateTray(icon, toolTipStr);
    }
}