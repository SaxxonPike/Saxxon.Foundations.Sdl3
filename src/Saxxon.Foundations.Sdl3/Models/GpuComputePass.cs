using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_GPUComputePass"/>.
/// </summary>
[PublicAPI]
public static class GpuComputePass
{
    public static unsafe void BindPipeline(
        this IntPtr<SDL_GPUComputePass> pass,
        IntPtr<SDL_GPUComputePipeline> pipeline
    )
    {
        SDL_BindGPUComputePipeline(pass, pipeline);
    }

    public static unsafe void BindSamplers(
        this IntPtr<SDL_GPUComputePass> pass,
        uint firstSlot,
        params ReadOnlySpan<SDL_GPUTextureSamplerBinding> samplerBindings
    )
    {
        fixed (SDL_GPUTextureSamplerBinding* samplerBindingsPtr = samplerBindings)
        {
            SDL_BindGPUComputeSamplers(
                pass,
                firstSlot,
                samplerBindingsPtr,
                (uint)samplerBindings.Length
            );
        }
    }

    public static unsafe void BindStorageBuffers(
        this IntPtr<SDL_GPUComputePass> pass,
        uint firstSlot,
        params ReadOnlySpan<IntPtr<SDL_GPUBuffer>> buffers
    )
    {
        fixed (IntPtr<SDL_GPUBuffer>* buffersPtr = buffers)
        {
            SDL_BindGPUComputeStorageBuffers(
                pass,
                firstSlot,
                (SDL_GPUBuffer**)buffersPtr,
                (uint)buffers.Length
            );
        }
    }

    public static unsafe void BindStorageTextures(
        this IntPtr<SDL_GPUComputePass> pass,
        uint firstSlot,
        params ReadOnlySpan<IntPtr<SDL_GPUTexture>> textures
    )
    {
        fixed (IntPtr<SDL_GPUTexture>* texturesPtr = textures)
        {
            SDL_BindGPUComputeStorageTextures(
                pass,
                firstSlot,
                (SDL_GPUTexture**)texturesPtr,
                (uint)textures.Length
            );
        }
    }

    public static unsafe void Dispatch(
        this IntPtr<SDL_GPUComputePass> pass,
        uint groupCountX,
        uint groupCountY,
        uint groupCountZ
    )
    {
        SDL_DispatchGPUCompute(pass, groupCountX, groupCountY, groupCountZ);
    }

    public static unsafe void DispatchIndirect(
        this IntPtr<SDL_GPUComputePass> pass,
        IntPtr<SDL_GPUBuffer> buffer,
        uint offset
    )
    {
        SDL_DispatchGPUComputeIndirect(pass, buffer, offset);
    }

    public static unsafe void End(
        this IntPtr<SDL_GPUComputePass> pass
    )
    {
        SDL_EndGPUComputePass(pass);
    }
}