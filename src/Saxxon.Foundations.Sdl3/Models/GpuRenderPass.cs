using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class GpuRenderPass
{
    public static unsafe void BindFragmentSamplers(
        this IntPtr<SDL_GPURenderPass> pass,
        uint firstSlot,
        params ReadOnlySpan<SDL_GPUTextureSamplerBinding> samplerBindings
    )
    {
        fixed (SDL_GPUTextureSamplerBinding* samplerBindingsPtr = samplerBindings)
        {
            SDL_BindGPUFragmentSamplers(
                pass,
                firstSlot,
                samplerBindingsPtr,
                (uint)samplerBindings.Length
            );
        }
    }

    public static unsafe void BindFragmentStorageBuffers(
        this IntPtr<SDL_GPURenderPass> pass,
        uint firstSlot,
        params ReadOnlySpan<IntPtr<SDL_GPUBuffer>> buffers
    )
    {
        fixed (IntPtr<SDL_GPUBuffer>* buffersPtr = buffers)
        {
            SDL_BindGPUFragmentStorageBuffers(
                pass,
                firstSlot,
                (SDL_GPUBuffer**)buffersPtr,
                (uint)buffers.Length
            );
        }
    }

    public static unsafe void BindFragmentStorageTextures(
        this IntPtr<SDL_GPURenderPass> pass,
        uint firstSlot,
        params ReadOnlySpan<IntPtr<SDL_GPUTexture>> textures
    )
    {
        fixed (IntPtr<SDL_GPUTexture>* texturesPtr = textures)
        {
            SDL_BindGPUFragmentStorageTextures(
                pass,
                firstSlot,
                (SDL_GPUTexture**)texturesPtr,
                (uint)textures.Length
            );
        }
    }

    public static unsafe void BindIndexBuffer(
        this IntPtr<SDL_GPURenderPass> pass,
        SDL_GPUBufferBinding binding,
        SDL_GPUIndexElementSize elementSize
    )
    {
        SDL_BindGPUIndexBuffer(pass, &binding, elementSize);
    }

    public static unsafe void BindPipeline(
        this IntPtr<SDL_GPURenderPass> pass,
        IntPtr<SDL_GPUGraphicsPipeline> pipeline
    )
    {
        SDL_BindGPUGraphicsPipeline(pass, pipeline);
    }

    public static unsafe void BindVertexBuffers(
        this IntPtr<SDL_GPURenderPass> pass,
        uint firstSlot,
        params ReadOnlySpan<SDL_GPUBufferBinding> bindings
    )
    {
        fixed (SDL_GPUBufferBinding* bindingsPtr = bindings)
        {
            SDL_BindGPUVertexBuffers(
                pass,
                firstSlot,
                bindingsPtr,
                (uint)bindings.Length
            );
        }
    }

    public static unsafe void BindVertexSamplers(
        this IntPtr<SDL_GPURenderPass> pass,
        uint firstSlot,
        params ReadOnlySpan<SDL_GPUTextureSamplerBinding> samplerBindings
    )
    {
        fixed (SDL_GPUTextureSamplerBinding* samplerBindingsPtr = samplerBindings)
        {
            SDL_BindGPUVertexSamplers(
                pass,
                firstSlot,
                samplerBindingsPtr,
                (uint)samplerBindings.Length
            );
        }
    }

    public static unsafe void BindVertexStorageBuffers(
        this IntPtr<SDL_GPURenderPass> pass,
        uint firstSlot,
        params ReadOnlySpan<IntPtr<SDL_GPUBuffer>> buffers
    )
    {
        fixed (IntPtr<SDL_GPUBuffer>* buffersPtr = buffers)
        {
            SDL_BindGPUVertexStorageBuffers(
                pass,
                firstSlot,
                (SDL_GPUBuffer**)buffersPtr,
                (uint)buffers.Length
            );
        }
    }

    public static unsafe void BindVertexStorageTextures(
        this IntPtr<SDL_GPURenderPass> pass,
        uint firstSlot,
        params ReadOnlySpan<IntPtr<SDL_GPUTexture>> textures
    )
    {
        fixed (IntPtr<SDL_GPUTexture>* texturesPtr = textures)
        {
            SDL_BindGPUVertexStorageTextures(
                pass,
                firstSlot,
                (SDL_GPUTexture**)texturesPtr,
                (uint)textures.Length
            );
        }
    }

    public static unsafe void DrawIndexedPrimitives(
        this IntPtr<SDL_GPURenderPass> pass,
        uint numIndices,
        uint numInstances,
        uint firstIndex,
        int vertexOffset,
        uint firstInstance
    )
    {
        SDL_DrawGPUIndexedPrimitives(
            pass,
            numIndices,
            numInstances,
            firstIndex,
            vertexOffset,
            firstInstance
        );
    }

    public static unsafe void DrawIndexedPrimitivesIndirect(
        this IntPtr<SDL_GPURenderPass> pass,
        IntPtr<SDL_GPUBuffer> buffer,
        uint offset,
        uint drawCount
    )
    {
        SDL_DrawGPUIndexedPrimitivesIndirect(
            pass,
            buffer,
            offset,
            drawCount
        );
    }

    public static unsafe void DrawPrimitives(
        this IntPtr<SDL_GPURenderPass> pass,
        uint numVertices,
        uint numInstances,
        uint firstVertex,
        uint firstInstance
    )
    {
        SDL_DrawGPUPrimitives(
            pass,
            numVertices,
            numInstances,
            firstVertex,
            firstInstance
        );
    }

    public static unsafe void DrawPrimitivesIndirect(
        this IntPtr<SDL_GPURenderPass> pass,
        IntPtr<SDL_GPUBuffer> buffer,
        uint offset,
        uint drawCount
    )
    {
        SDL_DrawGPUPrimitivesIndirect(
            pass,
            buffer,
            offset,
            drawCount
        );
    }

    public static unsafe void End(
        this IntPtr<SDL_GPURenderPass> pass
    )
    {
        SDL_EndGPURenderPass(pass);
    }

    public static unsafe void SetBlendConstants(
        this IntPtr<SDL_GPURenderPass> pass,
        SDL_FColor blendConstants
    )
    {
        SDL_SetGPUBlendConstants(pass, blendConstants);
    }

    public static unsafe void SetScissor(
        this IntPtr<SDL_GPURenderPass> pass,
        SDL_Rect scissor
    )
    {
        SDL_SetGPUScissor(pass, &scissor);
    }

    public static unsafe void SetStencilReference(
        this IntPtr<SDL_GPURenderPass> pass,
        byte reference
    )
    {
        SDL_SetGPUStencilReference(pass, reference);
    }

    public static unsafe void SetViewport(
        this IntPtr<SDL_GPURenderPass> pass,
        SDL_GPUViewport viewport
    )
    {
        SDL_SetGPUViewport(pass, &viewport);
    }
}