using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_GPUTextureFormat"/>.
/// </summary>
[PublicAPI]
public static class GpuTextureFormat
{
    public static uint CalculateSize(
        this SDL_GPUTextureFormat format,
        uint width,
        uint height,
        uint depthOrLayerCount
    )
    {
        return SDL_CalculateGPUTextureFormatSize(format, width, height, depthOrLayerCount);
    }

    // TODO: 3.4.0
    // public static SDL_GPUTextureFormat GetFromPixelFormat(
    //     SDL_PixelFormat pixelFormat
    // )
    // {
    //     return SDL_GetGPUTextureFormatFromPixelFormat(pixelFormat);
    // }

    // TODO: 3.4.0
    // public static SDL_PixelFormat GetPixelFormat(
    //     this SDL_GPUTextureFormat format
    // )
    // {
    //     return SDL_GetPixelFormatFromGPUTextureFormat(format);
    // }

    public static uint GetTexelBlockSize(
        this SDL_GPUTextureFormat format
    )
    {
        return SDL_GPUTextureFormatTexelBlockSize(format);
    }
}