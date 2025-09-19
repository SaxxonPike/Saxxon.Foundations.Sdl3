using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Texture"/>.
/// </summary>
[PublicAPI]
public static class Texture
{
    /// <summary>
    /// Gets the width of a texture, in pixels.
    /// </summary>
    public static int GetWidth(this IntPtr<SDL_Texture> ptr)
    {
        return ptr.IsNull ? 0 : ptr.AsReadOnlyRef().w;
    }

    /// <summary>
    /// Gets the height of a texture, in pixels.
    /// </summary>
    public static int GetHeight(this IntPtr<SDL_Texture> ptr)
    {
        return ptr.IsNull ? 0 : ptr.AsReadOnlyRef().h;
    }

    /// <summary>
    /// Gets the pixel format of a texture.
    /// </summary>
    public static SDL_PixelFormat GetFormat(this IntPtr<SDL_Texture> ptr)
    {
        return ptr.IsNull ? 0 : ptr.AsReadOnlyRef().format;
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
        return ((IntPtr)SDL_CreateTexture(renderer, format, access, w, h))
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
        return ((IntPtr)SDL_CreateTextureFromSurface(renderer, surface))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Texture> CreateWithProperties(
        IntPtr<SDL_Renderer> renderer,
        SDL_PropertiesID props
    )
    {
        return ((IntPtr)SDL_CreateTextureWithProperties(renderer, props))
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
        return ((IntPtr)SDL_GetRendererFromTexture(ptr))
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
    public static unsafe void SetBlendMode(this IntPtr<SDL_Texture> ptr, SDL_BlendMode mode)
    {
        SDL_SetTextureBlendMode(ptr, mode)
            .AssertSdlSuccess();
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

    public static unsafe void Unlock(
        this IntPtr<SDL_Texture> texture
    )
    {
        SDL_UnlockTexture(texture);
    }

    public static unsafe SDL_Colorspace GetColorSpace(
        this IntPtr<SDL_Texture> texture
    )
    {
        return (SDL_Colorspace)SDL_GetTextureProperties(texture)
            .GetNumber(SDL_PROP_TEXTURE_COLORSPACE_NUMBER);
    }

    public static unsafe SDL_TextureAccess GetAccess(
        this IntPtr<SDL_Texture> texture
    )
    {
        return (SDL_TextureAccess)SDL_GetTextureProperties(texture)
            .GetNumber(SDL_PROP_TEXTURE_ACCESS_NUMBER);
    }

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