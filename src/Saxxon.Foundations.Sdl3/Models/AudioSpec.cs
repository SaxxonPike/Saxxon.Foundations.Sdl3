using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Extensions for <see cref="SDL_AudioSpec"/>.
/// </summary>
[PublicAPI]
public static class AudioSpec
{
    /// <summary>
    /// Gets the block size of an audio format, which indicates the minimum
    /// number of bytes that can represent a single sample (taking into
    /// consideration channel count and bit-depth.)
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    public static int GetBlockSize(this SDL_AudioSpec spec)
    {
        return Math.Max(1, SDL_AUDIO_BITSIZE(spec.format) * spec.channels / 8);
    }

    /// <summary>
    /// Returns true only if the given audio specs match exactly.
    /// </summary>
    public static bool Matches(this SDL_AudioSpec spec, SDL_AudioSpec other)
    {
        return spec.channels == other.channels &&
               spec.format == other.format &&
               spec.freq == other.freq;
    }
}