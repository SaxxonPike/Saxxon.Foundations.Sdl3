using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an interface for manipulating screen saver settings.
/// </summary>
public static class ScreenSaver
{
    public static void Enable()
    {
        SDL_EnableScreenSaver()
            .AssertSdlSuccess();
    }

    public static void Disable()
    {
        SDL_DisableScreenSaver()
            .AssertSdlSuccess();
    }

    public static bool IsEnabled()
    {
        return SDL_ScreenSaverEnabled();
    }
}