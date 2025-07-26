using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_PixelFormatDetails"/>.
/// </summary>
[PublicAPI]
public static class PixelFormatDetails
{
    public static unsafe uint MapRgb(
        this IntPtr<SDL_PixelFormatDetails> ptr,
        IntPtr<SDL_Palette> palette,
        byte r,
        byte g,
        byte b
    )
    {
        return SDL_MapRGB(ptr, palette, r, g, b);
    }
    
    public static unsafe uint MapRgba(
        this IntPtr<SDL_PixelFormatDetails> ptr,
        IntPtr<SDL_Palette> palette,
        byte r,
        byte g,
        byte b,
        byte a
    )
    {
        return SDL_MapRGBA(ptr, palette, r, g, b, a);
    }
    

    public static unsafe SDL_Color GetRgb(
        this IntPtr<SDL_PixelFormatDetails> format,
        uint pixelValue,
        IntPtr<SDL_Palette> palette
    )
    {
        byte r, g, b;
        SDL_GetRGB(pixelValue, format, palette, &r, &g, &b);
        return new SDL_Color
        {
            r = r,
            g = g,
            b = b,
            a = 255
        };
    }

    public static unsafe SDL_Color GetRgba(
        this IntPtr<SDL_PixelFormatDetails> format,
        uint pixelValue,
        IntPtr<SDL_Palette> palette
    )
    {
        byte r, g, b, a;
        SDL_GetRGBA(pixelValue, format, palette, &r, &g, &b, &a);
        return new SDL_Color
        {
            r = r,
            g = g,
            b = b,
            a = a
        };
    }
}