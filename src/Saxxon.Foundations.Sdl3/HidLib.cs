using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for the SDL HID API.
/// </summary>
[PublicAPI]
public static class HidLib
{
    /// <summary>
    /// Start or stop a BLE scan on iOS and tvOS to pair Steam Controllers.
    /// </summary>
    /// <param name="active">
    /// True to start the scan, false to stop the scan.
    /// </param>
    public static void BleScan(bool active)
    {
        SDL_hid_ble_scan(active);
    }

    /// <summary>
    /// Check to see if devices may have been added or removed.
    /// </summary>
    /// <returns>
    /// A change counter, incremented with each potential device change, or 0 if device change detection
    /// isn't available.
    /// </returns>
    /// <remarks>
    /// Enumerating the HID devices is an expensive operation, so you can call this to see if there have been any
    /// system device changes since the last call to this function. A change in the counter returned doesn't
    /// necessarily mean that anything has changed, but you can call <see cref="HidDevice.GetAllInfo"/> to get an
    /// updated device list. Calling this function for the first time may cause a thread or other system resource to be
    /// allocated to track device change notifications.
    /// </remarks>
    public static uint GetDeviceChangeCount()
    {
        return SDL_hid_device_change_count();
    }

    /// <summary>
    /// Finalize the HIDAPI library.
    /// </summary>
    public static void Exit()
    {
        SDL_hid_exit()
            .AssertSdlZero();
    }
    
    
}