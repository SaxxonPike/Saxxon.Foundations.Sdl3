using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="MIX_AudioDecoder"/>.
/// </summary>
[PublicAPI]
public static class AudioDecoder
{
    public static unsafe IntPtr<MIX_AudioDecoder> Create(
        ReadOnlySpan<char> path,
        SDL_PropertiesID props
    )
    {
        using var pathStr = new UnmanagedString(path);
        return ((IntPtr<MIX_AudioDecoder>)MIX_CreateAudioDecoder(pathStr.Ptr, props))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<MIX_AudioDecoder> CreateIo(
        IntPtr<SDL_IOStream> stream,
        bool closeIo,
        SDL_PropertiesID props
    )
    {
        return ((IntPtr<MIX_AudioDecoder>)MIX_CreateAudioDecoder_IO(stream, closeIo, props))
            .AssertSdlNotNull();
    }

    public static unsafe Span<byte> Decode(
        this IntPtr<MIX_AudioDecoder> decoder,
        Span<byte> buffer,
        SDL_AudioSpec spec
    )
    {
        fixed (byte* bufferPtr = buffer)
        {
            var result = MIX_DecodeAudio(decoder, (IntPtr)bufferPtr, buffer.Length, &spec)
                .AssertSdlNotEqual(-1);
            return buffer[..result];
        }
    }

    public static unsafe void Destroy(this IntPtr<MIX_AudioDecoder> decoder)
    {
        MIX_DestroyAudioDecoder(decoder);
    }

    public static unsafe IntPtr<MIX_AudioDecoder> Get(int index)
    {
        return ((IntPtr<MIX_AudioDecoder>)MIX_GetAudioDecoder(index))
            .AssertSdlNotNull();
    }

    public static unsafe int GetCount()
    {
        return MIX_GetNumAudioDecoders();
    }

    public static unsafe SDL_AudioSpec GetFormat(this IntPtr<MIX_AudioDecoder> decoder)
    {
        SDL_AudioSpec result;

        MIX_GetAudioDecoderFormat(decoder, &result)
            .AssertSdlSuccess();

        return result;
    }

    public static unsafe SDL_PropertiesID GetProperties(this IntPtr<MIX_AudioDecoder> decoder)
    {
        var result = MIX_GetAudioDecoderProperties(decoder);
        if (result == 0)
            throw new SdlException();
        return result;
    }
}