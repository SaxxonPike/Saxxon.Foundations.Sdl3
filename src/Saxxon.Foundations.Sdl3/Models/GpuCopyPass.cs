using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_GPUCopyPass"/>.
/// </summary>
[PublicAPI]
public static class GpuCopyPass
{
    public static unsafe void CopyBufferToBuffer(
        this IntPtr<SDL_GPUCopyPass> copyPass,
        SDL_GPUBufferLocation source,
        SDL_GPUBufferLocation destination,
        uint size,
        bool cycle
    )
    {
        SDL_CopyGPUBufferToBuffer(
            copyPass,
            &source,
            &destination,
            size,
            cycle
        );
    }

    public static unsafe void CopyTextureToTexture(
        this IntPtr<SDL_GPUCopyPass> copyPass,
        SDL_GPUTextureLocation source,
        SDL_GPUTextureLocation destination,
        int width,
        int height,
        int depth,
        bool cycle
    )
    {
        SDL_CopyGPUTextureToTexture(
            copyPass,
            &source,
            &destination,
            (uint)width,
            (uint)height,
            (uint)depth,
            cycle
        );
    }

    public static unsafe void DownloadFromBuffer(
        this IntPtr<SDL_GPUCopyPass> pass,
        SDL_GPUBufferRegion source,
        SDL_GPUTransferBufferLocation destination
    )
    {
        SDL_DownloadFromGPUBuffer(pass, &source, &destination);
    }

    public static unsafe void DownloadFromTexture(
        this IntPtr<SDL_GPUCopyPass> pass,
        SDL_GPUTextureRegion source,
        SDL_GPUTextureTransferInfo destination
    )
    {
        SDL_DownloadFromGPUTexture(pass, &source, &destination);
    }

    public static unsafe void End(
        this IntPtr<SDL_GPUCopyPass> pass
    )
    {
        SDL_EndGPUCopyPass(pass);
    }

    public static unsafe void UploadToBuffer(
        this IntPtr<SDL_GPUCopyPass> pass,
        SDL_GPUTransferBufferLocation source,
        SDL_GPUBufferRegion destination,
        bool cycle
    )
    {
        SDL_UploadToGPUBuffer(pass, &source, &destination, cycle);
    }

    public static unsafe void UploadToTexture(
        this IntPtr<SDL_GPUCopyPass> pass,
        SDL_GPUTextureTransferInfo source,
        SDL_GPUTextureRegion destination,
        bool cycle
    )
    {
        SDL_UploadToGPUTexture(pass, &source, &destination, cycle);
    }
}