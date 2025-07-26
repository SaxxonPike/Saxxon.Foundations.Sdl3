using System.Buffers;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_AudioStream"/>.
/// </summary>
[PublicAPI]
public static class AudioStream
{
    /// <summary>
    /// Returns true if the audio device that an audio stream is bound to is
    /// in the paused state.
    /// </summary>
    public static unsafe bool DeviceIsPaused(this IntPtr<SDL_AudioStream> ptr)
    {
        return SDL_AudioStreamDevicePaused(ptr);
    }

    /// <summary>
    /// Binds an audio stream to an audio device.
    /// </summary>
    public static unsafe void Bind(this IntPtr<SDL_AudioStream> ptr, SDL_AudioDeviceID device)
    {
        SDL_BindAudioStream(device, ptr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Clears any pending data from an audio stream.
    /// </summary>
    public static unsafe void Clear(this IntPtr<SDL_AudioStream> ptr)
    {
        SDL_ClearAudioStream(ptr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Frees an audio stream.
    /// </summary>
    public static unsafe void Destroy(this IntPtr<SDL_AudioStream> ptr)
    {
        SDL_DestroyAudioStream(ptr);
    }

    /// <summary>
    /// Processes any pending data in an audio stream.
    /// </summary>
    public static unsafe void Flush(this IntPtr<SDL_AudioStream> ptr)
    {
        SDL_FlushAudioStream(ptr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Get the number of bytes available in an audio stream's internal buffer.
    /// </summary>
    public static unsafe int GetAvailable(this IntPtr<SDL_AudioStream> ptr)
    {
        return SDL_GetAudioStreamAvailable(ptr)
            .AssertSdlNotEqual(-1);
    }

    /// <summary>
    /// Gets raw data from an audio stream.
    /// </summary>
    public static unsafe int GetData<T>(this IntPtr<SDL_AudioStream> ptr, Span<T> buffer)
        where T : unmanaged
    {
        var bytes = MemoryMarshal.AsBytes(buffer);
        fixed (void* bufferPtr = bytes)
        {
            return SDL_GetAudioStreamData(
                ptr,
                (IntPtr)bufferPtr,
                bytes.Length
            ).AssertSdlNotEqual(-1);
        }
    }

    /// <summary>
    /// Gets the audio device that an audio stream is associated to.
    /// </summary>
    public static unsafe SDL_AudioDeviceID GetDevice(this IntPtr<SDL_AudioStream> ptr)
    {
        return SDL_GetAudioStreamDevice(ptr);
    }

    /// <summary>
    /// Gets the audio format of an audio stream.
    /// </summary>
    public static unsafe (SDL_AudioSpec Src, SDL_AudioSpec Dst) GetFormat(this IntPtr<SDL_AudioStream> ptr)
    {
        SDL_AudioSpec src, dst;
        SDL_GetAudioStreamFormat(ptr, &src, &dst)
            .AssertSdlSuccess();
        return (src, dst);
    }

    /// <summary>
    /// Gets the rate multiplier for an audio stream.
    /// </summary>
    public static unsafe float GetFrequencyRatio(this IntPtr<SDL_AudioStream> ptr)
    {
        return SDL_GetAudioStreamFrequencyRatio(ptr)
            .AssertSdlNotEqual(0f);
    }

    /// <summary>
    /// Gets the amount of gain for an audio stream.
    /// </summary>
    public static unsafe float GetGain(this IntPtr<SDL_AudioStream> ptr)
    {
        return SDL_GetAudioStreamGain(ptr)
            .AssertSdlNotEqual(-1f);
    }

    /// <summary>
    /// Gets the map of input channels from an audio stream.
    /// </summary>
    public static unsafe IMemoryOwner<int>? GetInputChannelMap(this IntPtr<SDL_AudioStream> ptr)
    {
        int count;
        var result = (IntPtr)SDL_GetAudioStreamInputChannelMap(ptr, &count);
        return result == IntPtr.Zero
            ? null
            : SdlMemoryPool<int>.Shared.Own((void*)result, count);
    }

    /// <summary>
    /// Gets the map of output channels from an audio stream.
    /// </summary>
    public static unsafe IMemoryOwner<int>? GetOutputChannelMap(this IntPtr<SDL_AudioStream> ptr)
    {
        int count;
        var result = (IntPtr)SDL_GetAudioStreamOutputChannelMap(ptr, &count);
        return result == IntPtr.Zero
            ? null
            : SdlMemoryPool<int>.Shared.Own((void*)result, count);
    }

    /// <summary>
    /// Gets the property set for an audio stream.
    /// </summary>
    public static unsafe SDL_PropertiesID GetProperties(this IntPtr<SDL_AudioStream> ptr)
    {
        return SDL_GetAudioStreamProperties(ptr);
    }

    /// <summary>
    /// Gets the number of bytes that are queued to be processed by an audio
    /// stream.
    /// </summary>
    public static unsafe int GetQueued(this IntPtr<SDL_AudioStream> ptr)
    {
        return SDL_GetAudioStreamQueued(ptr)
            .AssertSdlNotEqual(-1);
    }

    /// <summary>
    /// Assigns a mutex to an audio stream.
    /// </summary>
    public static unsafe void Lock(this IntPtr<SDL_AudioStream> ptr)
    {
        SDL_LockAudioStream(ptr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Writes raw audio data to an audio stream.
    /// </summary>
    public static unsafe void PutData<T>(this IntPtr<SDL_AudioStream> ptr, ReadOnlySpan<T> buffer)
        where T : unmanaged
    {
        var bytes = MemoryMarshal.AsBytes(buffer);
        fixed (void* bufferPtr = bytes)
        {
            SDL_PutAudioStreamData(ptr, (IntPtr)bufferPtr, bytes.Length)
                .AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Sets the audio format for an audio stream.
    /// </summary>
    public static unsafe void SetFormat(this IntPtr<SDL_AudioStream> ptr, SDL_AudioSpec? src, SDL_AudioSpec? dst)
    {
        SDL_SetAudioStreamFormat(
            ptr,
            src is { } srcValue ? &srcValue : null,
            dst is { } dstValue ? &dstValue : null
        ).AssertSdlSuccess();
    }

    /// <summary>
    /// Sets the rate multiplier for an audio stream.
    /// </summary>
    public static unsafe void SetFrequencyRatio(this IntPtr<SDL_AudioStream> ptr, float ratio)
    {
        SDL_SetAudioStreamFrequencyRatio(ptr, ratio)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets the amount of gain for an audio stream.
    /// </summary>
    public static unsafe void SetGain(this IntPtr<SDL_AudioStream> ptr, float gain)
    {
        SDL_SetAudioStreamGain(ptr, gain)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets the input channel map for an audio stream.
    /// </summary>
    public static unsafe void SetInputChannelMap(this IntPtr<SDL_AudioStream> ptr, ReadOnlySpan<int> map)
    {
        fixed (int* mapPtr = map)
        {
            SDL_SetAudioStreamInputChannelMap(ptr, mapPtr, map.Length)
                .AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Sets the output channel map for an audio stream.
    /// </summary>
    public static unsafe void SetOutputChannelMap(this IntPtr<SDL_AudioStream> ptr, ReadOnlySpan<int> map)
    {
        fixed (int* mapPtr = map)
        {
            SDL_SetAudioStreamOutputChannelMap(ptr, mapPtr, map.Length)
                .AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Unbinds the audio stream from its associated audio device.
    /// </summary>
    /// <param name="ptr"></param>
    public static unsafe void Unbind(this IntPtr<SDL_AudioStream> ptr)
    {
        SDL_UnbindAudioStream(ptr);
    }

    /// <summary>
    /// Removes a mutex from the audio stream.
    /// </summary>
    /// <param name="ptr"></param>
    public static unsafe void Unlock(this IntPtr<SDL_AudioStream> ptr)
    {
        SDL_UnlockAudioStream(ptr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Creates a new audio stream.
    /// </summary>
    public static unsafe IntPtr<SDL_AudioStream> Create(
        SDL_AudioSpec? src,
        SDL_AudioSpec? dst)
    {
        return ((IntPtr)SDL_CreateAudioStream(
            src is { } srcVal ? &srcVal : null,
            dst is { } dstVal ? &dstVal : null
        )).AssertSdlNotNull();
    }

    /// <summary>
    /// Assigns a callback that will be invoked when more data is requested
    /// from an audio stream.
    /// </summary>
    public static unsafe void SetGetCallback(
        this IntPtr<SDL_AudioStream> ptr,
        AudioStreamCallbackFunction callback
    )
    {
        SDL_SetAudioStreamGetCallback(
            ptr,
            AudioStreamCallbackFunction.Callback,
            callback.UserData
        ).AssertSdlSuccess();
    }
}