namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Represents a gamepad. This wraps <see cref="SDL_JoystickID"/> for the
/// purposes of cleaning up interaction with the SDL API.
///
/// This is not an actual SDL type.
/// </summary>
// ReSharper disable InconsistentNaming
public enum SDL_GamepadID : uint;