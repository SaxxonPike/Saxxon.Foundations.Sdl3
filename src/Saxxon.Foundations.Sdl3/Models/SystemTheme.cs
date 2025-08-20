using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_SystemTheme"/>.
/// </summary>
[PublicAPI]
public static class SystemTheme
{
    public static SDL_SystemTheme Get()
    {
        return SDL_GetSystemTheme();
    }
}