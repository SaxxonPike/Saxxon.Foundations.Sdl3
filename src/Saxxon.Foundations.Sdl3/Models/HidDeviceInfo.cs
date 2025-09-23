using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_hid_device_info"/>.
/// </summary>
[PublicAPI]
public static class HidDeviceInfo
{
    /// <summary>
    /// Gets the platform-specific device path for an HID device.
    /// </summary>
    public static unsafe string? GetPath(this IntPtr<SDL_hid_device_info> info) => 
        ((IntPtr<byte>)info.AsReadOnlyRef().path).GetString();

    /// <summary>
    /// Gets the device vendor ID for an HID device.
    /// </summary>
    public static ushort GetVendorId(this IntPtr<SDL_hid_device_info> info) => 
        info.AsReadOnlyRef().vendor_id;

    /// <summary>
    /// Gets the device product ID for an HID device.
    /// </summary>
    public static ushort GetProductId(this IntPtr<SDL_hid_device_info> info) => 
        info.AsReadOnlyRef().product_id;

    /// <summary>
    /// Gets the serial number for an HID device.
    /// </summary>
    public static string? GetSerialNumber(this IntPtr<SDL_hid_device_info> info) =>
        info.AsReadOnlyRef().serial_number.GetWString();

    /// <summary>
    /// Gets the Device Release Number in binary-coded decimal, also known as the Device Version Number,
    /// for an HID device.
    /// </summary>
    public static ushort GetReleaseNumber(this IntPtr<SDL_hid_device_info> info) => 
        info.AsReadOnlyRef().release_number;

    /// <summary>
    /// Gets the Manufacturer string for an HID device.
    /// </summary>
    public static string? GetManufacturerString(this IntPtr<SDL_hid_device_info> info) =>
        info.AsReadOnlyRef().manufacturer_string.GetWString();

    /// <summary>
    /// Gets the Product string for an HID device.
    /// </summary>
    public static string? GetProductString(this IntPtr<SDL_hid_device_info> info) =>
        info.AsReadOnlyRef().product_string.GetWString();

    /// <summary>
    /// Gets the Usage Page for an HID device.
    /// </summary>
    public static ushort GetUsagePage(this IntPtr<SDL_hid_device_info> info) => 
        info.AsReadOnlyRef().usage_page;

    /// <summary>
    /// Gets the Usage for an HID device.
    /// </summary>
    public static ushort GetUsage(this IntPtr<SDL_hid_device_info> info) => 
        info.AsReadOnlyRef().usage;

    /// <summary>
    /// Gets the USB interface which the logical device represents, if it is a USB connected
    /// HID device.
    /// </summary>
    public static int GetInterfaceNumber(this IntPtr<SDL_hid_device_info> info) => 
        info.AsReadOnlyRef().interface_number;

    /// <summary>
    /// Gets additional information about the USB interface for an HID device.
    /// </summary>
    public static int GetInterfaceClass(this IntPtr<SDL_hid_device_info> info) => 
        info.AsReadOnlyRef().interface_class;

    /// <summary>
    /// Gets the interface subclass for an HID device.
    /// </summary>
    public static int GetInterfaceSubclass(this IntPtr<SDL_hid_device_info> info) => 
        info.AsReadOnlyRef().interface_subclass;

    /// <summary>
    /// Gets the interface protocol for an HID device.
    /// </summary>
    public static int GetInterfaceProtocol(this IntPtr<SDL_hid_device_info> info) => 
        info.AsReadOnlyRef().interface_protocol;

    /// <summary>
    /// Gets the underlying bus type for an HID device.
    /// </summary>
    public static SDL_hid_bus_type GetBusType(this IntPtr<SDL_hid_device_info> info) => 
        info.AsReadOnlyRef().bus_type;
}