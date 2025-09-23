using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_TouchID"/>.
/// </summary>
[PublicAPI]
public static class Touch
{
    /// <summary>
    /// Get a list of active fingers for a given touch device.
    /// </summary>
    /// <param name="touch">
    /// The ID of a touch device.
    /// </param>
    /// <returns>
    /// An array of SDL_Finger pointers.
    /// </returns>
    public static unsafe IMemoryOwner<IntPtr<SDL_Finger>> GetFingers(this SDL_TouchID touch)
    {
        int count;
        var fingers = SDL_GetTouchFingers(touch, &count);
        return SdlMemoryManager.Owned(fingers, count);
    }

    public static SDL_TouchDeviceType GetDeviceType(this SDL_TouchID touch)
    {
        return SDL_GetTouchDeviceType(touch);
    }

    public static unsafe string? GetName(this SDL_TouchID touch)
    {
        return Ptr.ToUtf8String(Unsafe_SDL_GetTouchDeviceName(touch));
    }

    [MustDisposeResource]
    public static unsafe IMemoryOwner<SDL_TouchID> GetDevices()
    {
        int count;
        var devices = SDL_GetTouchDevices(&count);
        return SdlMemoryManager.Owned(devices, count);
    }
}