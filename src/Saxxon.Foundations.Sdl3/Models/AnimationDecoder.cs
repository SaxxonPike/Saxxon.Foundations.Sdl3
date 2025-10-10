using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="IMG_AnimationDecoder"/>.
/// </summary>
[PublicAPI]
public static class AnimationDecoder
{
    /// <summary>
    /// Creates a decoder to read a series of images from a file.
    /// </summary>
    /// <param name="name">
    /// The file containing a series of images.
    /// </param>
    public static unsafe IntPtr<IMG_AnimationDecoder> Create(
        ReadOnlySpan<char> name
    )
    {
        using var nameStr = new UnmanagedString(name);
        return ((IntPtr<IMG_AnimationDecoder>)IMG_CreateAnimationDecoder(nameStr))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Creates a decoder to read a series of images from an IOStream.
    /// </summary>
    /// <param name="dst">
    /// An <see cref="SDL_IOStream"/> containing a series of images.
    /// </param>
    /// <param name="closeIo">
    /// True to close the stream when done, false to leave it open.
    /// </param>
    /// <param name="type">
    /// A filename extension that represent this data ("WEBP", etc).
    /// </param>
    public static unsafe IntPtr<IMG_AnimationDecoder> CreateIo(
        IntPtr<SDL_IOStream> dst,
        bool closeIo,
        ReadOnlySpan<char> type
    )
    {
        using var typeStr = new UnmanagedString(type);
        return ((IntPtr<IMG_AnimationDecoder>)IMG_CreateAnimationDecoder_IO(dst, closeIo, typeStr))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Creates an animation decoder with the specified properties.
    /// </summary>
    /// <param name="props">
    /// The properties of the animation decoder.
    /// </param>
    public static unsafe IntPtr<IMG_AnimationDecoder> CreateWithProperties(
        SDL_PropertiesID props
    )
    {
        return ((IntPtr<IMG_AnimationDecoder>)IMG_CreateAnimationDecoderWithProperties(props))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Close an animation decoder, finishing any decoding.
    /// </summary>
    /// <param name="decoder">
    /// The decoder to close.
    /// </param>
    public static unsafe void Close(
        this IntPtr<IMG_AnimationDecoder> decoder
    )
    {
        IMG_CloseAnimationDecoder(decoder)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Gets the next frame in an animation decoder.
    /// </summary>
    /// <param name="decoder">
    /// The animation decoder.
    /// </param>
    /// <returns>
    /// The next frame, or null if there are no more frames.
    /// </returns>
    /// <remarks>
    /// This function decodes the next frame in the animation decoder,
    /// returning it as an <see cref="SDL_Surface"/>. The returned surface
    /// should be freed with <see cref="Surface.Destroy"/> when no longer
    /// needed.
    /// </remarks>
    public static unsafe (IntPtr<SDL_Surface> Surface, ulong Duration)? GetFrame(
        this IntPtr<IMG_AnimationDecoder> decoder
    )
    {
        SDL_Surface* surface = null;
        var duration = 0UL;
        var result = IMG_GetAnimationDecoderFrame(decoder, &surface, &duration);

        if (result) 
            return ((IntPtr<SDL_Surface>)surface, duration);

        SdlException.ThrowIfError();
        return null;
    }

    /// <summary>
    /// Gets the properties of an animation decoder.
    /// </summary>
    /// <param name="decoder">
    /// The animation decoder.
    /// </param>
    /// <remarks>
    /// This function returns the properties of the animation decoder, which
    /// holds information about the underlying image such as description,
    /// copyright text and loop count using IMG_PROP_METADATA property names.
    /// </remarks>
    public static unsafe SDL_PropertiesID? GetProperties(
        this IntPtr<IMG_AnimationDecoder> decoder
    )
    {
        var result = IMG_GetAnimationDecoderProperties(decoder);
        if (result != 0)
            return result;
        
        SdlException.ThrowIfError();
        return null;
    }

    /// <summary>
    /// Gets the decoder status indicating the current state of the decoder.
    /// </summary>
    /// <param name="decoder">
    /// The decoder to get the status of.
    /// </param>
    /// <returns>
    /// The status of the underlying decoder, or
    /// <see cref="IMG_AnimationDecoderStatus.IMG_DECODER_STATUS_INVALID"/> if
    /// the given decoder is invalid.
    /// </returns>
    public static unsafe IMG_AnimationDecoderStatus GetStatus(
        this IntPtr<IMG_AnimationDecoder> decoder
    )
    {
        return IMG_GetAnimationDecoderStatus(decoder);
    }

    /// <summary>
    /// Resets an animation decoder.
    /// </summary>
    /// <param name="decoder">
    /// The decoder to reset.
    /// </param>
    /// <remarks>
    /// Calling this function resets the animation decoder, allowing it to
    /// start from the beginning again. This is useful if you want to decode
    /// the frame sequence again without creating a new decoder.
    /// </remarks>
    public static unsafe void Reset(
        this IntPtr<IMG_AnimationDecoder> decoder
    )
    {
        IMG_ResetAnimationDecoder(decoder)
            .AssertSdlSuccess();
    }
}