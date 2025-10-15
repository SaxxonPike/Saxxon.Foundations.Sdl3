using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace SDL;

/// <summary>
/// Represents a gamepad. This wraps <see cref="SDL_JoystickID"/> for the
/// purposes of cleaning up interaction with the SDL API.
///
/// This is not an actual SDL type.
/// </summary>
[PublicAPI]
// ReSharper disable InconsistentNaming
public enum SDL_GamepadID : uint;