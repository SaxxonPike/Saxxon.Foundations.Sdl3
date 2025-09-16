using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class GamepadButton
{
    public static SDL_GamepadButton GetFromString(ReadOnlySpan<char> button)
    {
        using var buttonStr = new UnmanagedString(button);
        return SDL_GetGamepadButtonFromString(buttonStr);
    }
    
    public static string? GetStringForButton(
        this SDL_GamepadButton button
    )
    {
        return SDL_GetGamepadStringForButton(button);
    }
}