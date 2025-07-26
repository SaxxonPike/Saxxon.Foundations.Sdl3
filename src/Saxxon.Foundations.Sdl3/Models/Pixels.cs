using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for uncategorized SDL pixel functions.
/// </summary>
[PublicAPI]
public static class Pixels
{
    /// <summary>
    /// Converts raw pixels of one format to another format.
    /// </summary>
    public static unsafe void Convert(
        int width,
        int height,
        SDL_PixelFormat srcFormat,
        ReadOnlySpan<byte> src,
        int srcPitch,
        SDL_PixelFormat dstFormat,
        ReadOnlySpan<byte> dst,
        int dstPitch
    )
    {
        fixed (byte* srcPtr = src)
        fixed (byte* dstPtr = dst)
        {
            SDL_ConvertPixels(
                width,
                height,
                srcFormat,
                (IntPtr)srcPtr,
                srcPitch,
                dstFormat,
                (IntPtr)dstPtr,
                dstPitch
            ).AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Converts raw pixels of one format to another format.
    /// </summary>
    public static unsafe void ConvertWithColorSpace(
        int width,
        int height,
        SDL_PixelFormat srcFormat,
        SDL_Colorspace srcColorSpace,
        SDL_PropertiesID srcProperties,
        ReadOnlySpan<byte> src,
        int srcPitch,
        SDL_PixelFormat dstFormat,
        SDL_Colorspace dstColorSpace,
        SDL_PropertiesID dstProperties,
        ReadOnlySpan<byte> dst,
        int dstPitch
    )
    {
        fixed (byte* srcPtr = src)
        fixed (byte* dstPtr = dst)
        {
            SDL_ConvertPixelsAndColorspace(
                width,
                height,
                srcFormat,
                srcColorSpace,
                srcProperties,
                (IntPtr)srcPtr,
                srcPitch,
                dstFormat,
                dstColorSpace,
                dstProperties,
                (IntPtr)dstPtr,
                dstPitch
            ).AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Performs alpha pre-multiplication on raw pixel data.
    /// </summary>
    public static unsafe void PremultiplyAlpha(
        int width,
        int height,
        SDL_PixelFormat srcFormat,
        ReadOnlySpan<byte> src,
        int srcPitch,
        SDL_PixelFormat dstFormat,
        ReadOnlySpan<byte> dst,
        int dstPitch,
        bool linear
    )
    {
        fixed (byte* srcPtr = src)
        fixed (byte* dstPtr = dst)
        {
            SDL_PremultiplyAlpha(
                width,
                height,
                srcFormat,
                (IntPtr)srcPtr,
                srcPitch,
                dstFormat,
                (IntPtr)dstPtr,
                dstPitch,
                linear
            ).AssertSdlSuccess();
        }
    }
}