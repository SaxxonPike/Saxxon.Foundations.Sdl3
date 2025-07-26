using System.Runtime.InteropServices;

namespace Saxxon.Foundations.Sdl3.Models;

public static class VideoDriver
{
    public static unsafe string? GetCurrent()
    {
        return Marshal.PtrToStringUTF8((IntPtr)Unsafe_SDL_GetCurrentVideoDriver());
    }

    public static unsafe List<string> GetAll()
    {
        var result = new List<string>();
        var count = SDL_GetNumVideoDrivers();

        for (var i = 0; i < count; i++)
        {
            if (Marshal.PtrToStringUTF8((IntPtr)Unsafe_SDL_GetVideoDriver(i)) is { } name)
                result.Add(name);
        }

        return result;
    }
}