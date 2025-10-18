using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="IMG_Animation"/>.
/// </summary>
[PublicAPI]
public static class AnimationExtensions
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
    ) => ((IntPtr<IMG_Animation>)IMG_LoadAnimation_IO(src, closeIo))
        .AssertSdlNotNull();

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
    /// Loads an APNG animation directly.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> LoadApngIo(
        IntPtr<SDL_IOStream> src
    ) => ((IntPtr<IMG_Animation>)IMG_LoadAPNGAnimation_IO(src))
        .AssertSdlNotNull();

    /// <summary>
    /// Loads an AVIF animation directly.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> LoadAvifIo(
        IntPtr<SDL_IOStream> src
    ) => ((IntPtr<IMG_Animation>)IMG_LoadAVIFAnimation_IO(src))
        .AssertSdlNotNull();

    /// <summary>
    /// Loads a GIF animation directly.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> LoadGifIo(
        IntPtr<SDL_IOStream> src
    ) => ((IntPtr<IMG_Animation>)IMG_LoadGIFAnimation_IO(src))
        .AssertSdlNotNull();

    /// <summary>
    /// Loads a WEBP animation directly.
    /// </summary>
    public static unsafe IntPtr<IMG_Animation> LoadWebpIo(
        IntPtr<SDL_IOStream> src
    ) => ((IntPtr<IMG_Animation>)IMG_LoadWEBPAnimation_IO(src))
        .AssertSdlNotNull();

    /// <summary>
    /// Extension methods for <see cref="IMG_Animation"/>.
    /// </summary>
    extension(IntPtr<IMG_Animation> animation)
    {
        /// <summary>
        /// Gets the frame surfaces.
        /// </summary>
        public unsafe IntPtr<IntPtr<SDL_Surface>> Frames =>
            (IntPtr<IntPtr<SDL_Surface>>)animation.AsReadOnlyRef().frames;

        /// <summary>
        /// Dispose of an <see cref="IMG_Animation"/> and free its resources.
        /// </summary>
        public unsafe void Free() =>
            IMG_FreeAnimation(animation);

        /// <summary>
        /// Gets the frame surfaces.
        /// </summary>
        public unsafe ReadOnlySpan<IntPtr<SDL_Surface>> FramesSpan =>
            new(
                animation.AsReadOnlyRef().frames,
                animation.AsReadOnlyRef().count
            );

        /// <summary>
        /// Gets the delay time for the frames.
        /// </summary>
        public unsafe IntPtr<int> Delays =>
            animation.AsReadOnlyRef().delays;

        /// <summary>
        /// Gets the delay time for the frames.
        /// </summary>
        public unsafe ReadOnlySpan<int> DelaysSpan =>
            new(
                animation.AsReadOnlyRef().delays,
                animation.AsReadOnlyRef().count
            );

        /// <summary>
        /// Gets the width of the frames.
        /// </summary>
        public int Width =>
            animation.AsReadOnlyRef().w;

        /// <summary>
        /// Gets the height of the frames.
        /// </summary>
        public int Height =>
            animation.AsReadOnlyRef().h;

        /// <summary>
        /// Gets the two-dimensional size of the frames.
        /// </summary>
        public SDL_Size Size =>
            Size.Create(
                animation.AsReadOnlyRef().w,
                animation.AsReadOnlyRef().h
            );

        /// <summary>
        /// Gets the number of animation frames.
        /// </summary>
        public int FrameCount =>
            animation.AsReadOnlyRef().count;
    }
}