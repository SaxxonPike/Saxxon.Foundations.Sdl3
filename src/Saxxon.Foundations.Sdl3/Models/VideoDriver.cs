using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for SDL video drivers.
/// </summary>
[PublicAPI]
public static class VideoDriver
{
    public static unsafe string? GetCurrent()
    {
        return ((IntPtr)Unsafe_SDL_GetCurrentVideoDriver()).GetString();
    }

    public static unsafe List<string> GetAll()
    {
        var result = new List<string>();
        var count = SDL_GetNumVideoDrivers();

        for (var i = 0; i < count; i++)
        {
            if (((IntPtr)Unsafe_SDL_GetVideoDriver(i)).GetString() is { } name)
                result.Add(name);
        }

        return result;
    }
}