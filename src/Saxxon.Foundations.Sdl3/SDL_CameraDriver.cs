using System.Runtime.InteropServices;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

// ReSharper disable once CheckNamespace
namespace SDL;

// ReSharper disable InconsistentNaming
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

/// <summary>
/// Represents a camera driver.
///
/// This is not an actual SDL type.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct SDL_CameraDriver(IntPtr<byte> ptr)
{
    public string? Name => ptr.GetString();
}