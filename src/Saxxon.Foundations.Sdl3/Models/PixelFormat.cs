using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_PixelFormat"/>.
/// </summary>
[PublicAPI]
public static class PixelFormat
{
    public static unsafe IntPtr<SDL_PixelFormatDetails> GetDetails(
        this SDL_PixelFormat format
    )
    {
        return SDL_GetPixelFormatDetails(format);
    }

    public static unsafe (int Bpp, uint RMask, uint GMask, uint BMask, uint AMask) GetMasks(
        this SDL_PixelFormat format
    )
    {
        uint rMask, gMask, bMask, amMask;
        int bpp;
        SDL_GetMasksForPixelFormat(format, &bpp, &rMask, &gMask, &bMask, &amMask)
            .AssertSdlSuccess();
        return (bpp, rMask, gMask, bMask, amMask);
    }

    public static unsafe string? GetName(
        this SDL_PixelFormat format
    )
    {
        return ((IntPtr)Unsafe_SDL_GetPixelFormatName(format)).GetString();
    }
}