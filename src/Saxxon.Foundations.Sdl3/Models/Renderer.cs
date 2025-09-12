using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Renderer"/>.
/// </summary>
[PublicAPI]
public static class Renderer
{
    public static unsafe void Clear(this IntPtr<SDL_Renderer> ptr)
    {
        SDL_RenderClear(ptr)
            .AssertSdlSuccess();
    }

    public static unsafe bool IsClipEnabled(this IntPtr<SDL_Renderer> ptr)
    {
        return SDL_RenderClipEnabled(ptr);
    }

    public static unsafe void DebugText(this IntPtr<SDL_Renderer> ptr, float x, float y, ReadOnlySpan<char> text)
    {
        using var bytes = new Utf8Span(text);
        SDL_RenderDebugText(ptr, x, y, bytes)
            .AssertSdlSuccess();
    }

    [StringFormatMethod(nameof(format))]
    public static unsafe void DebugText(
        this IntPtr<SDL_Renderer> ptr,
        float x,
        float y,
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        using var bytes = Utf8Span.Format(format, args);
        SDL_RenderDebugText(ptr, x, y, bytes)
            .AssertSdlSuccess();
    }

    public static unsafe void FillRect(this IntPtr<SDL_Renderer> ptr, SDL_FRect rect)
    {
        SDL_RenderFillRect(ptr, &rect)
            .AssertSdlSuccess();
    }

    public static unsafe void FillRects(this IntPtr<SDL_Renderer> ptr, ReadOnlySpan<SDL_FRect> rects)
    {
        fixed (SDL_FRect* data = rects)
        {
            SDL_RenderFillRects(ptr, data, rects.Length)
                .AssertSdlSuccess();
        }
    }

    public static unsafe void Geometry(
        this IntPtr<SDL_Renderer> ptr,
        IntPtr<SDL_Texture> texture,
        ReadOnlySpan<SDL_Vertex> vertices,
        ReadOnlySpan<int> indices
    )
    {
        fixed (SDL_Vertex* data = vertices)
        fixed (int* indexData = indices)
        {
            SDL_RenderGeometry(
                ptr,
                texture,
                data,
                vertices.Length,
                indexData,
                indices.Length
            ).AssertSdlSuccess();
        }
    }

    public static unsafe void GeometryRaw<TIndex, TVertex>(
        this IntPtr<SDL_Renderer> ptr,
        IntPtr<SDL_Texture> texture,
        ReadOnlySpan<TVertex> vertices,
        int xyOffset,
        int colorOffset,
        int uvOffset,
        ReadOnlySpan<TIndex> indices
    ) where TIndex : unmanaged where TVertex : unmanaged
    {
        fixed (TVertex* vertexData = vertices)
        fixed (void* indexData = indices)
        {
            SDL_RenderGeometryRaw(
                ptr,
                texture,
                (float*)((byte*)vertexData + xyOffset),
                sizeof(TVertex),
                (SDL_FColor*)((byte*)vertexData + colorOffset),
                sizeof(TVertex),
                (float*)((byte*)vertexData + uvOffset),
                sizeof(TVertex),
                vertices.Length,
                (IntPtr)indexData,
                indices.Length * sizeof(TIndex),
                sizeof(TIndex)
            ).AssertSdlSuccess();
        }
    }

    public static unsafe void GeometryRaw<TIndex>(
        this IntPtr<SDL_Renderer> ptr,
        IntPtr<SDL_Texture> texture,
        ReadOnlySpan<SDL_FPoint> xy,
        int xyStride,
        ReadOnlySpan<SDL_FColor> color,
        int colorStride,
        ReadOnlySpan<SDL_FPoint> uv,
        int uvStride,
        int numVertices,
        ReadOnlySpan<TIndex> indices
    ) where TIndex : unmanaged
    {
        fixed (void* xyData = xy)
        fixed (void* colorData = color)
        fixed (void* uvData = uv)
        fixed (void* indexData = indices)
        {
            SDL_RenderGeometryRaw(
                ptr,
                texture,
                (float*)xyData,
                xyStride,
                (SDL_FColor*)colorData,
                colorStride,
                (float*)uvData,
                uvStride,
                numVertices,
                (IntPtr)indexData,
                indices.Length * sizeof(TIndex),
                sizeof(TIndex)
            ).AssertSdlSuccess();
        }
    }

    public static unsafe void Line(this IntPtr<SDL_Renderer> ptr, float x1, float y1, float x2, float y2)
    {
        SDL_RenderLine(ptr, x1, y1, x2, y2)
            .AssertSdlSuccess();
    }

    public static unsafe void Lines(this IntPtr<SDL_Renderer> ptr, ReadOnlySpan<SDL_FPoint> points)
    {
        fixed (SDL_FPoint* data = points)
        {
            SDL_RenderLines(ptr, data, points.Length);
        }
    }

    public static unsafe void Point(this IntPtr<SDL_Renderer> ptr, float x, float y)
    {
        SDL_RenderPoint(ptr, x, y);
    }

    public static unsafe void Points(this IntPtr<SDL_Renderer> ptr, ReadOnlySpan<SDL_FPoint> points)
    {
        fixed (SDL_FPoint* data = points)
        {
            SDL_RenderPoints(ptr, data, points.Length);
        }
    }

    public static unsafe void Rect(this IntPtr<SDL_Renderer> ptr, SDL_FRect rect)
    {
        SDL_RenderRect(ptr, &rect)
            .AssertSdlSuccess();
    }

    public static unsafe void Rects(this IntPtr<SDL_Renderer> ptr, ReadOnlySpan<SDL_FRect> rects)
    {
        fixed (SDL_FRect* data = rects)
        {
            SDL_RenderRects(ptr, data, rects.Length)
                .AssertSdlSuccess();
        }
    }

    public static unsafe void Texture(
        this IntPtr<SDL_Renderer> ptr,
        IntPtr<SDL_Texture> texture,
        SDL_FRect? srcRect,
        SDL_FRect? dstRect)
    {
        SDL_RenderTexture(
            ptr,
            texture,
            srcRect is { } sr ? &sr : null,
            dstRect is { } dr ? &dr : null
        ).AssertSdlSuccess();
    }

    public static unsafe void Texture9Grid(
        this IntPtr<SDL_Renderer> ptr,
        IntPtr<SDL_Texture> texture,
        SDL_FRect? srcRect,
        float leftWidth,
        float rightWidth,
        float topHeight,
        float bottomHeight,
        float scale,
        SDL_FRect? dstRect)
    {
        SDL_RenderTexture9Grid(
            ptr,
            texture,
            srcRect is { } sr ? &sr : null,
            leftWidth,
            rightWidth,
            topHeight,
            bottomHeight,
            scale,
            dstRect is { } dr ? &dr : null
        ).AssertSdlSuccess();
    }

    public static unsafe void Texture9GridTiled(
        this IntPtr<SDL_Renderer> ptr,
        IntPtr<SDL_Texture> texture,
        SDL_FRect? srcRect,
        float leftWidth,
        float rightWidth,
        float topHeight,
        float bottomHeight,
        float scale,
        SDL_FRect? dstRect,
        float tileScale)
    {
        SDL_RenderTexture9GridTiled(
            ptr,
            texture,
            srcRect is { } sr ? &sr : null,
            leftWidth,
            rightWidth,
            topHeight,
            bottomHeight,
            scale,
            dstRect is { } dr ? &dr : null,
            tileScale
        ).AssertSdlSuccess();
    }

    public static unsafe void TextureAffine(
        this IntPtr<SDL_Renderer> ptr,
        IntPtr<SDL_Texture> texture,
        SDL_FRect? srcRect,
        SDL_FPoint? origin,
        SDL_FPoint? right,
        SDL_FPoint? down)
    {
        SDL_RenderTextureAffine(
            ptr,
            texture,
            srcRect is { } sr ? &sr : null,
            origin is { } o ? &o : null,
            right is { } r ? &r : null,
            down is { } d ? &d : null
        ).AssertSdlSuccess();
    }

    public static unsafe void TextureRotated(
        this IntPtr<SDL_Renderer> ptr,
        IntPtr<SDL_Texture> texture,
        SDL_FRect? srcRect,
        SDL_FRect? dstRect,
        float angle,
        SDL_FPoint? center,
        SDL_FlipMode flip)
    {
        SDL_RenderTextureRotated(
            ptr,
            texture,
            srcRect is { } sr ? &sr : null,
            dstRect is { } dr ? &dr : null,
            angle,
            center is { } c ? &c : null,
            flip
        ).AssertSdlSuccess();
    }

    public static unsafe void TextureTiled(
        this IntPtr<SDL_Renderer> ptr,
        IntPtr<SDL_Texture> texture,
        SDL_FRect? srcRect,
        float scale,
        SDL_FRect? dstRect)
    {
        SDL_RenderTextureTiled(
            ptr,
            texture,
            srcRect is { } sr ? &sr : null,
            scale,
            dstRect is { } dr ? &dr : null
        ).AssertSdlSuccess();
    }

    public static unsafe bool IsViewportSet(this IntPtr<SDL_Renderer> ptr)
    {
        return SDL_RenderViewportSet(ptr);
    }

    public static unsafe void SetClipRect(this IntPtr<SDL_Renderer> ptr, SDL_Rect? rect)
    {
        SDL_SetRenderClipRect(ptr, rect is { } r ? &r : null)
            .AssertSdlSuccess();
    }

    public static unsafe void SetColorScale(this IntPtr<SDL_Renderer> ptr, float scale)
    {
        SDL_SetRenderColorScale(ptr, scale)
            .AssertSdlSuccess();
    }

    public static unsafe void SetBlendMode(this IntPtr<SDL_Renderer> ptr, SDL_BlendMode blendMode)
    {
        SDL_SetRenderDrawBlendMode(ptr, blendMode)
            .AssertSdlSuccess();
    }

    public static unsafe void SetColor(this IntPtr<SDL_Renderer> ptr, byte r, byte g, byte b, byte a)
    {
        SDL_SetRenderDrawColor(ptr, r, g, b, a)
            .AssertSdlSuccess();
    }

    public static unsafe void SetColorFloat(this IntPtr<SDL_Renderer> ptr, float r, float g, float b, float a)
    {
        SDL_SetRenderDrawColorFloat(ptr, r, g, b, a)
            .AssertSdlSuccess();
    }

    public static unsafe void SetScale(this IntPtr<SDL_Renderer> ptr, float x, float y)
    {
        SDL_SetRenderScale(ptr, x, y)
            .AssertSdlSuccess();
    }

    public static unsafe void SetTarget(this IntPtr<SDL_Renderer> ptr, IntPtr<SDL_Texture> texture)
    {
        SDL_SetRenderTarget(ptr, texture)
            .AssertSdlSuccess();
    }

    public static unsafe void SetViewport(this IntPtr<SDL_Renderer> ptr, SDL_Rect? rect)
    {
        SDL_SetRenderViewport(ptr, rect is { } r ? &r : null)
            .AssertSdlSuccess();
    }

    public static unsafe void SetVSync(this IntPtr<SDL_Renderer> ptr, int vsync)
    {
        SDL_SetRenderVSync(ptr, vsync);
    }

    public static unsafe (int W, int H) GetCurrentOutputSize(this IntPtr<SDL_Renderer> ptr)
    {
        int w, h;
        SDL_GetCurrentRenderOutputSize(ptr, &w, &h)
            .AssertSdlSuccess();
        return (w, h);
    }

    public static unsafe SDL_Rect? GetClipRect(this IntPtr<SDL_Renderer> ptr)
    {
        SDL_Rect result;
        SDL_GetRenderClipRect(ptr, &result)
            .AssertSdlSuccess();
        return result is { w: 0, h: 0 } ? null : *&result;
    }

    public static unsafe float GetColorScale(this IntPtr<SDL_Renderer> ptr)
    {
        float result;
        SDL_GetRenderColorScale(ptr, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe SDL_BlendMode GetBlendMode(this IntPtr<SDL_Renderer> ptr)
    {
        SDL_BlendMode result;
        SDL_GetRenderDrawBlendMode(ptr, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe SDL_Color GetColor(this IntPtr<SDL_Renderer> ptr)
    {
        SDL_Color result;
        SDL_GetRenderDrawColor(ptr, &result.r, &result.g, &result.b, &result.a)
            .AssertSdlSuccess();

        return new SDL_Color
        {
            r = result.r, g = result.g, b = result.b, a = result.a
        };
    }

    public static unsafe SDL_FColor GetColorFloat(this IntPtr<SDL_Renderer> ptr)
    {
        SDL_FColor result;
        SDL_GetRenderDrawColorFloat(ptr, &result.r, &result.g, &result.b, &result.a)
            .AssertSdlSuccess();
        return *&result;
    }

    public static unsafe SDL_PropertiesID GetProperties(this IntPtr<SDL_Renderer> ptr)
    {
        return SDL_GetRendererProperties(ptr);
    }

    public static unsafe (int W, int H) GetOutputSize(this IntPtr<SDL_Renderer> ptr)
    {
        int w, h;
        SDL_GetRenderOutputSize(ptr, &w, &h)
            .AssertSdlSuccess();
        return (w, h);
    }

    public static unsafe SDL_Rect GetSafeArea(this IntPtr<SDL_Renderer> ptr)
    {
        SDL_Rect result;
        SDL_GetRenderSafeArea(ptr, &result)
            .AssertSdlSuccess();
        return *&result;
    }

    public static unsafe float GetScale(this IntPtr<SDL_Renderer> ptr)
    {
        float result;
        SDL_GetRenderScale(ptr, &result, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe IntPtr<SDL_Texture> GetTarget(this IntPtr<SDL_Renderer> ptr)
    {
        var result = SDL_GetRenderTarget(ptr);
        return result != null ? result : IntPtr<SDL_Texture>.Zero;
    }

    public static unsafe SDL_Rect GetViewport(this IntPtr<SDL_Renderer> ptr)
    {
        SDL_Rect result;
        SDL_GetRenderViewport(ptr, &result)
            .AssertSdlSuccess();
        return *&result;
    }

    public static unsafe int GetVSync(this IntPtr<SDL_Renderer> ptr)
    {
        int result;
        SDL_GetRenderVSync(ptr, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe IntPtr<SDL_Window> GetWindow(this IntPtr<SDL_Renderer> ptr)
    {
        return SDL_GetRenderWindow(ptr);
    }

    public static unsafe void SetLogicalPresentation(
        this IntPtr<SDL_Renderer> ptr,
        int w,
        int h,
        SDL_RendererLogicalPresentation mode
    )
    {
        SDL_SetRenderLogicalPresentation(ptr, w, h, mode)
            .AssertSdlSuccess();
    }

    public static unsafe void Present(
        this IntPtr<SDL_Renderer> ptr
    )
    {
        SDL_RenderPresent(ptr)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<SDL_Texture> LoadTextureTypedIo(
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_IOStream> src,
        bool closeIo,
        ReadOnlySpan<char> type
    )
    {
        using var typeStr = new Utf8Span(type);
        return ((IntPtr<SDL_Texture>)IMG_LoadTextureTyped_IO(renderer, src, closeIo, typeStr))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Texture> LoadTexture(
        this IntPtr<SDL_Renderer> renderer,
        ReadOnlySpan<char> file
    )
    {
        using var fileStr = new Utf8Span(file);
        return ((IntPtr<SDL_Texture>)IMG_LoadTexture(renderer, fileStr))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Texture> LoadTextureIo(
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_IOStream> src,
        bool closeIo
    )
    {
        return ((IntPtr<SDL_Texture>)IMG_LoadTexture_IO(renderer, src, closeIo))
            .AssertSdlNotNull();
    }

    public static unsafe void SetGpuRenderState(
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_GPURenderState> state
    )
    {
        SDL_SetRenderGPUState(renderer, state)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<SDL_GPURenderState> CreateGpuRenderState(
        this IntPtr<SDL_Renderer> renderer,
        SDL_GPURenderStateCreateInfo createInfo
    )
    {
        return ((IntPtr<SDL_GPURenderState>)SDL_CreateGPURenderState(renderer, &createInfo))
            .AssertSdlNotNull();
    }

    public static unsafe SDL_ScaleMode GetDefaultTextureScaleMode(
        this IntPtr<SDL_Renderer> renderer
    )
    {
        SDL_ScaleMode result;
        SDL_GetDefaultTextureScaleMode(renderer, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe void SetDefaultTextureScaleMode(
        this IntPtr<SDL_Renderer> renderer,
        SDL_ScaleMode scaleMode
    )
    {
        SDL_SetDefaultTextureScaleMode(renderer, scaleMode)
            .AssertSdlSuccess();
    }

    public static unsafe void AddVulkanSemaphores(
        this IntPtr<SDL_Renderer> renderer,
        uint waitStageMask,
        long waitSemaphore,
        long signalSemaphore
    )
    {
        SDL_AddVulkanRenderSemaphores(renderer, waitStageMask, waitSemaphore, signalSemaphore)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr GetMetalCommandEncoder(
        this IntPtr<SDL_Renderer> renderer
    )
    {
        return SDL_GetRenderMetalCommandEncoder(renderer);
    }

    public static unsafe IntPtr GetMetalLayer(
        this IntPtr<SDL_Renderer> renderer
    )
    {
        return SDL_GetRenderMetalLayer(renderer);
    }

    public static unsafe void Flush(
        this IntPtr<SDL_Renderer> renderer
    )
    {
        SDL_FlushRenderer(renderer)
            .AssertSdlSuccess();
    }

    public static unsafe void Destroy(
        this IntPtr<SDL_Renderer> renderer
    )
    {
        SDL_DestroyRenderer(renderer);
    }

    public static unsafe IntPtr<SDL_Surface> ReadPixels(
        this IntPtr<SDL_Renderer> renderer,
        SDL_Rect? rect
    )
    {
        return ((IntPtr<SDL_Surface>)SDL_RenderReadPixels(renderer, rect is { } sr ? &sr : null))
            .AssertSdlNotNull();
    }

    public static unsafe (SDL_TextureAddressMode UMode, SDL_TextureAddressMode VMode) GetTextureAddressMode(
        this IntPtr<SDL_Renderer> renderer
    )
    {
        SDL_TextureAddressMode uMode, vMode;
        SDL_GetRenderTextureAddressMode(renderer, &uMode, &vMode)
            .AssertSdlSuccess();
        return (uMode, vMode);
    }

    public static unsafe void SetTextureAddressMode(
        this IntPtr<SDL_Renderer> renderer,
        SDL_TextureAddressMode uMode,
        SDL_TextureAddressMode vMode
    )
    {
        SDL_SetRenderTextureAddressMode(renderer, uMode, vMode)
            .AssertSdlSuccess();
    }

    public static unsafe void ConvertEventCoordinates(
        this IntPtr<SDL_Renderer> renderer,
        ref SDL_Event @event
    )
    {
        SDL_ConvertEventToRenderCoordinates(renderer, (SDL_Event*)Unsafe.AsPointer(ref @event))
            .AssertSdlSuccess();
    }

    public static unsafe (float X, float Y) CoordinatesToWindow(
        this IntPtr<SDL_Renderer> renderer,
        float x,
        float y
    )
    {
        float windowX, windowY;
        SDL_RenderCoordinatesToWindow(renderer, x, y, &windowX, &windowY)
            .AssertSdlSuccess();
        return (windowX, windowY);
    }

    public static unsafe (float X, float Y) CoordinatesFromWindow(
        this IntPtr<SDL_Renderer> renderer,
        float windowX,
        float windowY
    )
    {
        float x, y;
        SDL_RenderCoordinatesToWindow(renderer, windowX, windowY, &x, &y)
            .AssertSdlSuccess();
        return (x, y);
    }

    public static unsafe SDL_FRect GetLogicalPresentationRect(
        this IntPtr<SDL_Renderer> renderer
    )
    {
        SDL_FRect rect;
        SDL_GetRenderLogicalPresentationRect(renderer, &rect)
            .AssertSdlSuccess();
        return rect;
    }

    public static unsafe (int W, int H, SDL_RendererLogicalPresentation Mode) GetLogicalPresentation(
        this IntPtr<SDL_Renderer> renderer
    )
    {
        int w, h;
        SDL_RendererLogicalPresentation mode;
        SDL_GetRenderLogicalPresentation(renderer, &w, &h, &mode)
            .AssertSdlSuccess();
        return (w, h, mode);
    }

    public static unsafe string GetName(
        this IntPtr<SDL_Renderer> renderer
    )
    {
        return Ptr.ToUtf8String(Unsafe_SDL_GetRendererName(renderer)) ??
               throw new SdlException();
    }

    public static unsafe IntPtr<SDL_Renderer> CreateSoftware(
        IntPtr<SDL_Surface> surface
    )
    {
        return ((IntPtr<SDL_Renderer>)SDL_CreateSoftwareRenderer(surface))
            .AssertSdlNotNull();
    }

    public static unsafe (IntPtr<SDL_Renderer> Renderer, IntPtr<SDL_GPUDevice> Device) CreateGpu(
        IntPtr<SDL_Window> window,
        SDL_GPUShaderFormat formatFlags
    )
    {
        SDL_GPUDevice* device;
        var renderer = ((IntPtr<SDL_Renderer>)SDL_CreateGPURenderer(window, formatFlags, &device))
            .AssertSdlNotNull();
        // ReSharper disable once RedundantCast
        return (renderer, (IntPtr<SDL_GPUDevice>)device);
    }

    public static unsafe IntPtr<SDL_Renderer> CreateWithProperties(SDL_PropertiesID props)
    {
        return ((IntPtr<SDL_Renderer>)SDL_CreateRendererWithProperties(props))
            .AssertSdlNotNull();
    }

    public static unsafe string? GetDriverName(int index)
    {
        return Ptr.ToUtf8String(Unsafe_SDL_GetRenderDriver(index));
    }

    public static unsafe List<string> GetDriverNames()
    {
        var count = SDL_GetNumRenderDrivers();
        var result = new List<string>();
        for (var i = 0; i < count; i++)
            result.Add(Ptr.ToUtf8String(Unsafe_SDL_GetRenderDriver(i))!);
        return result;
    }
}