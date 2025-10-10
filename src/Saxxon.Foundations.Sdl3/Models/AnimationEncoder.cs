using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="IMG_AnimationEncoder"/>.
/// </summary>
[PublicAPI]
public static class AnimationEncoder
{
    /// <summary>
    /// Creates an encoder to save a series of images to a file.
    /// </summary>
    /// <param name="name">
    /// The file where the animation will be saved.
    /// </param>
    /// <remarks>
    /// The file type is determined from the file extension, e.g. "file.webp"
    /// will be encoded using WEBP.
    /// </remarks>
    public static unsafe IntPtr<IMG_AnimationEncoder> Create(
        ReadOnlySpan<char> name
    )
    {
        using var nameStr = new UnmanagedString(name);
        return ((IntPtr<IMG_AnimationEncoder>)IMG_CreateAnimationEncoder(nameStr))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Creates an encoder to save a series of images to an
    /// <see cref="SDL_IOStream"/>.
    /// </summary>
    /// <param name="dst">
    /// An <see cref="SDL_IOStream"/> that will be used to save the stream.
    /// </param>
    /// <param name="closeIo">
    /// True to close the SDL_IOStream when done, false to leave it open.
    /// </param>
    /// <param name="type">
    /// A filename extension that represent this data ("WEBP", etc).
    /// </param>
    /// <remarks>
    /// If closeIo is true, the stream will be closed before returning if this
    /// function fails, or when the animation encoder is closed if this
    /// function succeeds.
    /// </remarks>
    public static unsafe IntPtr<IMG_AnimationEncoder> CreateIo(
        IntPtr<SDL_IOStream> dst,
        bool closeIo,
        ReadOnlySpan<char> type
    )
    {
        using var typeStr = new UnmanagedString(type);
        return ((IntPtr<IMG_AnimationEncoder>)IMG_CreateAnimationEncoder_IO(dst, closeIo, typeStr))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Creates an animation encoder with the specified properties.
    /// </summary>
    /// <param name="props">
    /// The properties of the animation encoder.
    /// </param>
    /// <remarks>
    /// Only IMG_PROP_ANIMATION_ENCODER_CREATE properties are valid.
    /// </remarks>
    public static unsafe IntPtr<IMG_AnimationEncoder> CreateWithProperties(
        SDL_PropertiesID props
    )
    {
        return ((IntPtr<IMG_AnimationEncoder>)IMG_CreateAnimationEncoderWithProperties(props))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Adds a frame to an animation encoder.
    /// </summary>
    /// <param name="encoder">
    /// The encoder receiving images.
    /// </param>
    /// <param name="surface">
    /// The surface to add as the next frame in the animation.
    /// </param>
    /// <param name="duration">
    /// The duration of the frame, usually in milliseconds but can be other
    /// units if the IMG_PROP_ANIMATION_ENCODER_CREATE_TIMEBASE_DENOMINATOR_NUMBER
    /// property is set when creating the encoder.
    /// </param>
    public static unsafe void AddFrame(this IntPtr<IMG_AnimationEncoder> encoder, IntPtr<SDL_Surface> surface,
        ulong duration)
    {
        IMG_AddAnimationEncoderFrame(encoder, surface, duration)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Closes an animation encoder, finishing any encoding.
    /// </summary>
    /// <param name="encoder">
    /// The encoder to close.
    /// </param>
    public static unsafe void Close(this IntPtr<IMG_AnimationEncoder> encoder)
    {
        IMG_CloseAnimationEncoder(encoder)
            .AssertSdlSuccess();
    }
}