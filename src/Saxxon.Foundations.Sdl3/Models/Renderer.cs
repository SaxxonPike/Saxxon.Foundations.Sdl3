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
    /// <summary>
    /// Clears the current rendering target with the drawing color.
    /// </summary>
    /// <param name="renderer">
    /// The rendering context.
    /// </param>
    public static unsafe void Clear(this IntPtr<SDL_Renderer> renderer)
    {
        SDL_RenderClear(renderer)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Gets whether clipping is enabled on the given render target.
    /// </summary>
    /// <param name="renderer">
    /// The rendering context.
    /// </param>
    /// <returns>
    /// True if clipping is enabled or false if not.
    /// </returns>
    public static unsafe bool IsClipEnabled(this IntPtr<SDL_Renderer> renderer)
    {
        return SDL_RenderClipEnabled(renderer);
    }

    /// <summary>
    /// Draws debug text to a renderer.
    /// </summary>
    /// <param name="renderer">
    /// The renderer which should draw a line of text.
    /// </param>
    /// <param name="x">
    /// The x coordinate where the top-left corner of the text will draw.
    /// </param>
    /// <param name="y">
    /// The y coordinate where the top-left corner of the text will draw.
    /// </param>
    /// <param name="text">
    /// The string to render.
    /// </param>
    public static unsafe void DebugText(this IntPtr<SDL_Renderer> renderer, float x, float y, ReadOnlySpan<char> text)
    {
        using var bytes = new UnmanagedString(text);
        SDL_RenderDebugText(renderer, x, y, bytes)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Draws debug text to a renderer using a .NET string formatter.
    /// </summary>
    /// <param name="renderer">
    /// The renderer which should draw a line of text.
    /// </param>
    /// <param name="x">
    /// The x coordinate where the top-left corner of the text will draw.
    /// </param>
    /// <param name="y">
    /// The y coordinate where the top-left corner of the text will draw.
    /// </param>
    /// <param name="format">
    /// Formatting to use for assembling the string from argument values.
    /// </param>
    /// <param name="args">
    /// Arguments for use with the format string.
    /// </param>
    [StringFormatMethod(nameof(format))]
    public static unsafe void DebugText(
        this IntPtr<SDL_Renderer> renderer,
        float x,
        float y,
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        using var bytes = UnmanagedString.Format(format, args);
        SDL_RenderDebugText(renderer, x, y, bytes)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Fill a rectangle on the current rendering target with the drawing color at subpixel precision.
    /// </summary>
    /// <param name="renderer">
    /// The renderer which should fill a rectangle.
    /// </param>
    /// <param name="rect">
    /// The destination rectangle, or null for the entire rendering target.
    /// </param>
    public static unsafe void FillRect(this IntPtr<SDL_Renderer> renderer, SDL_FRect? rect)
    {
        SDL_RenderFillRect(renderer, rect is {} r ? &r : null)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Fill some number of rectangles on the current rendering target with the drawing color at subpixel precision.
    /// </summary>
    /// <param name="renderer">
    /// The renderer which should fill multiple rectangles.
    /// </param>
    /// <param name="rects">
    /// A span of destination rectangles.
    /// </param>
    public static unsafe void FillRects(this IntPtr<SDL_Renderer> renderer, ReadOnlySpan<SDL_FRect> rects)
    {
        fixed (SDL_FRect* data = rects)
        {
            SDL_RenderFillRects(renderer, data, rects.Length)
                .AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Render a list of triangles, optionally using a texture and indices into the vertex array. Color and alpha
    /// modulation is done per vertex (the texture's ColorMod and AlphaMod are ignored).
    /// </summary>
    /// <param name="renderer">
    /// The rendering context.
    /// </param>
    /// <param name="texture">
    /// The SDL texture to use, or null.
    /// </param>
    /// <param name="vertices">
    /// Vertex data of the geometry to render.
    /// </param>
    /// <param name="indices">
    /// Render order of vertices.
    /// </param>
    public static unsafe void Geometry(
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_Texture> texture,
        ReadOnlySpan<SDL_Vertex> vertices,
        ReadOnlySpan<int> indices
    )
    {
        fixed (SDL_Vertex* data = vertices)
        fixed (int* indexData = indices)
        {
            SDL_RenderGeometry(
                renderer,
                texture,
                data,
                vertices.Length,
                indexData,
                indices.Length
            ).AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Render a list of triangles, optionally using a texture and indices into the vertex arrays. Color and alpha
    /// modulation is done per vertex (the texture's ColorMod and AlphaMod are ignored).
    /// </summary>
    /// <param name="renderer">
    /// The rendering context.
    /// </param>
    /// <param name="texture">
    /// The SDL texture to use, or null.
    /// </param>
    /// <param name="vertices">
    /// Vertex data of the geometry to render.
    /// </param>
    /// <param name="xyOffset">
    /// Offset within TVertex of vertex X/Y coordinates.
    /// </param>
    /// <param name="colorOffset">
    /// Offset within TVertex of RGBA color values.
    /// </param>
    /// <param name="uvOffset">
    /// Offset within TVertex of texture U/V coordinates.
    /// </param>
    /// <param name="indices">
    /// Render order of vertices.
    /// </param>
    /// <typeparam name="TIndex">
    /// Type of indices into vertex data.
    /// </typeparam>
    /// <typeparam name="TVertex">
    /// Type of vertex data.
    /// </typeparam>
    public static unsafe void GeometryRaw<TIndex, TVertex>(
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_Texture> texture,
        ReadOnlySpan<TVertex> vertices,
        int xyOffset,
        int colorOffset,
        int uvOffset,
        ReadOnlySpan<TIndex> indices
    ) where TIndex : unmanaged where TVertex : unmanaged
    {
        var vertexSize = sizeof(TVertex);
        var indexSize = sizeof(TIndex);

        fixed (TVertex* vertexData = vertices)
        fixed (void* indexData = indices)
        {
            SDL_RenderGeometryRaw(
                renderer,
                texture,
                (float*)((byte*)vertexData + xyOffset),
                vertexSize,
                (SDL_FColor*)((byte*)vertexData + colorOffset),
                vertexSize,
                (float*)((byte*)vertexData + uvOffset),
                vertexSize,
                vertices.Length,
                (IntPtr)indexData,
                indices.Length * indexSize,
                indexSize
            ).AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Render a list of triangles, optionally using a texture and indices into the vertex arrays. Color and alpha
    /// modulation is done per vertex (the texture's ColorMod and AlphaMod are ignored).
    /// </summary>
    /// <param name="renderer">
    /// The rendering context.
    /// </param>
    /// <param name="texture">
    /// The SDL texture to use, or null.
    /// </param>
    /// <param name="xy">
    /// Reference to the first X/Y coordinate.
    /// </param>
    /// <param name="xyStride">
    /// Number of bytes between X/Y coordinate data.
    /// </param>
    /// <param name="color">
    /// Reference to the first color.
    /// </param>
    /// <param name="colorStride">
    /// Number of bytes between color data.
    /// </param>
    /// <param name="uv">
    /// Reference to the first U/V texture coordinate.
    /// </param>
    /// <param name="uvStride">
    /// Number of bytes between U/V coordinate data.
    /// </param>
    /// <param name="numVertices">
    /// Number of vertices.
    /// </param>
    /// <param name="indices">
    /// Render order of vertices.
    /// </param>
    /// <typeparam name="TIndex">
    /// Type of indices into vertex data.
    /// </typeparam>
    public static unsafe void GeometryRaw<TIndex>(
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_Texture> texture,
        ref readonly SDL_FPoint xy,
        int xyStride,
        ref readonly SDL_FColor color,
        int colorStride,
        ref readonly SDL_FPoint uv,
        int uvStride,
        int numVertices,
        ReadOnlySpan<TIndex> indices
    ) where TIndex : unmanaged
    {
        fixed (void* xyData = &xy)
        fixed (void* colorData = &color)
        fixed (void* uvData = &uv)
        fixed (void* indexData = indices)
        {
            SDL_RenderGeometryRaw(
                renderer,
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

    
    /// <summary>
    /// Render a list of triangles, optionally using a texture and indices into the vertex arrays. Color and alpha
    /// modulation is done per vertex (the texture's ColorMod and AlphaMod are ignored).
    /// </summary>
    /// <param name="renderer">
    /// The rendering context.
    /// </param>
    /// <param name="texture">
    /// The SDL texture to use, or null.
    /// </param>
    /// <param name="xy">
    /// Vertex X/Y coordinate data.
    /// </param>
    /// <param name="color">
    /// Vertex color data.
    /// </param>
    /// <param name="uv">
    /// Vertex U/V texture coordinate data.
    /// </param>
    /// <param name="indices">
    /// Render order of vertices.
    /// </param>
    /// <typeparam name="TIndex">
    /// Type of indices into vertex data.
    /// </typeparam>
    public static unsafe void GeometryRaw<TIndex>(
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_Texture> texture,
        ReadOnlySpan<SDL_FPoint> xy,
        ReadOnlySpan<SDL_FColor> color,
        ReadOnlySpan<SDL_FPoint> uv,
        ReadOnlySpan<TIndex> indices
    ) where TIndex : unmanaged
    {
        var numVertices = Math.Min(Math.Min(xy.Length, color.Length), uv.Length);

        fixed (void* xyData = xy)
        fixed (void* colorData = color)
        fixed (void* uvData = uv)
        fixed (void* indexData = indices)
        {
            SDL_RenderGeometryRaw(
                renderer,
                texture,
                (float*)xyData,
                sizeof(float) * 2,
                (SDL_FColor*)colorData,
                sizeof(SDL_FColor),
                (float*)uvData,
                sizeof(float) * 2,
                numVertices,
                (IntPtr)indexData,
                indices.Length * sizeof(TIndex),
                sizeof(TIndex)
            ).AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Draw a line on the current rendering target at subpixel precision.
    /// </summary>
    /// <param name="renderer">
    /// The renderer which should draw a line.
    /// </param>
    /// <param name="x1">
    /// The x coordinate of the start point.
    /// </param>
    /// <param name="y1">
    /// The y coordinate of the start point.
    /// </param>
    /// <param name="x2">
    /// The x coordinate of the end point.
    /// </param>
    /// <param name="y2">
    /// The y coordinate of the end point.
    /// </param>
    public static unsafe void Line(this IntPtr<SDL_Renderer> renderer, float x1, float y1, float x2, float y2)
    {
        SDL_RenderLine(renderer, x1, y1, x2, y2)
            .AssertSdlSuccess();
    }

    public static unsafe void Lines(this IntPtr<SDL_Renderer> renderer, ReadOnlySpan<SDL_FPoint> points)
    {
        fixed (SDL_FPoint* data = points)
        {
            SDL_RenderLines(renderer, data, points.Length);
        }
    }

    public static unsafe void Point(this IntPtr<SDL_Renderer> renderer, float x, float y)
    {
        SDL_RenderPoint(renderer, x, y);
    }

    public static unsafe void Points(this IntPtr<SDL_Renderer> renderer, ReadOnlySpan<SDL_FPoint> points)
    {
        fixed (SDL_FPoint* data = points)
        {
            SDL_RenderPoints(renderer, data, points.Length);
        }
    }

    public static unsafe void Rect(this IntPtr<SDL_Renderer> renderer, SDL_FRect rect)
    {
        SDL_RenderRect(renderer, &rect)
            .AssertSdlSuccess();
    }

    public static unsafe void Rects(this IntPtr<SDL_Renderer> renderer, ReadOnlySpan<SDL_FRect> rects)
    {
        fixed (SDL_FRect* data = rects)
        {
            SDL_RenderRects(renderer, data, rects.Length)
                .AssertSdlSuccess();
        }
    }

    public static unsafe void Texture(
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_Texture> texture,
        SDL_FRect? srcRect,
        SDL_FRect? dstRect)
    {
        SDL_RenderTexture(
            renderer,
            texture,
            srcRect is { } sr ? &sr : null,
            dstRect is { } dr ? &dr : null
        ).AssertSdlSuccess();
    }

    public static unsafe void Texture9Grid(
        this IntPtr<SDL_Renderer> renderer,
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
            renderer,
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
        this IntPtr<SDL_Renderer> renderer,
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
            renderer,
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
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_Texture> texture,
        SDL_FRect? srcRect,
        SDL_FPoint? origin,
        SDL_FPoint? right,
        SDL_FPoint? down)
    {
        SDL_RenderTextureAffine(
            renderer,
            texture,
            srcRect is { } sr ? &sr : null,
            origin is { } o ? &o : null,
            right is { } r ? &r : null,
            down is { } d ? &d : null
        ).AssertSdlSuccess();
    }

    public static unsafe void TextureRotated(
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_Texture> texture,
        SDL_FRect? srcRect,
        SDL_FRect? dstRect,
        float angle,
        SDL_FPoint? center,
        SDL_FlipMode flip)
    {
        SDL_RenderTextureRotated(
            renderer,
            texture,
            srcRect is { } sr ? &sr : null,
            dstRect is { } dr ? &dr : null,
            angle,
            center is { } c ? &c : null,
            flip
        ).AssertSdlSuccess();
    }

    public static unsafe void TextureTiled(
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_Texture> texture,
        SDL_FRect? srcRect,
        float scale,
        SDL_FRect? dstRect)
    {
        SDL_RenderTextureTiled(
            renderer,
            texture,
            srcRect is { } sr ? &sr : null,
            scale,
            dstRect is { } dr ? &dr : null
        ).AssertSdlSuccess();
    }

    public static unsafe bool IsViewportSet(this IntPtr<SDL_Renderer> renderer)
    {
        return SDL_RenderViewportSet(renderer);
    }

    public static unsafe void SetClipRect(this IntPtr<SDL_Renderer> renderer, SDL_Rect? rect)
    {
        SDL_SetRenderClipRect(renderer, rect is { } r ? &r : null)
            .AssertSdlSuccess();
    }

    public static unsafe void SetColorScale(this IntPtr<SDL_Renderer> renderer, float scale)
    {
        SDL_SetRenderColorScale(renderer, scale)
            .AssertSdlSuccess();
    }

    public static unsafe void SetBlendMode(this IntPtr<SDL_Renderer> renderer, SDL_BlendMode blendMode)
    {
        SDL_SetRenderDrawBlendMode(renderer, blendMode)
            .AssertSdlSuccess();
    }

    public static unsafe void SetColor(this IntPtr<SDL_Renderer> renderer, byte r, byte g, byte b, byte a)
    {
        SDL_SetRenderDrawColor(renderer, r, g, b, a)
            .AssertSdlSuccess();
    }

    public static void SetColor(this IntPtr<SDL_Renderer> renderer, SDL_Color color) =>
        SetColor(renderer, color.r, color.g, color.b, color.a);

    public static unsafe void SetColorFloat(this IntPtr<SDL_Renderer> renderer, float r, float g, float b, float a)
    {
        SDL_SetRenderDrawColorFloat(renderer, r, g, b, a)
            .AssertSdlSuccess();
    }

    public static void SetColorFloat(this IntPtr<SDL_Renderer> renderer, SDL_FColor color) =>
        SetColorFloat(renderer, color.r, color.g, color.b, color.a);

    public static unsafe void SetScale(this IntPtr<SDL_Renderer> renderer, float x, float y)
    {
        SDL_SetRenderScale(renderer, x, y)
            .AssertSdlSuccess();
    }

    public static unsafe void SetTarget(this IntPtr<SDL_Renderer> renderer, IntPtr<SDL_Texture> texture)
    {
        SDL_SetRenderTarget(renderer, texture)
            .AssertSdlSuccess();
    }

    public static unsafe void SetViewport(this IntPtr<SDL_Renderer> renderer, SDL_Rect? rect)
    {
        SDL_SetRenderViewport(renderer, rect is { } r ? &r : null)
            .AssertSdlSuccess();
    }

    public static unsafe void SetVSync(this IntPtr<SDL_Renderer> renderer, int vsync)
    {
        SDL_SetRenderVSync(renderer, vsync);
    }

    public static unsafe (int W, int H) GetCurrentOutputSize(this IntPtr<SDL_Renderer> renderer)
    {
        int w, h;
        SDL_GetCurrentRenderOutputSize(renderer, &w, &h)
            .AssertSdlSuccess();
        return (w, h);
    }

    public static unsafe SDL_Rect? GetClipRect(this IntPtr<SDL_Renderer> renderer)
    {
        SDL_Rect result;
        SDL_GetRenderClipRect(renderer, &result)
            .AssertSdlSuccess();
        return result is { w: 0, h: 0 } ? null : result;
    }

    public static unsafe float GetColorScale(this IntPtr<SDL_Renderer> renderer)
    {
        float result;
        SDL_GetRenderColorScale(renderer, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe SDL_BlendMode GetBlendMode(this IntPtr<SDL_Renderer> renderer)
    {
        SDL_BlendMode result;
        SDL_GetRenderDrawBlendMode(renderer, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe SDL_Color GetColor(this IntPtr<SDL_Renderer> renderer)
    {
        SDL_Color result;
        SDL_GetRenderDrawColor(renderer, &result.r, &result.g, &result.b, &result.a)
            .AssertSdlSuccess();

        return new SDL_Color
        {
            r = result.r, g = result.g, b = result.b, a = result.a
        };
    }

    public static unsafe SDL_FColor GetColorFloat(this IntPtr<SDL_Renderer> renderer)
    {
        SDL_FColor result;
        SDL_GetRenderDrawColorFloat(renderer, &result.r, &result.g, &result.b, &result.a)
            .AssertSdlSuccess();
        return *&result;
    }

    public static unsafe SDL_PropertiesID GetProperties(this IntPtr<SDL_Renderer> renderer)
    {
        return SDL_GetRendererProperties(renderer);
    }

    public static unsafe (int W, int H) GetOutputSize(this IntPtr<SDL_Renderer> renderer)
    {
        int w, h;
        SDL_GetRenderOutputSize(renderer, &w, &h)
            .AssertSdlSuccess();
        return (w, h);
    }

    public static unsafe SDL_Rect GetSafeArea(this IntPtr<SDL_Renderer> renderer)
    {
        SDL_Rect result;
        SDL_GetRenderSafeArea(renderer, &result)
            .AssertSdlSuccess();
        return *&result;
    }

    public static unsafe float GetScale(this IntPtr<SDL_Renderer> renderer)
    {
        float result;
        SDL_GetRenderScale(renderer, &result, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe IntPtr<SDL_Texture> GetTarget(this IntPtr<SDL_Renderer> renderer)
    {
        var result = SDL_GetRenderTarget(renderer);
        return result != null ? result : IntPtr<SDL_Texture>.Zero;
    }

    public static unsafe SDL_Rect GetViewport(this IntPtr<SDL_Renderer> renderer)
    {
        SDL_Rect result;
        SDL_GetRenderViewport(renderer, &result)
            .AssertSdlSuccess();
        return *&result;
    }

    public static unsafe int GetVSync(this IntPtr<SDL_Renderer> renderer)
    {
        int result;
        SDL_GetRenderVSync(renderer, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe IntPtr<SDL_Window> GetWindow(this IntPtr<SDL_Renderer> renderer)
    {
        return SDL_GetRenderWindow(renderer);
    }

    public static unsafe void SetLogicalPresentation(
        this IntPtr<SDL_Renderer> renderer,
        int w,
        int h,
        SDL_RendererLogicalPresentation mode
    )
    {
        SDL_SetRenderLogicalPresentation(renderer, w, h, mode)
            .AssertSdlSuccess();
    }

    public static unsafe void Present(
        this IntPtr<SDL_Renderer> renderer
    )
    {
        SDL_RenderPresent(renderer)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<SDL_Texture> LoadTextureTypedIo(
        this IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_IOStream> src,
        bool closeIo,
        ReadOnlySpan<char> type
    )
    {
        using var typeStr = new UnmanagedString(type);
        return ((IntPtr<SDL_Texture>)IMG_LoadTextureTyped_IO(renderer, src, closeIo, typeStr))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Texture> LoadTexture(
        this IntPtr<SDL_Renderer> renderer,
        ReadOnlySpan<char> file
    )
    {
        using var fileStr = new UnmanagedString(file);
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

    public static unsafe SDL_FPoint CoordinatesToWindow(
        this IntPtr<SDL_Renderer> renderer,
        float x,
        float y
    )
    {
        float windowX, windowY;
        SDL_RenderCoordinatesToWindow(renderer, x, y, &windowX, &windowY)
            .AssertSdlSuccess();
        return Models.Point.CreateFloat(windowX, windowY);
    }

    public static SDL_FPoint CoordinatesToWindow(
        this IntPtr<SDL_Renderer> renderer,
        SDL_FPoint point
    ) => CoordinatesToWindow(renderer, point.x, point.y);

    public static unsafe SDL_FPoint CoordinatesFromWindow(
        this IntPtr<SDL_Renderer> renderer,
        float windowX,
        float windowY
    )
    {
        float x, y;
        SDL_RenderCoordinatesToWindow(renderer, windowX, windowY, &x, &y)
            .AssertSdlSuccess();
        return Models.Point.CreateFloat(x, y);
    }

    public static SDL_FPoint CoordinatesFromWindow(
        this IntPtr<SDL_Renderer> renderer,
        SDL_FPoint point
    ) => CoordinatesFromWindow(renderer, point.x, point.y);

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