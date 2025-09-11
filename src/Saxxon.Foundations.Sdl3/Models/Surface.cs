using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

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
    public static int GetWidth(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return ptr.AsReadOnlyRef().w;
    }

    public static int GetHeight(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return ptr.AsReadOnlyRef().h;
    }

    public static SDL_PixelFormat GetFormat(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return ptr.AsReadOnlyRef().format;
    }

    public static int GetPitch(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return ptr.AsReadOnlyRef().pitch;
    }

    public static unsafe Span<byte> GetPixels(
        this IntPtr<SDL_Surface> ptr
    )
    {
        var ptrRef = ptr.AsReadOnlyRef();
        return ptrRef.pixels == IntPtr.Zero
            ? Span<byte>.Empty
            : new Span<byte>((void*)ptrRef.pixels, ptrRef.pitch * ptrRef.h);
    }

    public static SDL_SurfaceFlags GetFlags(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return ptr.AsReadOnlyRef().flags;
    }

    public static unsafe IntPtr<SDL_Surface> Convert(
        this IntPtr<SDL_Surface> ptr,
        SDL_PixelFormat format
    )
    {
        return ((IntPtr)SDL_ConvertSurface(ptr, format))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> Convert(
        this IntPtr<SDL_Surface> ptr,
        SDL_PixelFormat format,
        IntPtr<SDL_Palette> palette,
        SDL_Colorspace colorSpace,
        SDL_PropertiesID props
    )
    {
        return ((IntPtr)SDL_ConvertSurfaceAndColorspace(
                ptr,
                format,
                palette,
                colorSpace, props))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> Create(
        int width,
        int height,
        SDL_PixelFormat format
    )
    {
        return ((IntPtr)SDL_CreateSurface(width, height, format))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> CreateFrom(
        int width,
        int height,
        SDL_PixelFormat format,
        ReadOnlySpan<byte> pixels,
        int pitch
    )
    {
        fixed (byte* pixelsPtr = pixels)
        {
            return ((IntPtr)SDL_CreateSurfaceFrom(width, height, format, (IntPtr)pixelsPtr, pitch))
                .AssertSdlNotNull();
        }
    }

    public static unsafe IntPtr<SDL_Palette> CreatePalette(
        this IntPtr<SDL_Surface> ptr
    )
    {
        var result = SDL_CreateSurfacePalette(ptr);
        if (result == null)
            throw new SdlException();

        return result;
    }

    public static unsafe void Destroy(
        this IntPtr<SDL_Surface> ptr
    )
    {
        SDL_DestroySurface(ptr);
    }

    public static unsafe IntPtr<SDL_Surface> Duplicate(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return SDL_DuplicateSurface(ptr);
    }

    public static unsafe void Flip(
        this IntPtr<SDL_Surface> ptr,
        SDL_FlipMode flipMode
    )
    {
        SDL_FlipSurface(ptr, flipMode)
            .AssertSdlSuccess();
    }

    public static unsafe uint GetColorKey(
        this IntPtr<SDL_Surface> ptr
    )
    {
        uint result;
        SDL_GetSurfaceColorKey(ptr, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe SDL_Colorspace GetColorSpace(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return SDL_GetSurfaceColorspace(ptr);
    }

    public static unsafe IntPtr<SDL_Palette> GetPalette(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return SDL_GetSurfacePalette(ptr);
    }

    public static unsafe SDL_PropertiesID GetProperties(
        this IntPtr<SDL_Surface> ptr
    )
    {
        var result = SDL_GetSurfaceProperties(ptr);
        if (result == 0)
            throw new SdlException();

        return result;
    }

    public static unsafe bool HasColorKey(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return SDL_SurfaceHasColorKey(ptr);
    }

    public static unsafe bool HasRle(
        this IntPtr<SDL_Surface> ptr
    )
    {
        return SDL_SurfaceHasRLE(ptr);
    }

    public static unsafe IntPtr<SDL_Surface> Load(
        string name
    )
    {
        using var nameStr = new Utf8Span(name);

        return ((IntPtr)SDL_LoadBMP(nameStr))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> LoadIo(
        IntPtr<SDL_IOStream> stream,
        bool closeIo
    )
    {
        return ((IntPtr)SDL_LoadBMP_IO(stream, closeIo))
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
        return ((IntPtr)SDL_ScaleSurface(surface, width, height, scaleMode))
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