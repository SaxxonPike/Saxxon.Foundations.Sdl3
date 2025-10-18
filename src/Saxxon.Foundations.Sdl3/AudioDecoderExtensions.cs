using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="MIX_AudioDecoder"/>.
/// </summary>
[PublicAPI]
public static class AudioDecoderExtensions
{
    /// <summary>
    /// Extensions for <see cref="MIX_AudioDecoder"/>.
    /// </summary>
    extension(MIX_AudioDecoder)
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
        ) => ((IntPtr<MIX_AudioDecoder>)MIX_CreateAudioDecoder_IO(stream, closeIo, props))
            .AssertSdlNotNull();

        public static unsafe IntPtr<MIX_AudioDecoder> Get(int index) =>
            ((IntPtr<MIX_AudioDecoder>)MIX_GetAudioDecoder(index))
            .AssertSdlNotNull();

        public static int GetCount() =>
            MIX_GetNumAudioDecoders();
    }

    /// <summary>
    /// Extensions for <see cref="MIX_AudioDecoder"/> references.
    /// </summary>
    extension(IntPtr<MIX_AudioDecoder> decoder)
    {
        public unsafe Span<byte> Decode(
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

        public unsafe void Destroy() =>
            MIX_DestroyAudioDecoder(decoder);

        public unsafe SDL_AudioSpec Format
        {
            get
            {
                SDL_AudioSpec result;

                MIX_GetAudioDecoderFormat(decoder, &result)
                    .AssertSdlSuccess();

                return result;
            }
        }

        public unsafe SDL_PropertiesID Properties
        {
            get
            {
                var result = MIX_GetAudioDecoderProperties(decoder);
                return result == 0 ? throw new SdlException() : result;
            }
        }
    }
}