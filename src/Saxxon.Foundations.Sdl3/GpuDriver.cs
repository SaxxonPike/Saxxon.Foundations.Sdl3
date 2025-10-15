using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for drivers that are to be used with
/// SDL_gpu.
/// </summary>
[PublicAPI]
public static class GpuDriver
{
    public static unsafe string Get(
        int index
    )
    {
        IntPtr<byte> result = Unsafe_SDL_GetGPUDriver(index);
        return result.GetString() ?? throw new SdlException();
    }

    public static unsafe Dictionary<int, string> GetAll()
    {
        var result = new Dictionary<int, string>();
        var count = SDL_GetNumGPUDrivers();

        for (var i = 0; i < count; i++)
        {
            var driver = ((IntPtr<byte>)Unsafe_SDL_GetGPUDriver(i)).GetString();
            if (driver != null)
                result[i] = driver;
        }

        return result;
    }

    public static int GetCount()
    {
        return SDL_GetNumGPUDrivers();
    }

    public static bool SupportsProperties(
        SDL_PropertiesID props
    )
    {
        return SDL_GPUSupportsProperties(props);
    }

    public static unsafe bool SupportsShaderFormat(
        SDL_GPUShaderFormat format,
        ReadOnlySpan<char> driver
    )
    {
        if (driver.IsEmpty)
            return SDL_GPUSupportsShaderFormats(format, (byte*)null);

        using var driverStr = new UnmanagedString(driver);
        return SDL_GPUSupportsShaderFormats(format, driverStr);
    }

    public static unsafe void WaitForFences(
        this IntPtr<SDL_GPUDevice> device,
        bool waitAll,
        params ReadOnlySpan<IntPtr<SDL_GPUFence>> fences
    )
    {
        fixed (IntPtr<SDL_GPUFence>* fencesPtr = fences)
        {
            SDL_WaitForGPUFences(device, waitAll, (SDL_GPUFence**)fencesPtr, (uint)fences.Length)
                .AssertSdlSuccess();
        }
    }

    public static unsafe void WaitForIdle(
        this IntPtr<SDL_GPUDevice> device
    )
    {
        SDL_WaitForGPUIdle(device)
            .AssertSdlSuccess();
    }

    public static unsafe void WaitForSwapchain(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_Window> window)
    {
        SDL_WaitForGPUSwapchain(device, window)
            .AssertSdlSuccess();
    }
}