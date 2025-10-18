using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Extensions for <see cref="SDL_AudioSpec"/>.
/// </summary>
[PublicAPI]
public static class AudioSpecExtensions
{
    /// <summary>
    /// Extensions for <see cref="SDL_AudioSpec"/> references.
    /// </summary>
    extension(SDL_AudioSpec spec)
    {
        /// <summary>
        /// Gets the block size of an audio format, which indicates the minimum
        /// number of bytes that can represent a single sample (taking into
        /// consideration channel count and bit-depth.)
        /// </summary>
        /// <returns></returns>
        public int BlockSize =>
            Math.Max(1, SDL_AUDIO_BITSIZE(spec.format) * spec.channels / 8);

        /// <summary>
        /// Returns true only if the given audio specs match exactly.
        /// </summary>
        public bool Matches(SDL_AudioSpec other)
        {
            return spec.channels == other.channels &&
                   spec.format == other.format &&
                   spec.freq == other.freq;
        }
    }
}