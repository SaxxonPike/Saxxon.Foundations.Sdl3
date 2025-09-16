using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_GamepadAxis"/>.
/// </summary>
[PublicAPI]
public static class GamepadAxis
{
    public static SDL_GamepadAxis GetFromString(ReadOnlySpan<char> axis)
    {
        using var axisStr = new UnmanagedString(axis);
        return SDL_GetGamepadAxisFromString(axisStr);
    }
    
    public static string? GetString(
        this SDL_GamepadAxis axis
    )
    {
        return SDL_GetGamepadStringForAxis(axis);
    }

}