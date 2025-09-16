using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_GPUDevice"/>.
/// </summary>
[PublicAPI]
public static class GpuDevice
{
    public static unsafe IntPtr<SDL_GPUCommandBuffer> AcquireCommandBuffer(
        this IntPtr<SDL_GPUDevice> device
    )
    {
        return ((IntPtr<SDL_GPUCommandBuffer>)SDL_AcquireGPUCommandBuffer(device))
            .AssertSdlNotNull();
    }

    public static unsafe void ClaimWindow(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_Window> window
    )
    {
        SDL_ClaimWindowForGPUDevice(device, window)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<SDL_GPUDevice> Create(
        SDL_GPUShaderFormat format,
        bool debugMode,
        ReadOnlySpan<char> name
    )
    {
        using var nameStr = new UnmanagedString(name);
        return ((IntPtr<SDL_GPUDevice>)SDL_CreateGPUDevice(format, debugMode, nameStr))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_GPUDevice> CreateWithProperties(
        SDL_PropertiesID props
    )
    {
        return ((IntPtr<SDL_GPUDevice>)SDL_CreateGPUDeviceWithProperties(props))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_GPUBuffer> CreateBuffer(
        this IntPtr<SDL_GPUDevice> device,
        SDL_GPUBufferCreateInfo info
    )
    {
        return ((IntPtr<SDL_GPUBuffer>)SDL_CreateGPUBuffer(device, &info))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_GPUComputePipeline> CreateComputePipeline(
        this IntPtr<SDL_GPUDevice> device,
        SDL_GPUComputePipelineCreateInfo info
    )
    {
        return ((IntPtr<SDL_GPUComputePipeline>)SDL_CreateGPUComputePipeline(device, &info))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_GPUGraphicsPipeline> CreateGraphicsPipeline(
        this IntPtr<SDL_GPUDevice> device,
        SDL_GPUGraphicsPipelineCreateInfo info
    )
    {
        return ((IntPtr<SDL_GPUGraphicsPipeline>)SDL_CreateGPUGraphicsPipeline(device, &info))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_GPUSampler> CreateSampler(
        this IntPtr<SDL_GPUDevice> device,
        SDL_GPUSamplerCreateInfo info
    )
    {
        return ((IntPtr<SDL_GPUSampler>)SDL_CreateGPUSampler(device, &info))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_GPUSampler> CreateShader(
        this IntPtr<SDL_GPUDevice> device,
        SDL_GPUShaderCreateInfo info
    )
    {
        return ((IntPtr<SDL_GPUSampler>)SDL_CreateGPUShader(device, &info))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_GPUSampler> CreateTexture(
        this IntPtr<SDL_GPUDevice> device,
        SDL_GPUTextureCreateInfo info
    )
    {
        return ((IntPtr<SDL_GPUSampler>)SDL_CreateGPUTexture(device, &info))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_GPUSampler> CreateTransferBuffer(
        this IntPtr<SDL_GPUDevice> device,
        SDL_GPUTransferBufferCreateInfo info
    )
    {
        return ((IntPtr<SDL_GPUSampler>)SDL_CreateGPUTransferBuffer(device, &info))
            .AssertSdlNotNull();
    }

    public static unsafe void Destroy(
        this IntPtr<SDL_GPUDevice> device
    )
    {
        SDL_DestroyGPUDevice(device);
    }

    public static unsafe string GetDriver(
        this IntPtr<SDL_GPUDevice> device
    )
    {
        IntPtr<byte> result = Unsafe_SDL_GetGPUDeviceDriver(device);
        return result.GetString() ?? throw new SdlException();
    }

    public static unsafe SDL_PropertiesID GetProperties(
        this IntPtr<SDL_GPUDevice> device
    )
    {
        var result = SDL_GetGPUDeviceProperties(device);
        return result == 0 ? throw new SdlException() : result;
    }

    public static unsafe SDL_GPUShaderFormat GetShaderFormats(
        this IntPtr<SDL_GPUDevice> device
    )
    {
        return SDL_GetGPUShaderFormats(device);
    }

    public static unsafe SDL_GPUTextureFormat GetSwapchainTextureFormat(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_Window> window
    )
    {
        return SDL_GetGPUSwapchainTextureFormat(device, window);
    }

    public static unsafe IntPtr MapTransferBuffer(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUTransferBuffer> transferBuffer,
        bool cycle
    )
    {
        return SDL_MapGPUTransferBuffer(
            device,
            transferBuffer,
            cycle
        ).AssertSdlNotNull();
    }

    public static unsafe SDL_GPUFence? QueryFence(
        this IntPtr<SDL_GPUDevice> device
    )
    {
        SDL_GPUFence fence;
        return SDL_QueryGPUFence(device, &fence) ? fence : null;
    }

    public static unsafe void ReleaseBuffer(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUBuffer> buffer
    )
    {
        SDL_ReleaseGPUBuffer(device, buffer);
    }

    public static unsafe void ReleaseComputePipeline(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUComputePipeline> pipeline
    )
    {
        SDL_ReleaseGPUComputePipeline(device, pipeline);
    }

    public static unsafe void ReleaseFence(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUFence> fence
    )
    {
        SDL_ReleaseGPUFence(device, fence);
    }

    public static unsafe void ReleaseGraphicsPipeline(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUGraphicsPipeline> pipeline
    )
    {
        SDL_ReleaseGPUGraphicsPipeline(device, pipeline);
    }

    public static unsafe void ReleaseSampler(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUSampler> sampler
    )
    {
        SDL_ReleaseGPUSampler(device, sampler);
    }

    public static unsafe void ReleaseShader(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUShader> shader
    )
    {
        SDL_ReleaseGPUShader(device, shader);
    }

    public static unsafe void ReleaseTexture(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUTexture> texture
    )
    {
        SDL_ReleaseGPUTexture(device, texture);
    }

    public static unsafe void ReleaseTransferBuffer(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUTransferBuffer> buffer
    )
    {
        SDL_ReleaseGPUTransferBuffer(device, buffer);
    }

    public static unsafe void ReleaseWindow(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_Window> window
    )
    {
        SDL_ReleaseWindowFromGPUDevice(device, window);
    }

    public static unsafe void SetAllowedFramesInFlight(
        this IntPtr<SDL_GPUDevice> device,
        uint frames
    )
    {
        SDL_SetGPUAllowedFramesInFlight(device, frames)
            .AssertSdlSuccess();
    }

    public static unsafe void SetBufferName(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUBuffer> buffer,
        ReadOnlySpan<char> name
    )
    {
        using var nameStr = new UnmanagedString(name);
        SDL_SetGPUBufferName(device, buffer, nameStr);
    }

    public static unsafe void SetSwapchainParameters(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_Window> window,
        SDL_GPUSwapchainComposition composition,
        SDL_GPUPresentMode presentMode
    )
    {
        SDL_SetGPUSwapchainParameters(device, window, composition, presentMode)
            .AssertSdlSuccess();
    }

    public static unsafe void SetTextureName(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUTexture> texture,
        ReadOnlySpan<char> name
    )
    {
        using var nameStr = new UnmanagedString(name);
        SDL_SetGPUTextureName(device, texture, nameStr);
    }

    public static unsafe bool SupportsPresentMode(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_Window> window,
        SDL_GPUPresentMode presentMode
    )
    {
        return SDL_WindowSupportsGPUPresentMode(device, window, presentMode);
    }

    public static unsafe bool SupportsSwapchainComposition(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_Window> window,
        SDL_GPUSwapchainComposition composition
    )
    {
        return SDL_WindowSupportsGPUSwapchainComposition(device, window, composition);
    }

    public static unsafe bool SupportsTextureFormat(
        this IntPtr<SDL_GPUDevice> device,
        SDL_GPUTextureFormat format,
        SDL_GPUTextureType type,
        SDL_GPUTextureUsageFlags usage
    )
    {
        return SDL_GPUTextureSupportsFormat(
            device,
            format,
            type,
            usage
        );
    }

    public static unsafe bool SupportsTextureSampleCount(
        this IntPtr<SDL_GPUDevice> device,
        SDL_GPUTextureFormat format,
        SDL_GPUSampleCount sampleCount
    )
    {
        return SDL_GPUTextureSupportsSampleCount(
            device,
            format,
            sampleCount
        );
    }

    public static unsafe void UnmapTransferBuffer(
        this IntPtr<SDL_GPUDevice> device,
        IntPtr<SDL_GPUTransferBuffer> buffer
    )
    {
        SDL_UnmapGPUTransferBuffer(device, buffer);
    }
}