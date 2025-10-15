using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_DisplayMode"/>.
/// </summary>
[PublicAPI]
public static class DisplayMode
{
    public static SDL_DisplayID GetDisplayId(this IntPtr<SDL_DisplayMode> ptr)
    {
        return ptr.AsReadOnlyRef().displayID;
    }

    public static SDL_PixelFormat GetPixelFormat(this IntPtr<SDL_DisplayMode> ptr)
    {
        return ptr.AsReadOnlyRef().format;
    }

    public static int GetWidth(this IntPtr<SDL_DisplayMode> ptr)
    {
        return ptr.AsReadOnlyRef().w;
    }

    public static int GetHeight(this IntPtr<SDL_DisplayMode> ptr)
    {
        return ptr.AsReadOnlyRef().h;
    }

    public static float GetRefreshRate(this IntPtr<SDL_DisplayMode> ptr)
    {
        return ptr.AsReadOnlyRef().refresh_rate;
    }

    public static int GetRefreshRateNumerator(this IntPtr<SDL_DisplayMode> ptr)
    {
        return ptr.AsReadOnlyRef().refresh_rate_numerator;
    }

    public static int GetRefreshRateDenominator(this IntPtr<SDL_DisplayMode> ptr)
    {
        return ptr.AsReadOnlyRef().refresh_rate_denominator;
    }
}