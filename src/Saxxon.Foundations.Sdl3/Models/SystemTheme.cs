namespace Saxxon.Foundations.Sdl3.Models;

public static class SystemTheme
{
    public static SDL_SystemTheme Get()
    {
        return SDL_GetSystemTheme();
    }
}