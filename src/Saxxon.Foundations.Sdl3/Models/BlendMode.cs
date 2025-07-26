using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_BlendMode"/>.
/// </summary>
[PublicAPI]
public static class BlendMode
{
    public static SDL_BlendMode ComposeCustom(
        SDL_BlendFactor srcColorFactor,
        SDL_BlendFactor dstColorFactor,
        SDL_BlendOperation colorOperation,
        SDL_BlendFactor srcAlphaFactor,
        SDL_BlendFactor dstAlphaFactor,
        SDL_BlendOperation alphaOperation
    )
    {
        return SDL_ComposeCustomBlendMode(
            srcColorFactor,
            dstColorFactor,
            colorOperation,
            srcAlphaFactor,
            dstAlphaFactor,
            alphaOperation
        );
    }
}