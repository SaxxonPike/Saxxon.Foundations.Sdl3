using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="MIX_Audio"/>.
/// </summary>
[PublicAPI]
public static class Audio
{
    /// <summary>
    /// Create a <see cref="MIX_Audio"/> that generates a sine wave.
    /// </summary>
    /// <param name="mixer">
    /// Mixer the audio is intended to be used with.
    /// </param>
    /// <param name="hz">
    /// Sine wave frequency in hertz.
    /// </param>
    /// <param name="amplitude">
    /// Amplitude of the sine wave from 0.0f to 1.0f.
    /// </param>
    /// <returns></returns>
    public static unsafe IntPtr<MIX_Audio> CreateSineWave(
        IntPtr<MIX_Mixer> mixer,
        int hz,
        float amplitude
    )
    {
        return ((IntPtr<MIX_Audio>)MIX_CreateSineWaveAudio(mixer, hz, amplitude))
            .AssertSdlNotNull();
    }
    
    /// <summary>
    /// Converts sample frames to milliseconds.
    /// </summary>
    /// <param name="sampleRate">
    /// Sample rate of the audio.
    /// </param>
    /// <param name="frames">
    /// Number of frames.
    /// </param>
    /// <returns>
    /// Milliseconds equivalent.
    /// </returns>
    public static long FramesToMs(
        int sampleRate,
        long frames
    )
    {
        return MIX_FramesToMS(sampleRate, frames);
    }

    /// <summary>
    /// Converts sample frames to milliseconds.
    /// </summary>
    /// <param name="audio">
    /// Audio to get the sample rate from.
    /// </param>
    /// <param name="frames">
    /// Number of frames.
    /// </param>
    /// <returns>
    /// Milliseconds equivalent.
    /// </returns>
    public static unsafe long FramesToMs(
        this IntPtr<MIX_Audio> audio,
        long frames
    )
    {
        return MIX_AudioFramesToMS(audio, frames);
    }

    /// <summary>
    /// Converts milliseconds to sample frames.
    /// </summary>
    /// <param name="sampleRate">
    /// Sample rate of the audio.
    /// </param>
    /// <param name="ms">
    /// Number of milliseconds.
    /// </param>
    /// <returns>
    /// Sample frames equivalent.
    /// </returns>
    public static long MsToFrames(
        int sampleRate,
        long ms
    )
    {
        return MIX_MSToFrames(sampleRate, ms);
    }

    /// <summary>
    /// Converts milliseconds to sample frames.
    /// </summary>
    /// <param name="audio">
    /// Audio to get the sample rate from.
    /// </param>
    /// <param name="ms">
    /// Number of milliseconds.
    /// </param>
    /// <returns>
    /// Sample frames equivalent.
    /// </returns>
    public static unsafe long MsToFrames(
        this IntPtr<MIX_Audio> audio,
        long ms
    )
    {
        return MIX_AudioMSToFrames(audio, ms);
    }

    /// <summary>
    /// Destroy the specified audio.
    /// </summary>
    /// <param name="audio">
    /// Audio to destroy.
    /// </param>
    public static unsafe void Destroy(
        this IntPtr<MIX_Audio> audio
    )
    {
        MIX_DestroyAudio(audio);
    }

    /// <summary>
    /// Get the length of a <see cref="MIX_Audio"/>'s playback in sample frames.
    /// </summary>
    /// <param name="audio">
    /// Audio to query.
    /// </param>
    /// <returns>
    /// Length in sample frames. A value of <see cref="MIX_DURATION_UNKNOWN"/>
    /// is returned when the length cannot be determined. A value of
    /// <see cref="MIX_DURATION_INFINITE"/> is returned when the audio is to
    /// play indefinitely.
    /// </returns>
    public static unsafe long GetDuration(
        this IntPtr<MIX_Audio> audio
    )
    {
        return MIX_GetAudioDuration(audio);
    }

    /// <summary>
    /// Gets the internal format of a <see cref="MIX_Audio"/>.
    /// </summary>
    /// <param name="audio">
    /// Audio to query.
    /// </param>
    /// <returns>
    /// <see cref="SDL_AudioSpec"/> representing the internal audio format.
    /// </returns>
    public static unsafe SDL_AudioSpec GetFormat(
        this IntPtr<MIX_Audio> audio
    )
    {
        SDL_AudioSpec result;
        MIX_GetAudioFormat(audio, &result)
            .AssertSdlSuccess();
        return result;
    }

    /// <summary>
    /// Gets the property set for the specified <see cref="MIX_Audio"/>.
    /// </summary>
    /// <param name="audio">
    /// Audio to query.
    /// </param>
    /// <returns>
    /// Properties that were found.
    /// </returns>
    public static unsafe SDL_PropertiesID GetProperties(
        this IntPtr<MIX_Audio> audio
    )
    {
        var result = MIX_GetAudioProperties(audio);
        if (result == 0)
            throw new SdlException();
        return result;
    }

    /// <summary>
    /// Loads (and optionally converts) audio data into memory.
    /// </summary>
    /// <param name="mixer">
    /// Mixer the audio is intended to be used with.
    /// </param>
    /// <param name="path">
    /// File path of the audio data.
    /// </param>
    /// <param name="preDecode">
    /// If true, all audio data will be decoded immediately instead of
    /// as-needed.
    /// </param>
    /// <returns>
    /// Loaded audio data.
    /// </returns>
    public static unsafe IntPtr<MIX_Audio> Load(
        IntPtr<MIX_Mixer> mixer,
        ReadOnlySpan<char> path,
        bool preDecode
    )
    {
        using var pathStr = new Utf8Span(path);
        return ((IntPtr<MIX_Audio>)MIX_LoadAudio(mixer, pathStr.Ptr, preDecode))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads (and optionally converts) audio data into memory.
    /// </summary>
    /// <param name="mixer">
    /// Mixer the audio is intended to be used with.
    /// </param>
    /// <param name="stream">
    /// Stream to read the data from.
    /// </param>
    /// <param name="preDecode">
    /// If true, all audio data will be decoded immediately instead of
    /// as-needed.
    /// </param>
    /// <param name="closeIo">
    /// If true, the stream will be closed after reading audio data.
    /// </param>
    /// <returns>
    /// Loaded audio data.
    /// </returns>
    public static unsafe IntPtr<MIX_Audio> LoadIo(
        IntPtr<MIX_Mixer> mixer,
        IntPtr<SDL_IOStream> stream,
        bool preDecode,
        bool closeIo
    )
    {
        return ((IntPtr<MIX_Audio>)MIX_LoadAudio_IO(mixer, stream, preDecode, closeIo))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads audio data into memory using the specified property set.
    /// </summary>
    /// <param name="props">
    /// Property set to use for the audio data.
    /// </param>
    /// <returns>
    /// Loaded audio data.
    /// </returns>
    public static unsafe IntPtr<MIX_Audio> LoadWithProperties(
        SDL_PropertiesID props
    )
    {
        return ((IntPtr<MIX_Audio>)MIX_LoadAudioWithProperties(props))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads raw PCM audio data into memory.
    /// </summary>
    /// <param name="mixer">
    /// Mixer the audio is intended to be used with.
    /// </param>
    /// <param name="data">
    /// Data buffer that contains the raw audio data.
    /// </param>
    /// <param name="spec">
    /// Format of the audio data.
    /// </param>
    /// <returns>
    /// Loaded audio data.
    /// </returns>
    public static unsafe IntPtr<MIX_Audio> LoadRaw(
        IntPtr<MIX_Mixer> mixer,
        ReadOnlySpan<byte> data,
        SDL_AudioSpec spec
    )
    {
        fixed (byte* dataPtr = data)
        {
            return ((IntPtr<MIX_Audio>)MIX_LoadRawAudio(mixer, (IntPtr)dataPtr, (UIntPtr)data.Length, &spec))
                .AssertSdlNotNull();
        }
    }

    /// <summary>
    /// Loads raw PCM audio data into memory.
    /// </summary>
    /// <param name="mixer">
    /// Mixer the audio is intended to be used with.
    /// </param>
    /// <param name="stream">
    /// Stream that contains the raw audio data.
    /// </param>
    /// <param name="spec">
    /// Format of the audio data.
    /// </param>
    /// <param name="closeIo">
    /// If true, the stream will be closed after reading the audio data.
    /// </param>
    /// <returns>
    /// Loaded audio data.
    /// </returns>
    public static unsafe IntPtr<MIX_Audio> LoadRawIo(
        IntPtr<MIX_Mixer> mixer,
        IntPtr<SDL_IOStream> stream,
        SDL_AudioSpec spec,
        bool closeIo
    )
    {
        return ((IntPtr<MIX_Audio>)MIX_LoadRawAudio_IO(mixer, stream, &spec, closeIo))
            .AssertSdlNotNull();
    }

    /// <summary>
    /// Loads raw PCM audio data into memory.
    /// </summary>
    /// <param name="mixer">
    /// Mixer the audio is intended to be used with.
    /// </param>
    /// <param name="data">
    /// Pointer to raw audio data.
    /// </param>
    /// <param name="length">
    /// Length of the audio data, in bytes.
    /// </param>
    /// <param name="spec">
    /// Format of the audio data.
    /// </param>
    /// <param name="freeWhenDone">
    /// If true, <see cref="SDL3.SDL_free(System.IntPtr)"/> will be invoked
    /// on the data pointer when the audio is destroyed.
    /// </param>
    /// <returns>
    /// Loaded audio data.
    /// </returns>
    public static unsafe IntPtr<MIX_Audio> LoadRawNoCopy(
        IntPtr<MIX_Mixer> mixer,
        IntPtr data,
        int length,
        SDL_AudioSpec spec,
        bool freeWhenDone
    )
    {
        return ((IntPtr<MIX_Audio>)MIX_LoadRawAudioNoCopy(mixer, data, (UIntPtr)length, &spec, freeWhenDone))
            .AssertSdlNotNull();
    }
}