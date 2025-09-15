using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class GpuCommandBuffer
{
    public static unsafe (IntPtr<SDL_GPUTexture> Texture, uint Width, uint Height) AcquireSwapchainTexture(
        this IntPtr<SDL_GPUCommandBuffer> buffer,
        IntPtr<SDL_Window> window
    )
    {
        SDL_GPUTexture* texture;
        uint width, height;

        SDL_AcquireGPUSwapchainTexture(
            buffer,
            window,
            &texture,
            &width,
            &height
        ).AssertSdlSuccess();

        return ((IntPtr<SDL_GPUTexture>)texture, width, height);
    }

    public static unsafe IntPtr<SDL_GPUComputePass> BeginComputePass(
        this IntPtr<SDL_GPUCommandBuffer> buffer,
        Span<SDL_GPUStorageTextureReadWriteBinding> textureBindings,
        Span<SDL_GPUStorageBufferReadWriteBinding> bufferBindings
    )
    {
        fixed (SDL_GPUStorageTextureReadWriteBinding* textureBindingsPtr = textureBindings)
        fixed (SDL_GPUStorageBufferReadWriteBinding* bufferBindingsPtr = bufferBindings)
        {
            return SDL_BeginGPUComputePass(
                buffer,
                textureBindingsPtr,
                (uint)textureBindings.Length,
                bufferBindingsPtr,
                (uint)bufferBindings.Length
            );
        }
    }

    public static unsafe IntPtr<SDL_GPUCopyPass> BeginCopyPass(
        this IntPtr<SDL_GPUCommandBuffer> commandBuffer
    )
    {
        return SDL_BeginGPUCopyPass(commandBuffer);
    }

    public static unsafe IntPtr<SDL_GPURenderPass> BeginRenderPass(
        this IntPtr<SDL_GPUCommandBuffer> buffer,
        ReadOnlySpan<SDL_GPUColorTargetInfo> colorTargetInfos,
        SDL_GPUDepthStencilTargetInfo depthStencilTargetInfo
    )
    {
        fixed (SDL_GPUColorTargetInfo* colorTargetInfosPtr = colorTargetInfos)
        {
            return SDL_BeginGPURenderPass(
                buffer,
                colorTargetInfosPtr,
                (uint)colorTargetInfos.Length,
                &depthStencilTargetInfo
            );
        }
    }

    public static unsafe void BlitTexture(
        this IntPtr<SDL_GPUCommandBuffer> buffer,
        SDL_GPUBlitInfo blitInfo
    )
    {
        SDL_BlitGPUTexture(buffer, &blitInfo);
    }

    public static unsafe void Cancel(
        this IntPtr<SDL_GPUCommandBuffer> buffer
    )
    {
        SDL_CancelGPUCommandBuffer(buffer);
    }

    public static unsafe void GenerateMipmaps(
        this IntPtr<SDL_GPUCommandBuffer> buffer,
        IntPtr<SDL_GPUTexture> texture
    )
    {
        SDL_GenerateMipmapsForGPUTexture(buffer, texture);
    }

    public static unsafe void InsertDebugLabel(
        this IntPtr<SDL_GPUCommandBuffer> buffer,
        ReadOnlySpan<char> label
    )
    {
        using var labelStr = new Utf8Span(label);
        SDL_InsertGPUDebugLabel(buffer, labelStr);
    }

    public static unsafe void PopDebugGroup(
        this IntPtr<SDL_GPUCommandBuffer> buffer
    )
    {
        SDL_PopGPUDebugGroup(buffer);
    }

    public static unsafe void PushComputeUniformData(
        this IntPtr<SDL_GPUCommandBuffer> buffer,
        uint slotIndex,
        IntPtr data,
        uint dataLength
    )
    {
        SDL_PushGPUComputeUniformData(
            buffer,
            slotIndex,
            data,
            dataLength
        );
    }

    public static unsafe void PushDebugGroup(
        this IntPtr<SDL_GPUCommandBuffer> buffer,
        ReadOnlySpan<char> label
    )
    {
        using var labelStr = new Utf8Span(label);
        SDL_PushGPUDebugGroup(buffer, labelStr);
    }

    public static unsafe void PushFragmentUniformData(
        this IntPtr<SDL_GPUCommandBuffer> buffer,
        uint slotIndex,
        IntPtr data,
        uint dataLength
    )
    {
        SDL_PushGPUFragmentUniformData(
            buffer,
            slotIndex,
            data,
            dataLength
        );
    }

    public static unsafe void PushVertexUniformData(
        this IntPtr<SDL_GPUCommandBuffer> buffer,
        uint slotIndex,
        IntPtr data,
        uint dataLength
    )
    {
        SDL_PushGPUVertexUniformData(
            buffer,
            slotIndex,
            data,
            dataLength
        );
    }

    public static unsafe void Submit(
        this IntPtr<SDL_GPUCommandBuffer> buffer
    )
    {
        SDL_SubmitGPUCommandBuffer(buffer)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<SDL_GPUFence> SubmitAndAcquireFence(
        this IntPtr<SDL_GPUCommandBuffer> buffer
    )
    {
        return ((IntPtr<SDL_GPUFence>)SDL_SubmitGPUCommandBufferAndAcquireFence(buffer))
            .AssertSdlNotNull();
    }

    public static unsafe (IntPtr<SDL_GPUTexture> Texture, uint Width, uint Height) WaitAndAcquireSwapchainTexture(
        this IntPtr<SDL_GPUCommandBuffer> buffer,
        IntPtr<SDL_Window> window
    )
    {
        SDL_GPUTexture* texture;
        uint width, height;
        SDL_WaitAndAcquireGPUSwapchainTexture(buffer, window, &texture, &width, &height)
            .AssertSdlSuccess();

        return ((IntPtr<SDL_GPUTexture>)texture, width, height);
    }
}