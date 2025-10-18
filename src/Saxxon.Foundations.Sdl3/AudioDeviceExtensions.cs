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
public static class AudioDeviceExtensions
{
    /// <summary>
    /// Extensions for <see cref="SDL_AudioDeviceID"/>.
    /// </summary>
    extension(SDL_AudioDeviceID)
    {
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
    }

    /// <summary>
    /// Extensions for <see cref="SDL_AudioDeviceID"/> values.
    /// </summary>
    extension(SDL_AudioDeviceID id)
    {
        /// <summary>
        /// Returns true if an audio device is paused.
        /// </summary>
        public bool IsPaused() =>
            SDL_AudioDevicePaused(id);

        /// <summary>
        /// Binds an audio stream to an audio device.
        /// </summary>
        public unsafe void Bind(IntPtr<SDL_AudioStream> stream) =>
            SDL_BindAudioStream(id, stream)
                .AssertSdlSuccess();

        /// <summary>
        /// Binds multiple audio streams to an audio device.
        /// </summary>
        public unsafe void Bind(params ReadOnlySpan<IntPtr<SDL_AudioStream>> streams)
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
        public void Close() =>
            SDL_CloseAudioDevice(id);

        /// <summary>
        /// Gets the channel map for an audio device.
        /// </summary>
        public unsafe IMemoryOwner<int> GetChannelMap()
        {
            int count;
            var result = ((IntPtr<int>)SDL_GetAudioDeviceChannelMap(id, &count))
                .AssertSdlNotNull();
            return SdlMemoryManager.Owned(result, count);
        }

        /// <summary>
        /// Gets the audio format for an audio device.
        /// </summary>
        public unsafe (SDL_AudioSpec Format, int SampleFrames) GetFormat()
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
        public float GetGain() =>
            SDL_GetAudioDeviceGain(id)
                .AssertSdlNotEqual(-1f);

        /// <summary>
        /// Gets the name of an audio device.
        /// </summary>
        /// <returns></returns>
        public unsafe string? GetName()
        {
            var result = (IntPtr<byte>)Unsafe_SDL_GetAudioDeviceName(id);
            return result.GetString();
        }

        /// <summary>
        /// Returns true if the audio device is non-virtual.
        /// </summary>
        public bool IsPhysical() =>
            SDL_IsAudioDevicePhysical(id);

        /// <summary>
        /// Returns true if the audio device is for audio playback.
        /// </summary>
        public bool IsPlayback() =>
            SDL_IsAudioDevicePlayback(id);

        /// <summary>
        /// Opens an audio device.
        /// </summary>
        public unsafe SDL_AudioDeviceID Open(
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
        public void Pause() =>
            SDL_PauseAudioDevice(id)
                .AssertSdlSuccess();

        /// <summary>
        /// Resumes an audio device from the paused state.
        /// </summary>
        public void Resume() =>
            SDL_ResumeAudioDevice(id);

        /// <summary>
        /// Sets the level of gain of an audio device.
        /// </summary>
        public void SetGain(float gain) =>
            SDL_SetAudioDeviceGain(id, gain);

        /// <summary>
        /// Sets the global callback for when audio data is about to be sent to an
        /// audio device.
        /// </summary>
        public unsafe void SetPostMixCallback(
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
}