using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class GamepadType
{
    public static SDL_GamepadButtonLabel GetButtonLabel(
        this SDL_GamepadType type,
        SDL_GamepadButton button
    )
    {
        return SDL_GetGamepadButtonLabelForType(type, button);
    }

    public static string? GetStringForType(
        this SDL_GamepadType type
    )
    {
        return SDL_GetGamepadStringForType(type);
    }
}