using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class AnimationDecoder
{
    public static unsafe IntPtr<IMG_AnimationDecoder> Create(ReadOnlySpan<char> name)
    {
        using var nameStr = new UnmanagedString(name);
        return ((IntPtr<IMG_AnimationDecoder>)IMG_CreateAnimationDecoder(nameStr))
            .AssertSdlNotNull();
    }

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

    public static unsafe IntPtr<IMG_AnimationDecoder> CreateWithProperties(SDL_PropertiesID props)
    {
        return ((IntPtr<IMG_AnimationDecoder>)IMG_CreateAnimationDecoderWithProperties(props))
            .AssertSdlNotNull();
    }

    public static unsafe void Close(this IntPtr<IMG_AnimationDecoder> decoder)
    {
        IMG_CloseAnimationDecoder(decoder)
            .AssertSdlSuccess();
    }

    public static unsafe (IntPtr<SDL_Surface> Surface, ulong Duration) GetFrame(
        this IntPtr<IMG_AnimationDecoder> decoder
    )
    {
        SDL_Surface* surface = null;
        var duration = 0UL;
        IMG_GetAnimationDecoderFrame(decoder, &surface, &duration)
            .AssertSdlSuccess();
        return ((IntPtr<SDL_Surface>)surface, duration);
    }

    public static unsafe SDL_PropertiesID GetProperties(this IntPtr<IMG_AnimationDecoder> decoder)
    {
        return IMG_GetAnimationDecoderProperties(decoder);
    }

    public static unsafe IMG_AnimationDecoderStatus GetStatus(this IntPtr<IMG_AnimationDecoder> decoder)
    {
        return IMG_GetAnimationDecoderStatus(decoder);
    }

    public static unsafe void Reset(this IntPtr<IMG_AnimationDecoder> decoder)
    {
        IMG_ResetAnimationDecoder(decoder)
            .AssertSdlSuccess();
    }
}