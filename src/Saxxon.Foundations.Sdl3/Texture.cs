using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Texture"/>.
/// </summary>
[PublicAPI]
public static class Texture
{
    /// <summary>
    /// Extensions for <see cref="SDL_Texture"/> references.
    /// </summary>
    extension(IntPtr<SDL_Texture> ptr)
    {
        /// <summary>
        /// The width of the texture, in pixels.
        /// </summary>
        public ref readonly int Width =>
            ref ptr.AsReadOnlyRef().w;

        /// <summary>
        /// The height of the texture, in pixels.
        /// </summary>
        public ref readonly int Height =>
            ref ptr.AsReadOnlyRef().h;

        /// <summary>
        /// The format of the texture pixel data.
        /// </summary>
        public ref readonly SDL_PixelFormat Format =>
            ref ptr.AsReadOnlyRef().format;
    }

    /// <summary>
    /// Creates a new texture in GPU memory.
    /// </summary>
    public static unsafe IntPtr<SDL_Texture> Create(
        IntPtr<SDL_Renderer> renderer,
        SDL_PixelFormat format,
        SDL_TextureAccess access,
        int w,
        int h)
    {
        return ((IntPtr<SDL_Texture>)SDL_CreateTexture(renderer, format, access, w, h))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Copies surface pixels into a new texture in GPU memory.
    /// </summary>
    public static unsafe IntPtr<SDL_Texture> CreateFromSurface(
        IntPtr<SDL_Renderer> renderer,
        IntPtr<SDL_Surface> surface
    )
    {
        return ((IntPtr<SDL_Texture>)SDL_CreateTextureFromSurface(renderer, surface))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Creates a texture for a rendering context with the specified properties.
    /// </summary>
    /// <param name="renderer">
    /// The rendering context.
    /// </param>
    /// <param name="props">
    /// The properties to use.
    /// </param>
    /// <returns>
    /// The created texture.
    /// </returns>
    public static unsafe IntPtr<SDL_Texture> CreateWithProperties(
        IntPtr<SDL_Renderer> renderer,
        SDL_PropertiesID props
    )
    {
        return ((IntPtr<SDL_Texture>)SDL_CreateTextureWithProperties(renderer, props))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Frees a texture in GPU memory.
    /// </summary>
    public static unsafe void Destroy(this IntPtr<SDL_Texture> ptr)
    {
        SDL_DestroyTexture(ptr);
    }

    /// <summary>
    /// Gets the renderer that a texture is associated with.
    /// </summary>
    public static unsafe IntPtr<SDL_Renderer> GetRenderer(this IntPtr<SDL_Texture> ptr)
    {
        return ((IntPtr<SDL_Renderer>)SDL_GetRendererFromTexture(ptr))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Gets the alpha mod value of a texture.
    /// </summary>
    public static unsafe byte GetAlphaMod(this IntPtr<SDL_Texture> ptr)
    {
        byte result;
        SDL_GetTextureAlphaMod(ptr, &result)
            .AssertSdlSuccess();
        return result;
    }

    /// <summary>
    /// Gets the alpha mod value of a texture.
    /// </summary>
    public static unsafe float GetAlphaModFloat(this IntPtr<SDL_Texture> ptr)
    {
        float result;
        SDL_GetTextureAlphaModFloat(ptr, &result)
            .AssertSdlSuccess();
        return result;
    }

    /// <summary>
    /// Gets the blend mode of a texture.
    /// </summary>
    public static unsafe SDL_BlendMode GetBlendMode(this IntPtr<SDL_Texture> ptr)
    {
        SDL_BlendMode result;
        SDL_GetTextureBlendMode(ptr, &result)
            .AssertSdlSuccess();
        return result;
    }

    /// <summary>
    /// Gets the color mod values of a texture.
    /// </summary>
    public static unsafe SDL_Color GetColorMod(this IntPtr<SDL_Texture> ptr)
    {
        byte r, g, b;
        SDL_GetTextureColorMod(ptr, &r, &g, &b)
            .AssertSdlSuccess();
        return new SDL_Color
        {
            r = r,
            g = g,
            b = b
        };
    }

    /// <summary>
    /// Gets the color mod values of a texture.
    /// </summary>
    public static unsafe SDL_FColor GetColorModFloat(this IntPtr<SDL_Texture> ptr)
    {
        float r, g, b;
        SDL_GetTextureColorModFloat(ptr, &r, &g, &b)
            .AssertSdlSuccess();
        return new SDL_FColor
        {
            r = r,
            g = g,
            b = b
        };
    }

    /// <summary>
    /// Gets the property set of a texture.
    /// </summary>
    public static unsafe SDL_PropertiesID GetProperties(this IntPtr<SDL_Texture> ptr)
    {
        return SDL_GetTextureProperties(ptr);
    }

    /// <summary>
    /// Gets the scale mode of a texture.
    /// </summary>
    public static unsafe SDL_ScaleMode GetScaleMode(this IntPtr<SDL_Texture> ptr)
    {
        SDL_ScaleMode result;
        SDL_GetTextureScaleMode(ptr, &result)
            .AssertSdlSuccess();
        return result;
    }

    /// <summary>
    /// Gets the size of a texture, in pixels.
    /// </summary>
    public static unsafe (float W, float H) GetSize(this IntPtr<SDL_Texture> ptr)
    {
        float w, h;
        SDL_GetTextureSize(ptr, &w, &h)
            .AssertSdlSuccess();
        return (w, h);
    }

    /// <summary>
    /// Sets the alpha mod value of a texture.
    /// </summary>
    public static unsafe void SetAlphaMod(this IntPtr<SDL_Texture> ptr, byte alpha)
    {
        SDL_SetTextureAlphaMod(ptr, alpha)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets the alpha mod value of a texture.
    /// </summary>
    public static unsafe void SetAlphaModFloat(this IntPtr<SDL_Texture> ptr, float alpha)
    {
        SDL_SetTextureAlphaModFloat(ptr, alpha)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Gets the blend mode of a texture.
    /// </summary>
    public static unsafe bool SetBlendMode(this IntPtr<SDL_Texture> ptr, SDL_BlendMode mode)
    {
        return SDL_SetTextureBlendMode(ptr, mode);
    }

    /// <summary>
    /// Sets the scale mode of a texture.
    /// </summary>
    public static unsafe void SetScaleMode(this IntPtr<SDL_Texture> ptr, SDL_ScaleMode mode)
    {
        SDL_SetTextureScaleMode(ptr, mode)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets the color mod values of a texture.
    /// </summary>
    public static unsafe void SetColorMod(this IntPtr<SDL_Texture> ptr, SDL_Color color)
    {
        SDL_SetTextureColorMod(ptr, color.r, color.g, color.b)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets the color mod values of a texture.
    /// </summary>
    public static unsafe void SetColorMod(this IntPtr<SDL_Texture> ptr, byte r, byte g, byte b)
    {
        SDL_SetTextureColorMod(ptr, r, g, b)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets the color mod values of a texture.
    /// </summary>
    public static unsafe void SetColorModFloat(this IntPtr<SDL_Texture> ptr, SDL_FColor color)
    {
        SDL_SetTextureColorModFloat(ptr, color.r, color.g, color.b)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets the color mod values of a texture.
    /// </summary>
    public static unsafe void SetColorModFloat(this IntPtr<SDL_Texture> ptr, float r, float g, float b)
    {
        SDL_SetTextureColorModFloat(ptr, r, g, b)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Lock a portion of the texture for write-only pixel access, and expose it as a SDL surface.
    /// </summary>
    /// <param name="texture">
    /// The texture to lock for access, which must be created with
    /// <see cref="SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING"/>.
    /// </param>
    /// <param name="rect">
    /// The rectangle to lock for access. If null, the entire texture will be locked.
    /// </param>
    /// <returns>
    /// A new surface the same size as the rectangle. Don't assume any specific pixel content.
    /// </returns>
    /// <remarks>
    /// Besides providing an <see cref="SDL_Surface"/> instead of raw pixel data, this function operates like
    /// <see cref="Lock"/>.
    /// As an optimization, the pixels made available for editing don't necessarily contain the old texture data.
    /// This is a write-only operation, and if you need to keep a copy of the texture data you should do that at
    /// the application level.
    /// 
    /// You must use <see cref="Unlock"/> to unlock the pixels and apply any changes.
    /// 
    /// The returned surface is freed internally after calling <see cref="Unlock"/> or <see cref="Destroy"/>. The
    /// caller should not free it.
    /// </remarks>
    public static unsafe IntPtr<SDL_Surface> LockToSurface(
        this IntPtr<SDL_Texture> texture,
        SDL_Rect? rect
    )
    {
        SDL_Surface* surface;
        SDL_LockTextureToSurface(texture, rect is { } sr ? &sr : null, &surface)
            .AssertSdlSuccess();
        return surface;
    }

    /// <summary>
    /// Lock a portion of the texture for write-only pixel access.
    /// </summary>
    /// <param name="texture">
    /// The texture to lock for access, which must be created with
    /// <see cref="SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING"/>.
    /// </param>
    /// <param name="rect">
    /// The rectangle to lock for access. If null, the entire texture will be locked.
    /// </param>
    /// <returns>
    /// Pixels: the locked pixels, appropriately offset by the locked area.
    /// Pitch: the length of one row in bytes.
    /// </returns>
    /// <remarks>
    /// As an optimization, the pixels made available for editing don't necessarily contain the old texture data.
    /// This is a write-only operation, and if you need to keep a copy of the texture data you should do that at
    /// the application level.
    /// 
    /// You must use <see cref="Unlock"/> to unlock the pixels and apply any changes.
    /// </remarks>
    public static unsafe (IntPtr<byte> Pixels, int Pitch) Lock(
        this IntPtr<SDL_Texture> texture,
        SDL_Rect? rect
    )
    {
        IntPtr pixels;
        int pitch;
        SDL_LockTexture(texture, rect is { } sr ? &sr : null, &pixels, &pitch)
            .AssertSdlSuccess();
        return (pixels, pitch);
    }

    /// <summary>
    /// Unlocks a texture, uploading the changes to video memory, if needed.
    /// </summary>
    /// <param name="texture">
    /// </param>
    /// <remarks>
    /// Note that <see cref="Lock"/> is intended to be write-only; it will not guarantee the previous contents
    /// of the texture will be provided. You must fully initialize any area of a texture that you lock before
    /// unlocking it, as the pixels might otherwise be uninitialized memory.
    /// Consequently, locking and immediately unlocking a texture can result in corrupted textures,
    /// depending on the renderer in use.
    /// </remarks>
    public static unsafe void Unlock(
        this IntPtr<SDL_Texture> texture
    )
    {
        SDL_UnlockTexture(texture);
    }

    /// <summary>
    /// Gets a texture's <see cref="SDL_Colorspace"/>.
    /// </summary>
    public static unsafe SDL_Colorspace? GetColorSpace(
        this IntPtr<SDL_Texture> texture
    )
    {
        return SDL_GetTextureProperties(texture)
            .TryGetNumber(SDL_PROP_TEXTURE_COLORSPACE_NUMBER, out var val)
            ? (SDL_Colorspace)val
            : null;
    }

    /// <summary>
    /// Gets the <see cref="SDL_TextureAccess"/> used during a texture's creation.
    /// </summary>
    public static unsafe SDL_TextureAccess? GetAccess(
        this IntPtr<SDL_Texture> texture
    )
    {
        return SDL_GetTextureProperties(texture)
            .TryGetNumber(SDL_PROP_TEXTURE_ACCESS_NUMBER, out var val)
            ? (SDL_TextureAccess)val
            : null;
    }

    /// <summary>
    /// For HDR10 and floating point textures, this defines the value of 100% diffuse white,
    /// with higher values being displayed in the High Dynamic Range headroom. This defaults
    /// to 100 for HDR10 textures and 1.0 for other textures.
    /// </summary>
    public static unsafe float? GetSdrWhitePoint(
        this IntPtr<SDL_Texture> texture
    )
    {
        return SDL_GetTextureProperties(texture)
            .TryGetFloat(SDL_PROP_TEXTURE_SDR_WHITE_POINT_FLOAT, out var val)
            ? val
            : null;
    }

    /// <summary>
    /// For HDR10 and floating point textures, this defines the maximum dynamic range used by
    /// the content, in terms of the SDR white point. If this is defined, any values outside
    /// the range supported by the display will be scaled into the available HDR headroom,
    /// otherwise they are clipped. This defaults to 1.0 for SDR textures, 4.0 for HDR10 textures,
    /// and no default for floating point textures.
    /// </summary>
    public static unsafe float? GetHdrHeadroom(
        this IntPtr<SDL_Texture> texture
    )
    {
        return SDL_GetTextureProperties(texture)
            .TryGetFloat(SDL_PROP_TEXTURE_HDR_HEADROOM_FLOAT, out var val)
            ? val
            : null;
    }

    /// <summary>
    /// For textures within the SDL GPU context, gets the associated <see cref="SDL_GPUTexture"/> reference.
    /// </summary>
    public static unsafe IntPtr<SDL_GPUTexture> GetGpuTexture(
        this IntPtr<SDL_Texture> texture
    )
    {
        return SDL_GetTextureProperties(texture)
            .TryGetPointer(SDL_PROP_TEXTURE_GPU_TEXTURE_POINTER, out var val)
            ? val
            : null;
    }

    /// <summary>
    /// Updates the given texture rectangle with new pixel data.
    /// </summary>
    /// <param name="texture">
    /// The texture to update.
    /// </param>
    /// <param name="rect">
    /// A rectangle representing the area to update, or null to update the entire texture.
    /// </param>
    /// <param name="pixels">
    /// The raw pixel data in the format of the texture.
    /// </param>
    /// <typeparam name="TPixel">
    /// Concrete type of pixel data.
    /// </typeparam>
    /// <remarks>
    /// The pixel data must be in the pixel format of the texture, which can be queried using the
    /// Format property. This is a fairly slow function, intended for use with static textures that do not change
    /// often. If the texture is intended to be updated often, it is preferred to create the texture as streaming
    /// and you should use other locking functions. While this function will work with streaming textures,
    /// for optimization reasons you may not get the pixels back if you lock the texture afterward.
    /// </remarks>
    public static unsafe void UpdateTexture<TPixel>(
        this IntPtr<SDL_Texture> texture,
        SDL_Rect? rect,
        ReadOnlySpan<TPixel> pixels
    ) where TPixel : unmanaged
    {
        var pitch = sizeof(TPixel) * (rect is { } r ? r.w : texture.AsReadOnlyRef().w);
        fixed (void* pixelsPtr = pixels)
        {
            SDL_UpdateTexture(
                texture,
                rect == null ? null : &r,
                (IntPtr)pixelsPtr,
                pitch
            ).AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Updates the given texture rectangle with new pixel data.
    /// </summary>
    /// <param name="texture">
    /// The texture to update.
    /// </param>
    /// <param name="rect">
    /// A rectangle representing the area to update, or null to update the entire texture.
    /// </param>
    /// <param name="pixels">
    /// The raw pixel data in the format of the texture.
    /// </param>
    /// <param name="pitch">
    /// The number of bytes between the start of two pixel rows.
    /// </param>
    /// <remarks>
    /// The pixel data must be in the pixel format of the texture, which can be queried using the
    /// Format property. This is a fairly slow function, intended for use with static textures that do not change
    /// often. If the texture is intended to be updated often, it is preferred to create the texture as streaming
    /// and you should use other locking functions. While this function will work with streaming textures,
    /// for optimization reasons you may not get the pixels back if you lock the texture afterward.
    /// </remarks>
    public static unsafe void UpdateTexture(
        this IntPtr<SDL_Texture> texture,
        SDL_Rect? rect,
        ReadOnlySpan<byte> pixels,
        int pitch
    )
    {
        fixed (void* pixelsPtr = pixels)
        {
            SDL_UpdateTexture(
                texture,
                rect is { } dr ? &dr : null,
                (IntPtr)pixelsPtr,
                pitch
            ).AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Updates a rectangle within a planar YV12 or IYUV texture with new pixel data.
    /// </summary>
    /// <param name="texture">
    /// The texture to update.
    /// </param>
    /// <param name="rect">
    /// The rectangle of pixels to update, or null to update the entire texture.
    /// </param>
    /// <param name="yPlane">
    /// The raw pixel data for the Y plane.
    /// </param>
    /// <param name="yPitch">
    /// The number of bytes between rows of pixel data for the Y plane.
    /// </param>
    /// <param name="uPlane">
    /// The raw pixel data for the U plane.
    /// </param>
    /// <param name="uPitch">
    /// The number of bytes between rows of pixel data for the U plane.
    /// </param>
    /// <param name="vPlane">
    /// The raw pixel data for the V plane.
    /// </param>
    /// <param name="vPitch">
    /// The number of bytes between rows of pixel data for the V plane.
    /// </param>
    public static unsafe void UpdateYuvTexture(
        this IntPtr<SDL_Texture> texture,
        SDL_Rect? rect,
        ReadOnlySpan<byte> yPlane,
        int yPitch,
        ReadOnlySpan<byte> uPlane,
        int uPitch,
        ReadOnlySpan<byte> vPlane,
        int vPitch
    )
    {
        fixed (byte* yPlanePtr = yPlane)
        fixed (byte* uPlanePtr = uPlane)
        fixed (byte* vPlanePtr = vPlane)
        {
            SDL_UpdateYUVTexture(
                texture,
                rect is { } dr ? &dr : null,
                yPlanePtr,
                yPitch,
                uPlanePtr,
                uPitch,
                vPlanePtr,
                vPitch
            ).AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Updates a rectangle within a planar NV12 or NV21 texture with new pixels.
    /// </summary>
    /// <param name="texture">
    /// The texture to update.
    /// </param>
    /// <param name="rect">
    /// The rectangle of pixels to update, or null to update the entire texture.
    /// </param>
    /// <param name="yPlane">
    /// The raw pixel data for the Y plane.
    /// </param>
    /// <param name="yPitch">
    /// The number of bytes between rows of pixel data for the Y plane.
    /// </param>
    /// <param name="uvPlane">
    /// The raw pixel data for the UV plane.
    /// </param>
    /// <param name="uvPitch">
    /// The number of bytes between rows of pixel data for the UV plane.
    /// </param>
    public static unsafe void UpdateNvTexture(
        this IntPtr<SDL_Texture> texture,
        SDL_Rect? rect,
        ReadOnlySpan<byte> yPlane,
        int yPitch,
        ReadOnlySpan<byte> uvPlane,
        int uvPitch
    )
    {
        fixed (byte* yPlanePtr = yPlane)
        fixed (byte* uvPlanePtr = uvPlane)
        {
            SDL_UpdateNVTexture(
                texture,
                rect is { } dr ? &dr : null,
                yPlanePtr,
                yPitch,
                uvPlanePtr,
                uvPitch
            ).AssertSdlSuccess();
        }
    }
}