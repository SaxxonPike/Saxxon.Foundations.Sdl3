using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_BlendMode"/>.
/// </summary>
[PublicAPI]
public static class BlendMode
{
    /// <summary>
    /// Composes a custom blend mode for renderers.
    /// </summary>
    /// <param name="srcColorFactor">
    /// The <see cref="SDL_BlendFactor"/> applied to the red, green, and blue
    /// components of the source pixels.
    /// </param>
    /// <param name="dstColorFactor">
    /// The <see cref="SDL_BlendFactor"/> applied to the red, green, and blue
    /// components of the destination pixels.
    /// </param>
    /// <param name="colorOperation">
    /// The <see cref="SDL_BlendOperation"/> used to combine the red, green,
    /// and blue components of the source and destination pixels.
    /// </param>
    /// <param name="srcAlphaFactor">
    /// The <see cref="SDL_BlendFactor"/> applied to the alpha component of the
    /// source pixels.
    /// </param>
    /// <param name="dstAlphaFactor">
    /// The <see cref="SDL_BlendFactor"/> applied to the alpha component of the
    /// destination pixels.
    /// </param>
    /// <param name="alphaOperation">
    /// The <see cref="SDL_BlendOperation"/> used to combine the alpha
    /// component of the source and destination pixels.
    /// </param>
    /// <returns>
    /// An <see cref="SDL_BlendMode"/> that represents the chosen factors and
    /// operations.
    /// </returns>
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