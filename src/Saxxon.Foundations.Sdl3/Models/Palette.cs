using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class Palette
{
    public static unsafe void Destroy(
        this IntPtr<SDL_Palette> palette
    )
    {
        SDL_DestroyPalette(palette);
    }

    public static unsafe void SetColors(
        this IntPtr<SDL_Palette> palette,
        ReadOnlySpan<SDL_Color> colors,
        int firstColor
    )
    {
        fixed (SDL_Color* colorsPtr = colors)
        {
            SDL_SetPaletteColors(
                palette,
                colorsPtr,
                firstColor,
                colors.Length
            ).AssertSdlSuccess();
        }
    }

    public static unsafe IntPtr<SDL_Palette> Create(
        int colorCount
    )
    {
        return SDL_CreatePalette(colorCount);
    }
}