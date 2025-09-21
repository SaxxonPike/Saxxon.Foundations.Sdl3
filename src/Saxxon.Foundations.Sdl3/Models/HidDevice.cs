using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Helpers;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_hid_device"/>.
/// </summary>
[PublicAPI]
public static class HidDevice
{
    /// <summary>
    /// Close a HID device.
    /// </summary>
    /// <param name="dev">
    /// A device handle returned from <see cref="Open"/>.
    /// </param>
    public static unsafe void Close(IntPtr<SDL_hid_device> dev)
    {
        SDL_hid_close(dev)
            .AssertSdlZero();
    }

    /// <summary>
    /// Enumerate the HID Devices.
    /// </summary>
    /// <param name="vendorId">
    /// The Vendor ID (VID) of the types of device to open, or 0 to match any vendor.
    /// </param>
    /// <param name="productId">
    /// The Product ID (PID) of the types of device to open, or 0 to match any product.
    /// </param>
    /// <returns>
    /// A list of information about the HID devices attached to the system.
    /// </returns>
    public static unsafe IMemoryOwner<IntPtr<SDL_hid_device_info>> GetAllInfo(ushort vendorId, ushort productId)
    {
        var info = ((IntPtr<SDL_hid_device_info>)SDL_hid_enumerate(vendorId, productId))
            .AssertSdlNotNull();

        return new HidDeviceList(info);
    }

    /// <summary>
    /// Gets the device info from a HID device.
    /// </summary>
    /// <param name="dev">
    /// A device handle returned from <see cref="Open"/>.
    /// </param>
    /// <returns>
    /// A pointer to the HID device info.
    /// </returns>
    /// <remarks>
    /// This result is valid until the device is closed with <see cref="Close"/>.
    /// </remarks>
    public static unsafe IntPtr<SDL_hid_device_info> GetInfo(this IntPtr<SDL_hid_device> dev)
    {
        return ((IntPtr<SDL_hid_device_info>)SDL_hid_get_device_info(dev))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Gets a feature report from an HID device.
    /// </summary>
    /// <param name="dev">
    /// A device handle returned from <see cref="Open"/>.
    /// </param>
    /// <param name="reportId">
    /// Report ID of the report to be read, or zero if the device does not use numbered reports.
    /// </param>
    /// <param name="buffer">
    /// Buffer to write the report into. The buffer can be longer than the actual report.
    /// </param>
    /// <returns>
    /// Number of bytes actually read into the buffer.
    /// </returns>
    public static unsafe int GetFeatureReport(this IntPtr<SDL_hid_device> dev, byte reportId, Span<byte> buffer)
    {
        // Kind of annoying the way this was implemented. We can't know the length of the report
        // after the fact, and we can't use the buffer directly without sticking the report ID implementation detail
        // on the outside of our own API. Thus, the seemingly redundant buffer allocation.

        using var temp = SdlMemoryPool<byte>.Shared.Rent(buffer.Length + 1);

        fixed (byte* tempPtr = buffer)
        {
            tempPtr[0] = reportId;
            var result = SDL_hid_get_feature_report(dev, tempPtr + 1, (UIntPtr)buffer.Length)
                .AssertSdlNotEqual(-1);
            new Span<byte>(tempPtr, result)[1..].CopyTo(buffer);
            return result - 1;
        }
    }
}