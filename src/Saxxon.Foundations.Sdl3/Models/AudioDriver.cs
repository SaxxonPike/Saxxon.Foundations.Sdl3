using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for SDL audio drivers.
/// </summary>
[PublicAPI]
public class AudioDriver
{
    /// <summary>
    /// Gets the current audio driver.
    /// </summary>
    public static unsafe string? GetCurrent()
    {
        return Ptr.ToUtf8String(Unsafe_SDL_GetCurrentAudioDriver());
    }

    /// <summary>
    /// Gets all available audio drivers.
    /// </summary>
    public static unsafe List<string> GetAll()
    {
        var result = new List<string>();
        var count = SDL_GetNumAudioDrivers();

        for (var i = 0; i < count; i++)
        {
            if (Ptr.ToUtf8String(Unsafe_SDL_GetAudioDriver(i)) is { } name)
                result.Add(name);
        }

        return result;
    }
}