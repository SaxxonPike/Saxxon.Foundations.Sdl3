using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for handling image formats via
/// SDL3_image.
/// </summary>
[PublicAPI]
public static class Image
{
    /// <summary>
    /// Loads an image from an SDL data source into a software surface.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadTypedIo(
        IntPtr<SDL_IOStream> src,
        bool closeIo,
        ReadOnlySpan<char> type
    )
    {
        using var typeStr = new UnmanagedString(type);
        return ((IntPtr<SDL_Surface>)IMG_LoadTyped_IO(src, closeIo, typeStr))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an image from a filesystem path into a software surface.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> Load(
        ReadOnlySpan<char> file
    )
    {
        using var fileStr = new UnmanagedString(file);
        return ((IntPtr<SDL_Surface>)IMG_Load(fileStr))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an image from an SDL data source into a software surface.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadIo(
        IntPtr<SDL_IOStream> src,
        bool closeIo
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_Load_IO(src, closeIo))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an AVIF image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadAvifIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadAVIF_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an ICO image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadIcoIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadICO_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a CUR image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadCurIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadCUR_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a BMP image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadBmpIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadBMP_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a GIF image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadGifIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadGIF_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a JPG image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadJpgIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadJPG_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a JXL image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadJxlIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadJXL_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an LBM image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadLbmIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadLBM_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a PCX image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadPcxIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadPCX_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a PNG image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadPngIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadPNG_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a PNM image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadPnmIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadPNM_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an SVG image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadSvgIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadSVG_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a QOI image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadQoiIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadQOI_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a TGA image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadTgaIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadTGA_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a TIF image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadTifIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadTIF_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an XCF image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadXcfIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadXCF_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an XPM image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadXpmIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadXPM_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an XV image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadXvIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadXV_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a WEBP image directly.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadWebpIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadWEBP_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an SVG image, scaled to a specific size.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> LoadSizedSvgIo(
        IntPtr<SDL_IOStream> src,
        int width,
        int height
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_LoadSizedSVG_IO(src, width, height))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an XPM image from a memory array.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> ReadXpmFromArray(
        IntPtr<IntPtr> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_ReadXPMFromArray((byte**)src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an XPM image from a memory array.
    /// </summary>
    public static unsafe IntPtr<SDL_Surface> ReadXpmFromArrayToRgb888(
        IntPtr<IntPtr> src
    )
    {
        return ((IntPtr<SDL_Surface>)IMG_ReadXPMFromArrayToRGB888((byte**)src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Saves an <see cref="SDL_Surface"/> into an image file.
    /// </summary>
    public static unsafe void Save(
        this IntPtr<SDL_Surface> src,
        ReadOnlySpan<char> fileName
    )
    {
        using var fileNameStr = new UnmanagedString(fileName);
        IMG_Save(src, fileNameStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Saves an <see cref="SDL_Surface"/> into an <see cref="SDL_IOStream"/>.
    /// </summary>
    public static unsafe void SaveTypedIo(
        this IntPtr<SDL_Surface> src,
        IntPtr<SDL_IOStream> stream,
        bool closeIo,
        ReadOnlySpan<char> type
    )
    {
        using var typeStr = new UnmanagedString(type);
        IMG_SaveTyped_IO(src, stream, closeIo, typeStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Saves an <see cref="SDL_Surface"/> into an AVIF image file.
    /// </summary>
    public static unsafe void SaveAvif(
        this IntPtr<SDL_Surface> src,
        ReadOnlySpan<char> fileName,
        int quality
    )
    {
        using var fileNameStr = new UnmanagedString(fileName);
        IMG_SaveAVIF(src, fileNameStr, quality)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Save an <see cref="SDL_Surface"/> into AVIF image data, via an
    /// <see cref="SDL_IOStream"/>.
    /// </summary>
    public static unsafe void SaveAvifIo(
        this IntPtr<SDL_Surface> src,
        IntPtr<SDL_IOStream> dst,
        bool closeIo,
        int quality
    )
    {
        IMG_SaveAVIF_IO(src, dst, closeIo, quality)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Saves an <see cref="SDL_Surface"/> into a BMP image file.
    /// </summary>
    public static unsafe void SaveBmp(
        this IntPtr<SDL_Surface> src,
        ReadOnlySpan<char> fileName
    )
    {
        using var fileNameStr = new UnmanagedString(fileName);
        SDL_SaveBMP(src, fileNameStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Save an <see cref="SDL_Surface"/> into BMP image data, via an
    /// <see cref="SDL_IOStream"/>.
    /// </summary>
    public static unsafe void SaveBmpIo(
        this IntPtr<SDL_Surface> src,
        IntPtr<SDL_IOStream> dst,
        bool closeIo
    )
    {
        SDL_SaveBMP_IO(src, dst, closeIo)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Saves an <see cref="SDL_Surface"/> into a CUR image file.
    /// </summary>
    public static unsafe void SaveCur(
        this IntPtr<SDL_Surface> src,
        ReadOnlySpan<char> fileName
    )
    {
        using var fileNameStr = new UnmanagedString(fileName);
        IMG_SaveCUR(src, fileNameStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Save an <see cref="SDL_Surface"/> into CUR image data, via an
    /// <see cref="SDL_IOStream"/>.
    /// </summary>
    public static unsafe void SaveCurIo(
        this IntPtr<SDL_Surface> src,
        IntPtr<SDL_IOStream> dst,
        bool closeIo
    )
    {
        IMG_SaveCUR_IO(src, dst, closeIo)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Saves an <see cref="SDL_Surface"/> into a GIF image file.
    /// </summary>
    public static unsafe void SaveGif(
        this IntPtr<SDL_Surface> src,
        ReadOnlySpan<char> fileName
    )
    {
        using var fileNameStr = new UnmanagedString(fileName);
        IMG_SaveGIF(src, fileNameStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Save an <see cref="SDL_Surface"/> into GIF image data, via an
    /// <see cref="SDL_IOStream"/>.
    /// </summary>
    public static unsafe void SaveGifIo(
        this IntPtr<SDL_Surface> src,
        IntPtr<SDL_IOStream> dst,
        bool closeIo
    )
    {
        IMG_SaveGIF_IO(src, dst, closeIo)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Saves an <see cref="SDL_Surface"/> into an ICO image file.
    /// </summary>
    public static unsafe void SaveIco(
        this IntPtr<SDL_Surface> src,
        ReadOnlySpan<char> fileName
    )
    {
        using var fileNameStr = new UnmanagedString(fileName);
        IMG_SaveICO(src, fileNameStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Save an <see cref="SDL_Surface"/> into ICO image data, via an
    /// <see cref="SDL_IOStream"/>.
    /// </summary>
    public static unsafe void SaveIcoIo(
        this IntPtr<SDL_Surface> src,
        IntPtr<SDL_IOStream> dst,
        bool closeIo
    )
    {
        IMG_SaveICO_IO(src, dst, closeIo)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Saves an <see cref="SDL_Surface"/> into a JPG image file.
    /// </summary>
    public static unsafe void SaveJpg(
        this IntPtr<SDL_Surface> src,
        ReadOnlySpan<char> fileName,
        int quality
    )
    {
        using var fileNameStr = new UnmanagedString(fileName);
        IMG_SaveJPG(src, fileNameStr, quality)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Save an <see cref="SDL_Surface"/> into JPG image data, via an
    /// <see cref="SDL_IOStream"/>.
    /// </summary>
    public static unsafe void SaveJpgIo(
        this IntPtr<SDL_Surface> src,
        IntPtr<SDL_IOStream> dst,
        bool closeIo,
        int quality
    )
    {
        IMG_SaveJPG_IO(src, dst, closeIo, quality)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Saves an <see cref="SDL_Surface"/> into a PNG image file.
    /// </summary>
    public static unsafe void SavePng(
        this IntPtr<SDL_Surface> src,
        ReadOnlySpan<char> fileName
    )
    {
        using var fileNameStr = new UnmanagedString(fileName);
        SDL_SavePNG(src, fileNameStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Save an <see cref="SDL_Surface"/> into PNG image data, via an
    /// <see cref="SDL_IOStream"/>.
    /// </summary>
    public static unsafe void SavePngIo(
        this IntPtr<SDL_Surface> src,
        IntPtr<SDL_IOStream> dst,
        bool closeIo
    )
    {
        SDL_SavePNG_IO(src, dst, closeIo)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Saves an <see cref="SDL_Surface"/> into a TGA image file.
    /// </summary>
    public static unsafe void SaveTga(
        this IntPtr<SDL_Surface> src,
        ReadOnlySpan<char> fileName
    )
    {
        using var fileNameStr = new UnmanagedString(fileName);
        IMG_SaveTGA(src, fileNameStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Save an <see cref="SDL_Surface"/> into TGA image data, via an
    /// <see cref="SDL_IOStream"/>.
    /// </summary>
    public static unsafe void SaveTgaIo(
        this IntPtr<SDL_Surface> src,
        IntPtr<SDL_IOStream> dst,
        bool closeIo
    )
    {
        IMG_SaveTGA_IO(src, dst, closeIo)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Saves an <see cref="SDL_Surface"/> into a WEBP image file.
    /// </summary>
    public static unsafe void SaveWebp(
        this IntPtr<SDL_Surface> src,
        ReadOnlySpan<char> fileName,
        float quality
    )
    {
        using var fileNameStr = new UnmanagedString(fileName);
        IMG_SaveWEBP(src, fileNameStr, quality)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Save an <see cref="SDL_Surface"/> into WEBP image data, via an
    /// <see cref="SDL_IOStream"/>.
    /// </summary>
    public static unsafe void SaveWebpIo(
        this IntPtr<SDL_Surface> src,
        IntPtr<SDL_IOStream> dst,
        bool closeIo,
        float quality
    )
    {
        IMG_SaveWEBP_IO(src, dst, closeIo, quality)
            .AssertSdlSuccess();
    }
}