using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_AudioDeviceID"/>.
/// </summary>
[PublicAPI]
public static class AudioDevice
{
    /// <summary>
    /// Returns true if an audio device is paused.
    /// </summary>
    public static bool IsPaused(this SDL_AudioDeviceID id)
    {
        return SDL_AudioDevicePaused(id);
    }

    /// <summary>
    /// Binds an audio stream to an audio device.
    /// </summary>
    public static unsafe void Bind(this SDL_AudioDeviceID id, IntPtr<SDL_AudioStream> stream)
    {
        SDL_BindAudioStream(id, stream)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Binds multiple audio streams to an audio device.
    /// </summary>
    public static unsafe void Bind(this SDL_AudioDeviceID id, params ReadOnlySpan<IntPtr<SDL_AudioStream>> streams)
    {
        fixed (void* streamPtrs = streams)
        {
            SDL_BindAudioStreams(id, (SDL_AudioStream**)streamPtrs, streams.Length)
                .AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Closes an audio device.
    /// </summary>
    public static void Close(this SDL_AudioDeviceID id)
    {
        SDL_CloseAudioDevice(id);
    }

    /// <summary>
    /// Gets the channel map for an audio device.
    /// </summary>
    public static unsafe IMemoryOwner<int> GetChannelMap(this SDL_AudioDeviceID id)
    {
        int count;
        var result = ((IntPtr<int>)SDL_GetAudioDeviceChannelMap(id, &count))
            .AssertSdlNotNull();
        return SdlMemoryManager.Owned(result, count);
    }

    /// <summary>
    /// Gets the audio format for an audio device.
    /// </summary>
    public static unsafe (SDL_AudioSpec Format, int SampleFrames) GetFormat(this SDL_AudioDeviceID id)
    {
        SDL_AudioSpec result;
        int sampleFrames;
        SDL_GetAudioDeviceFormat(id, &result, &sampleFrames)
            .AssertSdlSuccess();

        return (result, sampleFrames);
    }

    /// <summary>
    /// Gets the current gain level for an audio device.
    /// </summary>
    public static float GetGain(this SDL_AudioDeviceID id)
    {
        return SDL_GetAudioDeviceGain(id)
            .AssertSdlNotEqual(-1f);
    }

    /// <summary>
    /// Gets the name of an audio device.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static unsafe string? GetName(this SDL_AudioDeviceID id)
    {
        var result = (IntPtr<byte>)Unsafe_SDL_GetAudioDeviceName(id);
        return result.GetString();
    }

    /// <summary>
    /// Gets all playback audio devices.
    /// </summary>
    public static unsafe IMemoryOwner<SDL_AudioDeviceID> GetPlayback()
    {
        int count;
        var result = ((IntPtr<SDL_AudioDeviceID>)SDL_GetAudioPlaybackDevices(&count))
            .AssertSdlNotNull();
        return SdlMemoryManager.Owned(result, count);
    }

    /// <summary>
    /// Gets all recording audio devices.
    /// </summary>
    public static unsafe IMemoryOwner<SDL_AudioDeviceID> GetRecording()
    {
        int count;
        var result = ((IntPtr<SDL_AudioDeviceID>)SDL_GetAudioRecordingDevices(&count))
            .AssertSdlNotNull();
        return SdlMemoryManager.Owned(result, count);
    }

    /// <summary>
    /// Returns true if the audio device is non-virtual.
    /// </summary>
    public static bool IsPhysical(this SDL_AudioDeviceID id)
    {
        return SDL_IsAudioDevicePhysical(id);
    }

    /// <summary>
    /// Returns true if the audio device is for audio playback.
    /// </summary>
    public static bool IsPlayback(this SDL_AudioDeviceID id)
    {
        return SDL_IsAudioDevicePlayback(id);
    }

    /// <summary>
    /// Opens an audio device.
    /// </summary>
    public static unsafe SDL_AudioDeviceID Open(
        SDL_AudioDeviceID id,
        SDL_AudioSpec? format
    )
    {
        var spec = format ?? default;
        var result = SDL_OpenAudioDevice(id, &spec);

        return result == 0 ? throw new SdlException() : result;
    }

    /// <summary>
    /// Pauses an audio device.
    /// </summary>
    public static void Pause(this SDL_AudioDeviceID id)
    {
        SDL_PauseAudioDevice(id)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Resumes an audio device from the paused state.
    /// </summary>
    public static void Resume(this SDL_AudioDeviceID id)
    {
        SDL_ResumeAudioDevice(id);
    }

    /// <summary>
    /// Sets the level of gain of an audio device.
    /// </summary>
    public static void SetGain(this SDL_AudioDeviceID id, float gain)
    {
        SDL_SetAudioDeviceGain(id, gain);
    }

    /// <summary>
    /// Sets the global callback for when audio data is about to be sent to an
    /// audio device.
    /// </summary>
    public static unsafe void SetPostMixCallback(
        this SDL_AudioDeviceID id,
        AudioDevicePostMixCallbackFunction callback
    )
    {
        SDL_SetAudioPostmixCallback(
            id,
            AudioDevicePostMixCallbackFunction.Callback,
            callback.UserData
        ).AssertSdlSuccess();
    }
}