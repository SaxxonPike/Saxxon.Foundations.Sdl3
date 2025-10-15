using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an interface for manipulating screen saver settings.
/// </summary>
[PublicAPI]
public static class ScreenSaver
{
    /// <summary>
    /// Allows the screen to be blanked by a screen saver.
    /// </summary>
    public static void Enable()
    {
        SDL_EnableScreenSaver()
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Prevents the screen from being blanked by a screen saver.
    /// </summary>
    public static void Disable()
    {
        SDL_DisableScreenSaver()
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Checks whether the screensaver is currently enabled.
    /// </summary>
    /// <returns>
    /// True if the screensaver is enabled, false if it is disabled.
    /// </returns>
    public static bool IsEnabled()
    {
        return SDL_ScreenSaverEnabled();
    }
}