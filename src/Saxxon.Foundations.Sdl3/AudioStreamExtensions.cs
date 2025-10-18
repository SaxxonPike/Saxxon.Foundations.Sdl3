using System.Buffers;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_AudioStream"/>.
/// </summary>
[PublicAPI]
public static class AudioStreamExtensions
{
    /// <summary>
    /// Extensions for <see cref="SDL_AudioStream"/>.
    /// </summary>
    extension(SDL_AudioStream)
    {
        /// <summary>
        /// Creates a new audio stream.
        /// </summary>
        public static unsafe IntPtr<SDL_AudioStream> Create(
            SDL_AudioSpec? src,
            SDL_AudioSpec? dst
        ) => ((IntPtr)SDL_CreateAudioStream(
            src is { } srcVal ? &srcVal : null,
            dst is { } dstVal ? &dstVal : null
        )).AssertSdlNotNull();
    }

    /// <summary>
    /// Extensions for <see cref="SDL_AudioStream"/> references.
    /// </summary>
    extension(IntPtr<SDL_AudioStream> stream)
    {
        /// <summary>
        /// Returns true if the audio device that an audio stream is bound to is
        /// in the paused state.
        /// </summary>
        public unsafe bool DeviceIsPaused =>
            SDL_AudioStreamDevicePaused(stream);

        /// <summary>
        /// Binds an audio stream to an audio device.
        /// </summary>
        public unsafe void Bind(SDL_AudioDeviceID device) =>
            SDL_BindAudioStream(device, stream)
                .AssertSdlSuccess();

        /// <summary>
        /// Clears any pending data from an audio stream.
        /// </summary>
        public unsafe void Clear() =>
            SDL_ClearAudioStream(stream)
                .AssertSdlSuccess();

        /// <summary>
        /// Frees an audio stream.
        /// </summary>
        public unsafe void Destroy() =>
            SDL_DestroyAudioStream(stream);

        /// <summary>
        /// Processes any pending data in an audio stream.
        /// </summary>
        public unsafe void Flush() =>
            SDL_FlushAudioStream(stream)
                .AssertSdlSuccess();

        /// <summary>
        /// Get the number of bytes available in an audio stream's internal buffer.
        /// </summary>
        public unsafe int Available =>
            SDL_GetAudioStreamAvailable(stream)
                .AssertSdlNotEqual(-1);

        /// <summary>
        /// Gets raw data from an audio stream.
        /// </summary>
        public unsafe int GetData<T>(Span<T> buffer)
            where T : unmanaged
        {
            var bytes = MemoryMarshal.AsBytes(buffer);
            fixed (void* bufferPtr = bytes)
            {
                return SDL_GetAudioStreamData(
                    stream,
                    (IntPtr)bufferPtr,
                    bytes.Length
                ).AssertSdlNotEqual(-1);
            }
        }

        /// <summary>
        /// Gets the audio device that an audio stream is associated to.
        /// </summary>
        public unsafe SDL_AudioDeviceID Device =>
            SDL_GetAudioStreamDevice(stream);

        /// <summary>
        /// Gets the audio format of an audio stream.
        /// </summary>
        public unsafe (SDL_AudioSpec Src, SDL_AudioSpec Dst) Format
        {
            get
            {
                SDL_AudioSpec src, dst;
                SDL_GetAudioStreamFormat(stream, &src, &dst)
                    .AssertSdlSuccess();
                return (src, dst);
            }
            set => SetFormat(stream, value.Src, value.Dst);
        }

        /// <summary>
        /// Gets the rate multiplier for an audio stream.
        /// </summary>
        public unsafe float FrequencyRatio
        {
            get => SDL_GetAudioStreamFrequencyRatio(stream)
                .AssertSdlNotEqual(0f);
            set => SDL_SetAudioStreamFrequencyRatio(stream, value)
                .AssertSdlSuccess();
        }

        /// <summary>
        /// Gets the amount of gain for an audio stream.
        /// </summary>
        public unsafe float Gain
        {
            get => SDL_GetAudioStreamGain(stream)
                .AssertSdlNotEqual(-1f);
            set => SDL_SetAudioStreamGain(stream, value)
                .AssertSdlSuccess();
        }

        /// <summary>
        /// Gets the map of input channels from an audio stream.
        /// </summary>
        public unsafe IMemoryOwner<int>? GetInputChannelMap()
        {
            int count;
            var result = (IntPtr<int>)SDL_GetAudioStreamInputChannelMap(stream, &count);
            return result == IntPtr.Zero
                ? null
                : SdlMemoryManager.Owned(result, count);
        }

        /// <summary>
        /// Gets the map of output channels from an audio stream.
        /// </summary>
        public unsafe IMemoryOwner<int>? GetOutputChannelMap()
        {
            int count;
            var result = (IntPtr<int>)SDL_GetAudioStreamOutputChannelMap(stream, &count);
            return result == IntPtr.Zero
                ? null
                : SdlMemoryManager.Owned(result, count);
        }

        /// <summary>
        /// Gets the property set for an audio stream.
        /// </summary>
        public unsafe SDL_PropertiesID Properties =>
            SDL_GetAudioStreamProperties(stream);

        /// <summary>
        /// Gets the number of bytes that are queued to be processed by an audio
        /// stream.
        /// </summary>
        public unsafe int Queued =>
            SDL_GetAudioStreamQueued(stream)
                .AssertSdlNotEqual(-1);

        /// <summary>
        /// Assigns a mutex to an audio stream.
        /// </summary>
        public unsafe void Lock() =>
            SDL_LockAudioStream(stream)
                .AssertSdlSuccess();

        /// <summary>
        /// Writes raw audio data to an audio stream.
        /// </summary>
        public unsafe void PutData<T>(ReadOnlySpan<T> buffer)
            where T : unmanaged
        {
            var bytes = MemoryMarshal.AsBytes(buffer);
            fixed (void* bufferPtr = bytes)
            {
                SDL_PutAudioStreamData(stream, (IntPtr)bufferPtr, bytes.Length)
                    .AssertSdlSuccess();
            }
        }

        /// <summary>
        /// Sets the audio format for an audio stream.
        /// </summary>
        public unsafe void SetFormat(SDL_AudioSpec? src, SDL_AudioSpec? dst) =>
            SDL_SetAudioStreamFormat(
                stream,
                src is { } srcValue ? &srcValue : null,
                dst is { } dstValue ? &dstValue : null
            ).AssertSdlSuccess();

        /// <summary>
        /// Sets the input channel map for an audio stream.
        /// </summary>
        public unsafe void SetInputChannelMap(ReadOnlySpan<int> map)
        {
            fixed (int* mapPtr = map)
            {
                SDL_SetAudioStreamInputChannelMap(stream, mapPtr, map.Length)
                    .AssertSdlSuccess();
            }
        }

        /// <summary>
        /// Sets the output channel map for an audio stream.
        /// </summary>
        public unsafe void SetOutputChannelMap(ReadOnlySpan<int> map)
        {
            fixed (int* mapPtr = map)
            {
                SDL_SetAudioStreamOutputChannelMap(stream, mapPtr, map.Length)
                    .AssertSdlSuccess();
            }
        }

        /// <summary>
        /// Unbinds the audio stream from its associated audio device.
        /// </summary>
        public unsafe void Unbind() =>
            SDL_UnbindAudioStream(stream);

        /// <summary>
        /// Removes a mutex from the audio stream.
        /// </summary>
        public unsafe void Unlock() =>
            SDL_UnlockAudioStream(stream)
                .AssertSdlSuccess();

        /// <summary>
        /// Assigns a callback that will be invoked when more data is requested
        /// from an audio stream.
        /// </summary>
        public unsafe void SetGetCallback(
            AudioStreamCallbackFunction callback
        ) => SDL_SetAudioStreamGetCallback(
            stream,
            AudioStreamCallbackFunction.Callback,
            callback.UserData
        ).AssertSdlSuccess();
    }
}