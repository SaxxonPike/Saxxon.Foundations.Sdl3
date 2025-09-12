using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Gamepad"/>.
/// </summary>
[PublicAPI]
public static class Gamepad
{
    private const short MinAxisValue = -32767;
    private const short MaxAxisValue = 32767;
    private const float AxisValueScale = 32767f;

    public static bool AddMapping(ReadOnlySpan<char> mapping)
    {
        using var mappingStr = new Utf8Span(mapping);
        return SDL_AddGamepadMapping(mappingStr)
            .AssertSdlNotEqual(-1) > 0;
    }

    public static int AddMappingsFromFile(ReadOnlySpan<char> file)
    {
        using var fileStr = new Utf8Span(file);
        return SDL_AddGamepadMappingsFromFile(fileStr)
            .AssertSdlNotEqual(-1);
    }

    public static unsafe int AddMappingsFromIo(IntPtr<SDL_IOStream> src, bool closeIo)
    {
        return SDL_AddGamepadMappingsFromIO(src, closeIo)
            .AssertSdlNotEqual(-1);
    }

    public static unsafe void Close(this IntPtr<SDL_Gamepad> ptr)
    {
        SDL_CloseGamepad(ptr);
    }

    public static unsafe bool IsConnected(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GamepadConnected(ptr);
    }

    public static bool EventsAreEnabled(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GamepadEventsEnabled();
    }

    public static unsafe bool HasAxis(this IntPtr<SDL_Gamepad> ptr, SDL_GamepadAxis axis)
    {
        return SDL_GamepadHasAxis(ptr, axis);
    }

    public static unsafe bool HasButton(this IntPtr<SDL_Gamepad> ptr, SDL_GamepadButton button)
    {
        return SDL_GamepadHasButton(ptr, button);
    }

    public static unsafe bool HasSensor(this IntPtr<SDL_Gamepad> ptr, SDL_SensorType sensor)
    {
        return SDL_GamepadHasSensor(ptr, sensor);
    }

    public static unsafe bool IsSensorEnabled(this IntPtr<SDL_Gamepad> ptr, SDL_SensorType sensor)
    {
        return SDL_GamepadSensorEnabled(ptr, sensor);
    }

    public static unsafe float GetAxis(this IntPtr<SDL_Gamepad> ptr, SDL_GamepadAxis axis)
    {
        return Math.Clamp(SDL_GetGamepadAxis(ptr, axis), MinAxisValue, MaxAxisValue) / AxisValueScale;
    }

    public static unsafe List<SDL_GamepadBinding> GetBindings(this IntPtr<SDL_Gamepad> ptr)
    {
        IntPtr<IntPtr<SDL_GamepadBinding>> items = default;

        try
        {
            int count;
            items = ((IntPtr<IntPtr<SDL_GamepadBinding>>)SDL_GetGamepadBindings(ptr, &count))
                .AssertSdlNotNull();

            var result = new List<SDL_GamepadBinding>(count);
            foreach (var item in items.AsSpan(count))
                result.Add(item.AsReadOnlyRef());

            return result;
        }
        finally
        {
            if (!items.IsNull)
                SDL_free(items.Ptr);
        }
    }

    public static unsafe bool GetButton(this IntPtr<SDL_Gamepad> ptr, SDL_GamepadButton button)
    {
        return SDL_GetGamepadButton(ptr, button);
    }

    public static unsafe SDL_GamepadButtonLabel GetButtonLabel(this IntPtr<SDL_Gamepad> ptr, SDL_GamepadButton button)
    {
        return SDL_GetGamepadButtonLabel(ptr, button);
    }

    public static unsafe SDL_JoystickConnectionState GetConnectionState(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadConnectionState(ptr);
    }

    public static unsafe int GetFirmwareVersion(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadFirmwareVersion(ptr);
    }

    public static unsafe IntPtr<SDL_Gamepad> GetFromPlayerIndex(int player)
    {
        return SDL_GetGamepadFromPlayerIndex(player);
    }

    public static unsafe SDL_GamepadID GetId(this IntPtr<SDL_Gamepad> ptr)
    {
        var value = SDL_GetGamepadID(ptr);
        if (value == 0)
            throw new SdlException();
        return (SDL_GamepadID)value;
    }

    public static unsafe string GetMapping(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadMapping(ptr) ?? throw new SdlException();
    }

    public static unsafe string GetMappingForGuid(Guid guid)
    {
        return SDL_GetGamepadMappingForGUID(*(SDL_GUID*)&guid) ?? throw new SdlException();
    }

    public static unsafe List<string> GetMappings()
    {
        var result = new List<string>();
        IntPtr<IntPtr<byte>> mappings = default;

        try
        {
            int count;
            mappings = ((IntPtr<IntPtr<byte>>)SDL_GetGamepadMappings(&count))
                .AssertSdlNotNull();

            foreach (var mapping in mappings.AsSpan(count))
                result.Add(mapping.GetString()!);

            return result;
        }
        finally
        {
            if (!mappings.IsNull)
                SDL_free(mappings.Ptr);
        }
    }

    public static unsafe string? GetName(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadName(ptr);
    }

    public static unsafe string? GetPath(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadPath(ptr);
    }

    public static unsafe int GetPlayerIndex(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadPlayerIndex(ptr);
    }

    public static unsafe (SDL_PowerState State, int Percent) GetPowerInfo(this IntPtr<SDL_Gamepad> ptr)
    {
        int percent;
        var result = SDL_GetGamepadPowerInfo(ptr, &percent);
        return (result, percent);
    }

    public static unsafe int GetProduct(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadProduct(ptr);
    }

    public static unsafe int GetProductVersion(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadProductVersion(ptr);
    }

    public static unsafe SDL_PropertiesID GetProperties(this IntPtr<SDL_Gamepad> ptr)
    {
        var id = SDL_GetGamepadProperties(ptr);
        if (id == 0)
            throw new SdlException();
        return id;
    }

    [MustDisposeResource]
    public static unsafe IMemoryOwner<SDL_GamepadID> GetAll()
    {
        int count;
        var gamepads = (SDL_GamepadID*)SDL_GetGamepads(&count);
        return SdlMemoryManager.Owned(gamepads, count);
    }

    public static unsafe string? GetSerial(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadSerial(ptr);
    }

    public static unsafe ulong GetSteamHandle(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadSteamHandle(ptr);
    }

    public static unsafe (bool Down, float X, float Y, float Pressure) GetTouchpadFinger(
        this IntPtr<SDL_Gamepad> ptr,
        int touchPad,
        int finger
    )
    {
        SDLBool down;
        float x, y, pressure;

        SDL_GetGamepadTouchpadFinger(ptr, touchPad, finger, &down, &x, &y, &pressure)
            .AssertSdlSuccess();

        return (Down: down, X: x, Y: y, Pressure: pressure);
    }

    public static unsafe SDL_GamepadType GetGamepadType(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadType(ptr);
    }

    public static SDL_GamepadType GetGamepadTypeFromString(ReadOnlySpan<char> str)
    {
        using var buttonStr = new Utf8Span(str);
        return SDL_GetGamepadTypeFromString(buttonStr);
    }

    public static unsafe int GetVendor(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetGamepadVendor(ptr);
    }

    public static int GetVendor(this SDL_GamepadID id)
    {
        return SDL_GetGamepadVendorForID((SDL_JoystickID)id);
    }

    public static unsafe int GetNumTouchpadFingers(this IntPtr<SDL_Gamepad> ptr, int touchPad)
    {
        return SDL_GetNumGamepadTouchpadFingers(ptr, touchPad);
    }

    public static unsafe int GetNumTouchpads(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetNumGamepadTouchpads(ptr);
    }

    public static unsafe SDL_GamepadType GetRealGamepadType(this IntPtr<SDL_Gamepad> ptr)
    {
        return SDL_GetRealGamepadType(ptr);
    }

    public static SDL_GamepadType GetRealGamepadType(this SDL_GamepadID id)
    {
        return SDL_GetRealGamepadTypeForID((SDL_JoystickID)id);
    }

    public static bool HasGamepad()
    {
        return SDL_HasGamepad();
    }

    public static unsafe IntPtr<SDL_Gamepad> Open(this SDL_GamepadID id)
    {
        return ((IntPtr<SDL_Gamepad>)SDL_OpenGamepad((SDL_JoystickID)id))
            .AssertSdlNotNull();
    }

    public static void ReloadMappings()
    {
        SDL_ReloadGamepadMappings();
    }

    public static unsafe void Rumble(this IntPtr<SDL_Gamepad> ptr, float lowIntensity, float highIntensity, TimeSpan time)
    {
        var lowValue = (ushort)(lowIntensity * 65535f);
        var highValue = (ushort)(highIntensity * 65535f);
        SDL_RumbleGamepad(ptr, lowValue, highValue, (uint)time.TotalMilliseconds)
            .AssertSdlSuccess();
    }

    public static unsafe void RumbleTriggers(this IntPtr<SDL_Gamepad> ptr, float leftIntensity, float rightIntensity,
        TimeSpan time)
    {
        var leftValue = (ushort)(leftIntensity * 65535f);
        var rightValue = (ushort)(rightIntensity * 65535f);
        SDL_RumbleGamepadTriggers(ptr, leftValue, rightValue, (uint)time.TotalMilliseconds)
            .AssertSdlSuccess();
    }

    public static unsafe void SendEffect(this IntPtr<SDL_Gamepad> ptr, ReadOnlySpan<byte> data)
    {
        fixed (void* dataPtr = data)
        {
            SDL_SendGamepadEffect(ptr, (IntPtr)dataPtr, data.Length)
                .AssertSdlSuccess();
        }
    }

    public static void SetEventsEnabled(bool enabled)
    {
        SDL_SetGamepadEventsEnabled(enabled);
    }

    public static unsafe void SetLed(this IntPtr<SDL_Gamepad> ptr, byte r, byte g, byte b)
    {
        SDL_SetGamepadLED(ptr, r, g, b)
            .AssertSdlSuccess();
    }

    public static void SetMapping(SDL_JoystickID id, ReadOnlySpan<char> mapping)
    {
        using var mappingStr = new Utf8Span(mapping);
        SDL_SetGamepadMapping(id, mappingStr)
            .AssertSdlSuccess();
    }

    public static unsafe void SetPlayerIndex(this IntPtr<SDL_Gamepad> ptr, int playerIndex)
    {
        SDL_SetGamepadPlayerIndex(ptr, playerIndex)
            .AssertSdlSuccess();
    }

    public static unsafe void SetSensorEnabled(this IntPtr<SDL_Gamepad> ptr, SDL_SensorType type, bool enabled)
    {
        SDL_SetGamepadSensorEnabled(ptr, type, enabled)
            .AssertSdlSuccess();
    }

    public static void UpdateAll()
    {
        SDL_UpdateGamepads();
    }

    public static int GetProductVersion(
        this SDL_GamepadID id
    )
    {
        return SDL_GetGamepadProductVersionForID((SDL_JoystickID)id);
    }

    public static SDL_GamepadType GetGamepadType(
        this SDL_GamepadID id
    )
    {
        return SDL_GetGamepadTypeForID((SDL_JoystickID)id);
    }

    public static int GetProduct(
        this SDL_GamepadID id
    )
    {
        return SDL_GetGamepadProductForID((SDL_JoystickID)id);
    }

    public static int GetPlayerIndex(
        this SDL_GamepadID id
    )
    {
        return SDL_GetGamepadPlayerIndexForID((SDL_JoystickID)id);
    }

    public static string GetPath(
        this SDL_GamepadID id
    )
    {
        return SDL_GetGamepadPathForID((SDL_JoystickID)id) ?? throw new SdlException();
    }

    public static string GetName(
        this SDL_GamepadID id
    )
    {
        return SDL_GetGamepadNameForID((SDL_JoystickID)id) ?? throw new SdlException();
    }

    public static unsafe Guid GetGuid(
        this SDL_GamepadID id
    )
    {
        var value = SDL_GetGamepadGUIDForID((SDL_JoystickID)id);
        return *(Guid*)&value;
    }

    public static string GetMapping(
        this SDL_GamepadID id
    )
    {
        return SDL_GetGamepadMappingForID((SDL_JoystickID)id)
               ?? throw new SdlException();
    }

    public static unsafe IntPtr<SDL_Joystick> OpenDevice(
        this SDL_GamepadID id
    )
    {
        return ((IntPtr<SDL_Joystick>)SDL_OpenJoystick((SDL_JoystickID)id))
            .AssertSdlNotNull();
    }
}