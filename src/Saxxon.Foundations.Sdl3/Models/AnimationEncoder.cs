using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class AnimationEncoder
{
    public static unsafe IntPtr<IMG_AnimationEncoder> Create(ReadOnlySpan<char> name)
    {
        using var nameStr = new UnmanagedString(name);
        return ((IntPtr<IMG_AnimationEncoder>)IMG_CreateAnimationEncoder(nameStr))
            .AssertSdlNotNull();
    }

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

    public static unsafe IntPtr<IMG_AnimationEncoder> CreateWithProperties(SDL_PropertiesID props)
    {
        return ((IntPtr<IMG_AnimationEncoder>)IMG_CreateAnimationEncoderWithProperties(props))
            .AssertSdlNotNull();
    }

    public static unsafe void AddFrame(this IntPtr<IMG_AnimationEncoder> encoder, IntPtr<SDL_Surface> surface,
        TimeSpan duration)
    {
        IMG_AddAnimationEncoderFrame(encoder, surface, (ulong)duration.TotalMilliseconds)
            .AssertSdlSuccess();
    }

    public static unsafe void AddFrame(this IntPtr<IMG_AnimationEncoder> encoder, IntPtr<SDL_Surface> surface,
        ulong duration)
    {
        IMG_AddAnimationEncoderFrame(encoder, surface, duration)
            .AssertSdlSuccess();
    }

    public static unsafe void Close(this IntPtr<IMG_AnimationEncoder> encoder)
    {
        IMG_CloseAnimationEncoder(encoder)
            .AssertSdlSuccess();
    }
}