using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for SDL audio drivers.
/// </summary>
[PublicAPI]
public static class AudioDriverExtensions
{
    /// <summary>
    /// Extensions for <see cref="SDL_AudioDriver"/>.
    /// </summary>
    extension(SDL_AudioDriver)
    {
        /// <summary>
        /// Gets the current audio driver.
        /// </summary>
        public static unsafe SDL_AudioDriver? Current
        {
            get
            {
                var ptr = Unsafe_SDL_GetCurrentAudioDriver();
                return ptr == null ? null : new SDL_AudioDriver(ptr);
            }
        }

        /// <summary>
        /// Gets all available audio drivers.
        /// </summary>
        public static unsafe List<SDL_AudioDriver> GetAll()
        {
            var result = new List<SDL_AudioDriver>();
            var count = SDL_GetNumAudioDrivers();

            for (var i = 0; i < count; i++)
            {
                var item = Unsafe_SDL_GetAudioDriver(i);
                if (item != null)
                    result.Add(new SDL_AudioDriver(item));
            }

            return result;
        }
    }
}