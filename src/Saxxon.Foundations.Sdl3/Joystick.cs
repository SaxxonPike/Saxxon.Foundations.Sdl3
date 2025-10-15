using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Joystick"/> and
/// <see cref="SDL_JoystickID"/>.
/// </summary>
[PublicAPI]
public static class Joystick
{
    public static unsafe IntPtr<SDL_Gamepad> GetGamepad(
        this SDL_JoystickID id
    )
    {
        return ((IntPtr<SDL_Gamepad>)SDL_GetGamepadFromID(id))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Joystick> GetDevice(
        this SDL_JoystickID id
    )
    {
        return SDL_GetJoystickFromID(id);
    }

    public static unsafe Guid GetGuid(
        this IntPtr<SDL_Joystick> joystick
    )
    {
        var value = SDL_GetJoystickGUID(joystick);
        var result = *(Guid*)&value;
        return result == Guid.Empty ? throw new SdlException() : result;
    }

    public static unsafe Guid GetGuid(
        this SDL_JoystickID id
    )
    {
        var value = SDL_GetJoystickGUIDForID(id);
        var result = *(Guid*)&value;
        return result == Guid.Empty ? throw new SdlException() : result;
    }

    public static string? GetName(
        this SDL_JoystickID id
    )
    {
        return SDL_GetJoystickNameForID(id);
    }

    public static unsafe string? GetName(
        this IntPtr<SDL_Joystick> joystick
    )
    {
        return SDL_GetJoystickName(joystick);
    }

    public static string GetPath(
        this SDL_JoystickID id
    )
    {
        return SDL_GetJoystickPathForID(id) ?? throw new SdlException();
    }

    public static unsafe string GetPath(
        this IntPtr<SDL_Joystick> joystick
    )
    {
        return SDL_GetJoystickPath(joystick) ?? throw new SdlException();
    }

    public static int GetPlayerIndex(
        this SDL_JoystickID id
    )
    {
        return SDL_GetJoystickPlayerIndexForID(id);
    }

    public static unsafe int GetPlayerIndex(
        this IntPtr<SDL_Joystick> joystick
    )
    {
        return SDL_GetJoystickPlayerIndex(joystick);
    }

    public static int GetProduct(
        this SDL_JoystickID id
    )
    {
        return SDL_GetJoystickProductForID(id);
    }

    public static unsafe int GetProduct(
        this IntPtr<SDL_Joystick> joystick
    )
    {
        return SDL_GetJoystickProduct(joystick);
    }

    public static int GetProductVersion(
        this SDL_JoystickID id
    )
    {
        return SDL_GetJoystickProductVersionForID(id);
    }

    public static unsafe int GetProductVersion(
        this IntPtr<SDL_Joystick> joystick
    )
    {
        return SDL_GetJoystickProductVersion(joystick);
    }

    public static bool IsGamepad(this SDL_JoystickID id)
    {
        return SDL_IsGamepad(id);
    }

    public static unsafe IntPtr<SDL_Gamepad> OpenGamepad(
        this SDL_JoystickID id
    )
    {
        return ((IntPtr<SDL_Gamepad>)SDL_OpenGamepad(id))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Joystick> Open(
        this SDL_JoystickID id
    )
    {
        return ((IntPtr<SDL_Joystick>)SDL_OpenJoystick(id))
            .AssertSdlNotNull();
    }

    public static unsafe void SendEffect(
        this IntPtr<SDL_Joystick> joystick,
        ReadOnlySpan<byte> data
    )
    {
        fixed (byte* dataPtr = data)
        {
            SDL_SendJoystickEffect(joystick, (IntPtr)dataPtr, data.Length)
                .AssertSdlSuccess();
        }
    }
}