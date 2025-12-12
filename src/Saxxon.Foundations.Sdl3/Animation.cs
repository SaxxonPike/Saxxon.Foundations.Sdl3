using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="IMG_Animation"/>.
/// </summary>
[PublicAPI]
public static class Animation
{
    /// <summary>
    /// Loads an animation from a file.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> Load(
        ReadOnlySpan<char> fileName
    )
    {
        var fileNameLen = fileName.MeasureUtf8();
        Span<byte> fileNameStr = stackalloc byte[fileNameLen];
        fileName.EncodeUtf8(fileNameStr);

        fixed (byte* fileNameStrPtr = fileNameStr)
            return ((IntPtr<IMG_Animation>)IMG_LoadAnimation(fileNameStrPtr))
                .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an animation from an <see cref="SDL_IOStream"/>.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> LoadIo(
        IntPtr<SDL_IOStream> src,
        bool closeIo
    )
    {
        return ((IntPtr<IMG_Animation>)IMG_LoadAnimation_IO(src, closeIo))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an animation from an SDL datasource.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> LoadTypedIo(
        IntPtr<SDL_IOStream> src,
        bool closeIo,
        ReadOnlySpan<char> type)
    {
        var typeLen = type.MeasureUtf8();
        Span<byte> typeStr = stackalloc byte[typeLen];
        type.EncodeUtf8(typeStr);

        fixed (byte* typeStrPtr = typeStr)
            return ((IntPtr<IMG_Animation>)IMG_LoadAnimationTyped_IO(src, closeIo, typeStrPtr))
                .AssertSdlNotNull();
    }

    /// <summary>
    /// Dispose of an <see cref="IMG_Animation"/> and free its resources.
    /// </summary>
    public static unsafe void Free(
        this IntPtr<IMG_Animation> src
    )
    {
        IMG_FreeAnimation(src);
    }

    /// <summary>
    /// Loads an ANI animation directly.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> LoadAniIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<IMG_Animation>)IMG_LoadANIAnimation_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an APNG animation directly.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> LoadApngIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<IMG_Animation>)IMG_LoadAPNGAnimation_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads an AVIF animation directly.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> LoadAvifIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<IMG_Animation>)IMG_LoadAVIFAnimation_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a GIF animation directly.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> LoadGifIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<IMG_Animation>)IMG_LoadGIFAnimation_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads a WEBP animation directly.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> LoadWebpIo(
        IntPtr<SDL_IOStream> src
    )
    {
        return ((IntPtr<IMG_Animation>)IMG_LoadWEBPAnimation_IO(src))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Gets the frame surfaces.
    /// </summary>
    public static unsafe IntPtr<IntPtr<SDL_Surface>> GetFrames(
        this IntPtr<IMG_Animation> animation
    )
    {
        return (IntPtr<IntPtr<SDL_Surface>>)animation.AsReadOnlyRef().frames;
    }

    /// <summary>
    /// Gets the frame surfaces.
    /// </summary>
    public static unsafe ReadOnlySpan<IntPtr<SDL_Surface>> GetFramesSpan(
        this IntPtr<IMG_Animation> animation
    )
    {
        return new ReadOnlySpan<IntPtr<SDL_Surface>>(
            animation.AsReadOnlyRef().frames,
            animation.AsReadOnlyRef().count
        );
    }

    /// <summary>
    /// Gets the delay time for the frames.
    /// </summary>
    public static unsafe IntPtr<int> GetDelays(
        this IntPtr<IMG_Animation> animation
    )
    {
        return animation.AsReadOnlyRef().delays;
    }

    /// <summary>
    /// Gets the delay time for the frames.
    /// </summary>
    public static unsafe ReadOnlySpan<int> GetDelaysSpan(
        this IntPtr<IMG_Animation> animation
    )
    {
        return new ReadOnlySpan<int>(
            animation.AsReadOnlyRef().delays,
            animation.AsReadOnlyRef().count
        );
    }

    /// <summary>
    /// Gets the width of the frames.
    /// </summary>
    /// <param name="animation"></param>
    /// <returns></returns>
    public static int GetWidth(
        this IntPtr<IMG_Animation> animation
    )
    {
        return animation.AsReadOnlyRef().w;
    }

    /// <summary>
    /// Gets the height of the frames.
    /// </summary>
    public static int GetHeight(
        this IntPtr<IMG_Animation> animation
    )
    {
        return animation.AsReadOnlyRef().h;
    }

    /// <summary>
    /// Gets the two-dimensional size of the frames.
    /// </summary>
    public static SDL_Size GetSize(
        this IntPtr<IMG_Animation> animation
    )
    {
        return Size.Create(
            animation.AsReadOnlyRef().w,
            animation.AsReadOnlyRef().h
        );
    }

    /// <summary>
    /// Gets the number of animation frames.
    /// </summary>
    public static int GetCount(
        this IntPtr<IMG_Animation> animation
    )
    {
        return animation.AsReadOnlyRef().count;
    }

    /// <summary>
    /// Saves an animation to a file.
    /// </summary>
    /// <param name="animation">
    /// Animation to save.
    /// </param>
    /// <param name="file">
    /// Path on the filesystem containing an animated image.
    /// </param>
    public static unsafe void Save(
        this IntPtr<IMG_Animation> animation,
        string file
    )
    {
        using var nameStr = new UnmanagedString(file);

        IMG_SaveAnimation(animation, nameStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Saves an animation to an <see cref="SDL_IOStream"/>.
    /// </summary>
    /// <param name="anim">
    /// Animation to save.
    /// </param>
    /// <param name="stream">
    /// Stream that the data will be written to.
    /// </param>
    /// <param name="closeIo">
    /// True to close the stream before returning, false to leave it open.
    /// </param>
    /// <param name="type">
    /// A filename extension that represents this data ("GIF", etc.)
    /// </param>
    public static unsafe void SaveTypedIo(
        this IntPtr<IMG_Animation> anim,
        IntPtr<SDL_IOStream> stream,
        bool closeIo,
        ReadOnlySpan<char> type
    )
    {
        using var typeStr = new UnmanagedString(type);

        IMG_SaveAnimationTyped_IO(anim, stream, closeIo, typeStr)
            .AssertSdlSuccess();
    }

    public static unsafe void SaveAniIo(
        this IntPtr<IMG_Animation> animation,
        IntPtr<SDL_IOStream> stream,
        bool closeIo
    )
    {
        IMG_SaveANIAnimation_IO(animation, stream, closeIo)
            .AssertSdlSuccess();
    }

    public static unsafe void SaveApngIo(
        this IntPtr<IMG_Animation> animation,
        IntPtr<SDL_IOStream> stream,
        bool closeIo
    )
    {
        IMG_SaveAPNGAnimation_IO(animation, stream, closeIo)
            .AssertSdlSuccess();
    }

    public static unsafe void SaveAvifIo(
        this IntPtr<IMG_Animation> animation,
        IntPtr<SDL_IOStream> stream,
        bool closeIo,
        int quality
    )
    {
        IMG_SaveAVIFAnimation_IO(animation, stream, closeIo, quality)
            .AssertSdlSuccess();
    }

    public static unsafe void SaveGifIo(
        this IntPtr<IMG_Animation> animation,
        IntPtr<SDL_IOStream> stream,
        bool closeIo
    )
    {
        IMG_SaveGIFAnimation_IO(animation, stream, closeIo)
            .AssertSdlSuccess();
    }

    public static unsafe void SaveWebpIo(
        this IntPtr<IMG_Animation> animation,
        IntPtr<SDL_IOStream> stream,
        bool closeIo,
        int quality
    )
    {
        IMG_SaveWEBPAnimation_IO(animation, stream, closeIo, quality)
            .AssertSdlSuccess();
    }
}