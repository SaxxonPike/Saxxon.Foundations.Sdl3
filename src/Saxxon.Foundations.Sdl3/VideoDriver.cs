using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for SDL video drivers.
/// </summary>
[PublicAPI]
public static class VideoDriver
{
    /// <summary>
    /// Gets the current video driver.
    /// </summary>
    /// <returns></returns>
    public static unsafe string? GetCurrent()
    {
        return Ptr.ToUtf8String(Unsafe_SDL_GetCurrentVideoDriver());
    }

    /// <summary>
    /// Gets all available video drivers.
    /// </summary>
    public static unsafe List<string> GetAll()
    {
        var result = new List<string>();
        var count = SDL_GetNumVideoDrivers();

        for (var i = 0; i < count; i++)
        {
            if (Ptr.ToUtf8String(Unsafe_SDL_GetVideoDriver(i)) is { } name)
                result.Add(name);
        }

        return result;
    }
}