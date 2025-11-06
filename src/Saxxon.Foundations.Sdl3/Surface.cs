using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Surface"/>.
/// </summary>
/// <remarks>
/// Since most of the engine's actions will be done with
/// textures and renderers directly, blitting operations are
/// not included.
/// </remarks>
[PublicAPI]
public static class Surface
{
    /// <summary>
    /// Extensions for <see cref="SDL_Surface"/> references.
    /// </summary>
    extension(IntPtr<SDL_Surface> surface)
    {
        /// <summary>
        /// The width of the surface, in pixels.
        /// </summary>
        public ref readonly int Width =>
            ref surface.AsReadOnlyRef().w;

        /// <summary>
        /// The height of the surface, in pixels.
        /// </summary>
        public ref readonly int Height =>
            ref surface.AsReadOnlyRef().h;

        /// <summary>
        /// The format of the pixel data.
        /// </summary>
        public ref readonly SDL_PixelFormat Format =>
            ref surface.AsReadOnlyRef().format;

        /// <summary>
        /// The distance in bytes between rows of pixels.
        /// </summary>
        public ref readonly int Pitch =>
            ref surface.AsReadOnlyRef().pitch;

        /// <summary>
        /// The flags set on the surface.
        /// </summary>
        public ref readonly SDL_SurfaceFlags Flags =>
            ref surface.AsReadOnlyRef().flags;
    }

    /// <summary>
    /// Gets a span of the raw pixel data for a surface.
    /// </summary>
    public static unsafe Span<byte> GetPixels(
        this IntPtr<SDL_Surface> surface
    )
    {
        var ptrRef = surface.AsReadOnlyRef();
        return ptrRef.pixels == IntPtr.Zero
            ? Span<byte>.Empty
            : new Span<byte>((void*)ptrRef.pixels, ptrRef.pitch * ptrRef.h);
    }

    /// <summary>
    /// Copies an existing surface to a new surface of the specified format.
    /// </summary>
    /// <param name="surface">
    /// The existing surface to convert.
    /// </param>
    /// <param name="format">
    /// The new pixel format.
    /// </param>
    /// <returns>
    /// The new surface that is created.
    /// </returns>
    /// <remarks>
    /// This function is used to optimize images for faster repeat blitting. This is accomplished by
    /// converting the original and storing the result as a new surface. The new, optimized surface
    /// can then be used as the source for future blits, making them faster.
    /// 
    /// If you are converting to an indexed surface and want to map colors to a palette, you can use
    /// <see cref="Convert(IntPtr{SDL_Surface},SDL_PixelFormat,IntPtr{SDL_Palette},SDL_Colorspace,SDL_PropertiesID)"/>
    /// instead. If the original surface has alternate images, the new surface will have a reference to them as well.
    ///
    /// This function can be called on different threads with different surfaces.
    /// </remarks>
    public static unsafe IntPtr<SDL_Surface> Convert(
        this IntPtr<SDL_Surface> surface,
        SDL_PixelFormat format
    )
    {
        return ((IntPtr<SDL_Surface>)SDL_ConvertSurface(surface, format))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Copies an existing surface to a new surface of the specified format and colorspace.
    /// </summary>
    /// <param name="surface">
    /// The existing surface to convert.
    /// </param>
    /// <param name="format">
    /// The new pixel format.
    /// </param>
    /// <param name="palette">
    /// An optional palette to use for indexed formats, may be null.
    /// </param>
    /// <param name="colorSpace">
    /// The new colorspace.
    /// </param>
    /// <param name="props">
    /// Additional color properties, or 0.
    /// </param>
    /// <returns>
    /// The new surface that is created.
    /// </returns>
    /// <remarks>
    /// This function converts an existing surface to a new format and colorspace and returns the new surface.
    /// This will perform any pixel format and colorspace conversion needed.
    /// 
    /// If the original surface has alternate images, the new surface will have a reference to them as well.
    ///
    /// This function can be called on different threads with different surfaces.
    /// </remarks>
    public static unsafe IntPtr<SDL_Surface> Convert(
        this IntPtr<SDL_Surface> surface,
        SDL_PixelFormat format,
        IntPtr<SDL_Palette> palette,
        SDL_Colorspace colorSpace,
        SDL_PropertiesID props
    )
    {
        return ((IntPtr<SDL_Surface>)SDL_ConvertSurfaceAndColorspace(
                surface,
                format,
                palette,
                colorSpace, props))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Allocates a new surface with a specific pixel format.
    /// </summary>
    /// <param name="width">
    /// Width of the surface, in pixels.
    /// </param>
    /// <param name="height">
    /// Height of the surface, in pixels.
    /// </param>
    /// <param name="format">
    /// The new surface's pixel format.
    /// </param>
    /// <returns>
    /// The created surface.
    /// </returns>
    /// <remarks>
    /// The pixels of the new surface are initialized to zero.
    ///
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe IntPtr<SDL_Surface> Create(
        int width,
        int height,
        SDL_PixelFormat format
    )
    {
        return ((IntPtr<SDL_Surface>)SDL_CreateSurface(width, height, format))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Allocates a new surface with a specific pixel format and existing pixel data.
    /// </summary>
    /// <param name="width">
    /// Width of the surface, in pixels.
    /// </param>
    /// <param name="height">
    /// Height of the surface, in pixels.
    /// </param>
    /// <param name="format">
    /// The new surface's pixel format.
    /// </param>
    /// <param name="pixels">
    /// Pointer to existing pixel data.
    /// </param>
    /// <param name="pitch">
    /// The number of bytes between each row, including padding.
    /// </param>
    /// <returns>
    /// The created surface.
    /// </returns>
    /// <remarks>
    /// No copy is made of the pixel data. Pixel data is not managed automatically; you must free the surface before
    /// you free the pixel data.
    /// 
    /// Pitch is the offset in bytes from one row of pixels to the next, e.g. width*4 for
    /// <see cref="SDL_PixelFormat.SDL_PIXELFORMAT_RGBA8888"/>.
    /// 
    /// You may pass null for pixel data and 0 for pitch to create a surface that you will fill in with valid values
    /// later.
    /// </remarks>
    public static unsafe IntPtr<SDL_Surface> CreateFrom(
        int width,
        int height,
        SDL_PixelFormat format,
        IntPtr pixels,
        int pitch
    )
    {
        return ((IntPtr<SDL_Surface>)SDL_CreateSurfaceFrom(width, height, format, pixels, pitch))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Creates a palette and associate it with a surface.
    /// </summary>
    /// <param name="surface">
    /// The surface to update.
    /// </param>
    /// <returns>
    /// The new palette.
    /// </returns>
    /// <remarks>
    /// This function creates a palette compatible with the provided surface. The palette is then returned for you
    /// to modify, and the surface will automatically use the new palette in future operations. You do not need to
    /// destroy the returned palette, it will be freed when the reference count reaches 0, usually when the surface
    /// is destroyed.
    /// 
    /// Bitmap surfaces with 1-bit indexed formats will have the palette initialized with 0 as white and 1 as black.
    /// Other surfaces will get a palette initialized with white in every entry.
    ///
    /// If this function is called for a surface that already has a palette, a new palette will be created to
    /// replace it.
    /// </remarks>
    public static unsafe IntPtr<SDL_Palette> CreatePalette(
        this IntPtr<SDL_Surface> surface
    )
    {
        var result = SDL_CreateSurfacePalette(surface);
        return result == null ? throw new SdlException() : result;
    }

    /// <summary>
    /// Frees a surface.
    /// </summary>
    /// <param name="surface">
    /// The surface to free.
    /// </param>
    public static unsafe void Destroy(
        this IntPtr<SDL_Surface> surface
    )
    {
        SDL_DestroySurface(surface);
    }

    /// <summary>
    /// Creates a new surface identical to the existing surface.
    /// </summary>
    /// <param name="surface">
    /// The surface to duplicate.
    /// </param>
    /// <returns>
    /// A copy of the surface.
    /// </returns>
    /// <remarks>
    /// If the original surface has alternate images, the new surface will have a reference to them as well.
    /// The returned surface should be freed with <see cref="Destroy"/>.
    /// </remarks>
    public static unsafe IntPtr<SDL_Surface> Duplicate(
        this IntPtr<SDL_Surface> surface
    )
    {
        return SDL_DuplicateSurface(surface);
    }

    /// <summary>
    /// Flip a surface vertically or horizontally.
    /// </summary>
    /// <param name="surface">
    /// The surface to flip.
    /// </param>
    /// <param name="flipMode">
    /// The direction to flip.
    /// </param>
    public static unsafe void Flip(
        this IntPtr<SDL_Surface> surface,
        SDL_FlipMode flipMode
    )
    {
        SDL_FlipSurface(surface, flipMode)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Gets the color key (transparent pixel) for a surface.
    /// </summary>
    public static unsafe uint GetColorKey(
        this IntPtr<SDL_Surface> ptr
    )
    {
        uint result;
        SDL_GetSurfaceColorKey(ptr, &result)
            .AssertSdlSuccess();
        return result;
    }

    /// <summary>
    /// Gets the colorspace used by a surface.
    /// </summary>
    public static unsafe SDL_Colorspace GetColorSpace(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return SDL_GetSurfaceColorspace(ptr);
    }

    /// <summary>
    /// Gets the palette used by a surface.
    /// </summary>
    public static unsafe IntPtr<SDL_Palette> GetPalette(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return SDL_GetSurfacePalette(ptr);
    }

    /// <summary>
    /// Gets the properties associated with a surface.
    /// </summary>
    public static unsafe SDL_PropertiesID GetProperties(
        this IntPtr<SDL_Surface> ptr
    )
    {
        var result = SDL_GetSurfaceProperties(ptr);
        return result == 0 ? throw new SdlException() : result;
    }

    /// <summary>
    /// Determines whether the surface has a color key.
    /// </summary>
    public static unsafe bool HasColorKey(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return SDL_SurfaceHasColorKey(ptr);
    }

    /// <summary>
    /// Determines whether the surface is RLE enabled.
    /// </summary>
    public static unsafe bool HasRle(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return SDL_SurfaceHasRLE(ptr);
    }

    /// <summary>
    /// Loads a BMP image from a file.
    /// </summary>
    /// <param name="name">
    /// The BMP file to load.
    /// </param>
    /// <returns>
    /// The created surface.
    /// </returns>
    /// <remarks>
    /// The new surface should be freed with <see cref="Destroy"/>. Not doing so will result in a memory leak.
    /// </remarks>
    public static unsafe IntPtr<SDL_Surface> Load(
        string name
    )
    {
        using var nameStr = new UnmanagedString(name);

        return ((IntPtr<SDL_Surface>)SDL_LoadBMP(nameStr))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a BMP image from a seekable SDL data stream.
    /// </summary>
    /// <param name="stream">
    /// The data stream for the surface.
    /// </param>
    /// <param name="closeIo">
    /// If true, closes the stream before returning, even in the case of an error.
    /// </param>
    /// <returns>
    /// The created surface.
    /// </returns>
    /// <remarks>
    /// The new surface should be freed with <see cref="Destroy"/>. Not doing so will result in a memory leak.
    /// </remarks>
    public static unsafe IntPtr<SDL_Surface> LoadIo(
        IntPtr<SDL_IOStream> stream,
        bool closeIo
    )
    {
        return ((IntPtr<SDL_Surface>)SDL_LoadBMP_IO(stream, closeIo))
            .AssertSdlNotNull();
    }

    public static unsafe void Lock(
        IntPtr<SDL_Surface> ptr
    )
    {
        SDL_LockSurface(ptr)
            .AssertSdlSuccess();
    }

    public static unsafe uint MapRgb(
        this IntPtr<SDL_Surface> ptr,
        byte r,
        byte g,
        byte b
    )
    {
        return SDL_MapSurfaceRGB(ptr, r, g, b);
    }

    public static unsafe uint MapRgba(
        this IntPtr<SDL_Surface> ptr,
        byte r,
        byte g,
        byte b,
        byte a
    )
    {
        return SDL_MapSurfaceRGBA(ptr, r, g, b, a);
    }

    public static unsafe void PremultiplyAlpha(
        this IntPtr<SDL_Surface> ptr,
        bool linear
    )
    {
        SDL_PremultiplySurfaceAlpha(ptr, linear)
            .AssertSdlSuccess();
    }

    public static unsafe SDL_Color ReadPixel(
        this IntPtr<SDL_Surface> ptr,
        int x,
        int y
    )
    {
        byte r, g, b, a;
        SDL_ReadSurfacePixel(ptr, x, y, &r, &g, &b, &a)
            .AssertSdlSuccess();
        return new SDL_Color
        {
            r = r,
            g = g,
            b = b,
            a = a
        };
    }

    public static unsafe SDL_FColor ReadPixelFloat(
        this IntPtr<SDL_Surface> ptr,
        int x,
        int y
    )
    {
        float r, g, b, a;
        SDL_ReadSurfacePixelFloat(ptr, x, y, &r, &g, &b, &a)
            .AssertSdlSuccess();
        return new SDL_FColor
        {
            r = r,
            g = g,
            b = b,
            a = a
        };
    }

    public static unsafe void SetPalette(
        this IntPtr<SDL_Surface> ptr,
        IntPtr<SDL_Palette> palette
    )
    {
        SDL_SetSurfacePalette(ptr, palette)
            .AssertSdlSuccess();
    }

    public static unsafe void SetRle(
        this IntPtr<SDL_Surface> ptr,
        bool enabled
    )
    {
        SDL_SetSurfaceRLE(ptr, enabled)
            .AssertSdlSuccess();
    }

    public static unsafe void Unlock(
        IntPtr<SDL_Surface> ptr
    )
    {
        SDL_UnlockSurface(ptr);
    }

    public static unsafe void WritePixel(
        this IntPtr<SDL_Surface> ptr,
        int x,
        int y,
        byte r,
        byte g,
        byte b,
        byte a
    )
    {
        SDL_WriteSurfacePixel(ptr, x, y, r, g, b, a)
            .AssertSdlSuccess();
    }

    public static unsafe void WritePixelFloat(
        this IntPtr<SDL_Surface> ptr,
        int x,
        int y,
        float r,
        float g,
        float b,
        float a
    )
    {
        SDL_WriteSurfacePixelFloat(ptr, x, y, r, g, b, a)
            .AssertSdlSuccess();
    }

    public static unsafe void Blit9Grid(
        this IntPtr<SDL_Surface> src,
        SDL_Rect? srcRect,
        int leftWidth,
        int rightWidth,
        int topHeight,
        int bottomHeight,
        float scale,
        SDL_ScaleMode scaleMode,
        IntPtr<SDL_Surface> dst,
        SDL_Rect? dstRect
    )
    {
        SDL_BlitSurface9Grid(
            src,
            srcRect is { } sr ? &sr : null,
            leftWidth,
            rightWidth,
            topHeight,
            bottomHeight,
            scale,
            scaleMode,
            dst,
            dstRect is { } dr ? &dr : null
        ).AssertSdlSuccess();
    }

    public static unsafe void BlitTiledWithScale(
        this IntPtr<SDL_Surface> src,
        SDL_Rect? srcRect,
        float scale,
        SDL_ScaleMode scaleMode,
        IntPtr<SDL_Surface> dst,
        SDL_Rect? dstRect
    )
    {
        SDL_BlitSurfaceTiledWithScale(
            src,
            srcRect is { } sr ? &sr : null,
            scale,
            scaleMode,
            dst,
            dstRect is { } dr ? &dr : null
        ).AssertSdlSuccess();
    }

    public static unsafe void BlitTiled(
        this IntPtr<SDL_Surface> src,
        SDL_Rect? srcRect,
        IntPtr<SDL_Surface> dst,
        SDL_Rect? dstRect
    )
    {
        SDL_BlitSurfaceTiled(
            src,
            srcRect is { } sr ? &sr : null,
            dst,
            dstRect is { } dr ? &dr : null
        ).AssertSdlSuccess();
    }

    public static unsafe void Stretch(
        this IntPtr<SDL_Surface> src,
        SDL_Rect? srcRect,
        IntPtr<SDL_Surface> dst,
        SDL_Rect? dstRect,
        SDL_ScaleMode scaleMode
    )
    {
        SDL_StretchSurface(
            src,
            srcRect is { } sr ? &sr : null,
            dst,
            dstRect is { } dr ? &dr : null,
            scaleMode
        ).AssertSdlSuccess();
    }

    public static unsafe void BlitUncheckedScaled(
        this IntPtr<SDL_Surface> src,
        SDL_Rect? srcRect,
        IntPtr<SDL_Surface> dst,
        SDL_Rect? dstRect,
        SDL_ScaleMode scaleMode
    )
    {
        SDL_BlitSurfaceUncheckedScaled(
            src,
            srcRect is { } sr ? &sr : null,
            dst,
            dstRect is { } dr ? &dr : null,
            scaleMode
        ).AssertSdlSuccess();
    }

    public static unsafe void BlitScaled(
        this IntPtr<SDL_Surface> src,
        SDL_Rect? srcRect,
        IntPtr<SDL_Surface> dst,
        SDL_Rect? dstRect,
        SDL_ScaleMode scaleMode
    )
    {
        SDL_BlitSurfaceScaled(
            src,
            srcRect is { } sr ? &sr : null,
            dst,
            dstRect is { } dr ? &dr : null,
            scaleMode
        ).AssertSdlSuccess();
    }

    public static unsafe void BlitUnchecked(
        this IntPtr<SDL_Surface> src,
        SDL_Rect? srcRect,
        IntPtr<SDL_Surface> dst,
        SDL_Rect? dstRect
    )
    {
        SDL_BlitSurfaceUnchecked(
            src,
            srcRect is { } sr ? &sr : null,
            dst,
            dstRect is { } dr ? &dr : null
        ).AssertSdlSuccess();
    }

    public static unsafe void Blit(
        this IntPtr<SDL_Surface> src,
        SDL_Rect? srcRect,
        IntPtr<SDL_Surface> dst,
        SDL_Rect? dstRect
    )
    {
        SDL_BlitSurface(
            src,
            srcRect is { } sr ? &sr : null,
            dst,
            dstRect is { } dr ? &dr : null
        ).AssertSdlSuccess();
    }

    public static unsafe void FillRects(
        this IntPtr<SDL_Surface> dst,
        ReadOnlySpan<SDL_Rect> rects,
        uint color
    )
    {
        fixed (SDL_Rect* rectPtr = rects)
        {
            SDL_FillSurfaceRects(
                dst,
                rectPtr,
                rects.Length,
                color
            ).AssertSdlSuccess();
        }
    }

    public static unsafe void FillRect(
        this IntPtr<SDL_Surface> dst,
        SDL_Rect rect,
        uint color
    )
    {
        SDL_FillSurfaceRect(dst, &rect, color)
            .AssertSdlSuccess();
    }

    public static unsafe void Clear(
        this IntPtr<SDL_Surface> surface,
        SDL_FColor color
    )
    {
        SDL_ClearSurface(surface, color.r, color.g, color.b, color.a)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<SDL_Surface> Scale(
        this IntPtr<SDL_Surface> surface,
        int width,
        int height,
        SDL_ScaleMode scaleMode
    )
    {
        return ((IntPtr<SDL_Surface>)SDL_ScaleSurface(surface, width, height, scaleMode))
            .AssertSdlNotNull();
    }

    public static unsafe SDL_Rect? GetClipRect(
        this IntPtr<SDL_Surface> surface
    )
    {
        SDL_Rect rect;
        SDL_GetSurfaceClipRect(surface, &rect)
            .AssertSdlSuccess();
        return rect;
    }

    public static unsafe void SetClipRect(
        this IntPtr<SDL_Surface> surface,
        SDL_Rect? rect
    )
    {
        SDL_SetSurfaceClipRect(surface, rect is { } sr ? &sr : null)
            .AssertSdlSuccess();
    }

    public static unsafe SDL_BlendMode GetBlendMode(
        this IntPtr<SDL_Surface> surface
    )
    {
        SDL_BlendMode result;
        SDL_GetSurfaceBlendMode(surface, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe void SetBlendMode(
        this IntPtr<SDL_Surface> surface,
        SDL_BlendMode blendMode
    )
    {
        SDL_SetSurfaceBlendMode(surface, blendMode)
            .AssertSdlSuccess();
    }

    public static unsafe byte GetAlphaMod(
        this IntPtr<SDL_Surface> surface
    )
    {
        byte result;
        SDL_GetSurfaceAlphaMod(surface, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe void SetAlphaMod(
        this IntPtr<SDL_Surface> surface,
        byte alpha
    )
    {
        SDL_SetSurfaceAlphaMod(surface, alpha)
            .AssertSdlSuccess();
    }

    public static unsafe SDL_Color GetColorMod(
        this IntPtr<SDL_Surface> surface
    )
    {
        byte r, g, b;
        SDL_GetSurfaceColorMod(surface, &r, &g, &b)
            .AssertSdlSuccess();
        return new SDL_Color
        {
            r = r,
            g = g,
            b = b
        };
    }

    public static unsafe void SetColorMod(
        this IntPtr<SDL_Surface> surface,
        SDL_Color color
    )
    {
        SDL_SetSurfaceColorMod(surface, color.r, color.g, color.b)
            .AssertSdlSuccess();
    }

    public static unsafe void SetColorKey(
        this IntPtr<SDL_Surface> surface,
        uint? colorKey
    )
    {
        SDL_SetSurfaceColorKey(surface, colorKey is not null, colorKey ?? 0)
            .AssertSdlSuccess();
    }

    public static unsafe void RemoveAlternateImages(this IntPtr<SDL_Surface> surface)
    {
        SDL_RemoveSurfaceAlternateImages(surface);
    }

    [MustDisposeResource]
    public static unsafe IMemoryOwner<IntPtr<SDL_Surface>> GetImages(this IntPtr<SDL_Surface> surface)
    {
        int count;
        var images = SDL_GetSurfaceImages(surface, &count);
        return SdlMemoryManager.Owned(images, count);
    }

    public static unsafe bool HasAlternateImages(this IntPtr<SDL_Surface> surface)
    {
        return SDL_SurfaceHasAlternateImages(surface);
    }

    public static unsafe void AddAlternateImage(this IntPtr<SDL_Surface> surface, IntPtr<SDL_Surface> image)
    {
        SDL_AddSurfaceAlternateImage(surface, image)
            .AssertSdlSuccess();
    }

    public static unsafe void SetColorSpace(this IntPtr<SDL_Surface> surface, SDL_Colorspace colorSpace)
    {
        SDL_SetSurfaceColorspace(surface, colorSpace)
            .AssertSdlSuccess();
    }
}