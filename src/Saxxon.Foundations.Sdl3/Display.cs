using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_DisplayID"/>.
/// </summary>
[PublicAPI]
public static class Display
{
    public static unsafe SDL_DisplayID GetForPoint(SDL_Point point)
    {
        return SDL_GetDisplayForPoint(&point);
    }

    public static unsafe SDL_DisplayID GetForRect(SDL_Rect rect)
    {
        return SDL_GetDisplayForRect(&rect);
    }

    public static unsafe IntPtr<SDL_DisplayMode> GetDesktopMode(this SDL_DisplayID display)
    {
        return SDL_GetDesktopDisplayMode(display);
    }

    public static unsafe IntPtr<SDL_DisplayMode> GetCurrentMode(this SDL_DisplayID display)
    {
        return SDL_GetCurrentDisplayMode(display);
    }

    public static unsafe SDL_DisplayMode GetClosestFullScreenMode(
        this SDL_DisplayID display,
        int w,
        int h,
        float refreshRate,
        bool includeHighDensityModes
    )
    {
        SDL_DisplayMode closest;
        SDL_GetClosestFullscreenDisplayMode(display, w, h, refreshRate, includeHighDensityModes, &closest)
            .AssertSdlSuccess();

        return closest;
    }
    
    public static unsafe IMemoryOwner<IntPtr<SDL_DisplayMode>> GetFullScreenModes(this SDL_DisplayID display)
    {
        int count;
        var modes = ((IntPtr<IntPtr<SDL_DisplayMode>>)SDL_GetFullscreenDisplayModes(display, &count))
            .AssertSdlNotNull();
        return SdlMemoryManager.Owned(modes, count);
    }

    public static float GetContentScale(this SDL_DisplayID display)
    {
        return SDL_GetDisplayContentScale(display);
    }

    public static SDL_DisplayOrientation GetCurrentOrientation(this SDL_DisplayID display)
    {
        return SDL_GetCurrentDisplayOrientation(display);
    }

    public static SDL_DisplayOrientation GetNaturalOrientation(this SDL_DisplayID display)
    {
        return SDL_GetNaturalDisplayOrientation(display);
    }

    public static unsafe SDL_Rect GetUsableBounds(this SDL_DisplayID display)
    {
        SDL_Rect result;
        SDL_GetDisplayUsableBounds(display, &result)
            .AssertSdlSuccess();
        return result;
    }
    
    public static unsafe SDL_Rect GetBounds(this SDL_DisplayID display)
    {
        SDL_Rect result;
        SDL_GetDisplayBounds(display, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe string? GetName(this SDL_DisplayID display)
    {
        return Ptr.ToUtf8String(Unsafe_SDL_GetDisplayName(display));
    }

    public static SDL_PropertiesID GetProperties(this SDL_DisplayID display)
    {
        return SDL_GetDisplayProperties(display);
    }

    public static SDL_DisplayID GetPrimary()
    {
        return SDL_GetPrimaryDisplay();
    }

    public static unsafe IMemoryOwner<SDL_DisplayID> GetAll()
    {
        int count;
        var displays = ((IntPtr<SDL_DisplayID>)SDL_GetDisplays(&count))
            .AssertSdlNotNull();
        return SdlMemoryManager.Owned(displays, count);
    }
}