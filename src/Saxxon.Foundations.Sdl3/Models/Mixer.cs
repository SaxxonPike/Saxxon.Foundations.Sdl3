using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="MIX_Mixer"/>.
/// </summary>
[PublicAPI]
public static class Mixer
{
    public static IntPtr<MIX_Mixer> Default => 0;

    public static unsafe IntPtr<MIX_Mixer> Create(
        SDL_AudioSpec spec
    )
    {
        return ((IntPtr<MIX_Mixer>)MIX_CreateMixer(&spec))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<MIX_Mixer> CreateDevice(
        SDL_AudioDeviceID device,
        SDL_AudioSpec spec
    )
    {
        return ((IntPtr<MIX_Mixer>)MIX_CreateMixerDevice(device, &spec))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<MIX_Group> CreateGroup(
        this IntPtr<MIX_Mixer> mixer
    )
    {
        return ((IntPtr<MIX_Group>)MIX_CreateGroup(mixer))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<MIX_Track> CreateTrack(
        this IntPtr<MIX_Mixer> mixer
    )
    {
        return ((IntPtr<MIX_Track>)MIX_CreateTrack(mixer))
            .AssertSdlNotNull();
    }

    public static unsafe void Destroy(
        this IntPtr<MIX_Mixer> mixer
    )
    {
        MIX_DestroyMixer(mixer);
    }

    public static unsafe void Generate(
        this IntPtr<MIX_Mixer> mixer,
        Span<byte> buffer
    )
    {
        fixed (byte* bufferPtr = buffer)
        {
            MIX_Generate(mixer, (IntPtr)bufferPtr, buffer.Length);
        }
    }

    public static unsafe float GetMasterGain(
        this IntPtr<MIX_Mixer> mixer
    )
    {
        return MIX_GetMasterGain(mixer);
    }

    public static unsafe SDL_AudioSpec GetFormat(
        this IntPtr<MIX_Mixer> mixer
    )
    {
        SDL_AudioSpec result;

        MIX_GetMixerFormat(mixer, &result)
            .AssertSdlSuccess();

        return result;
    }

    public static unsafe SDL_PropertiesID GetProperties(
        this IntPtr<MIX_Mixer> mixer
    )
    {
        var result = MIX_GetMixerProperties(mixer);
        if (result == 0)
            throw new SdlException();
        return result;
    }

    public static unsafe void PauseAllTracks(
        this IntPtr<MIX_Mixer> mixer
    )
    {
        MIX_PauseAllTracks(mixer)
            .AssertSdlSuccess();
    }

    public static unsafe void PauseTag(
        this IntPtr<MIX_Mixer> mixer,
        ReadOnlySpan<char> tag
    )
    {
        using var tagStr = new UnmanagedString(tag);
        MIX_PauseTag(mixer, tagStr.Ptr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Plays a sound directly on a mixer without specifying a track.
    /// </summary>
    /// <param name="audio">
    /// Audio to play.
    /// </param>
    /// <param name="mixer">
    /// Mixer to use for playback.
    /// </param>
    public static unsafe void PlayAudio(
        this IntPtr<MIX_Mixer> mixer,
        IntPtr<MIX_Audio> audio
    )
    {
        MIX_PlayAudio(mixer, audio)
            .AssertSdlSuccess();
    }

    public static unsafe void PlayTag(
        this IntPtr<MIX_Mixer> mixer,
        ReadOnlySpan<char> tag,
        SDL_PropertiesID options = 0
    )
    {
        using var tagStr = new UnmanagedString(tag);
        MIX_PlayTag(mixer, tagStr.Ptr, options)
            .AssertSdlSuccess();
    }

    public static unsafe void ResumeAllTracks(
        this IntPtr<MIX_Mixer> mixer
    )
    {
        MIX_ResumeAllTracks(mixer)
            .AssertSdlSuccess();
    }

    public static unsafe void ResumeTag(
        this IntPtr<MIX_Mixer> mixer,
        ReadOnlySpan<char> tag
    )
    {
        using var tagStr = new UnmanagedString(tag);
        MIX_ResumeTag(mixer, tagStr.Ptr)
            .AssertSdlSuccess();
    }

    public static unsafe void SetMasterGain(
        this IntPtr<MIX_Mixer> mixer,
        float gain
    )
    {
        MIX_SetMasterGain(mixer, gain)
            .AssertSdlSuccess();
    }

    public static unsafe void SetTagGain(
        this IntPtr<MIX_Mixer> mixer,
        ReadOnlySpan<char> tag,
        float gain
    )
    {
        using var tagStr = new UnmanagedString(tag);
        MIX_SetTagGain(mixer, tagStr.Ptr, gain)
            .AssertSdlSuccess();
    }

    public static unsafe void StopAllTracks(
        this IntPtr<MIX_Mixer> mixer,
        TimeSpan fadeOut
    )
    {
        MIX_StopAllTracks(mixer, (long)fadeOut.TotalMilliseconds)
            .AssertSdlSuccess();
    }

    public static unsafe void StopTag(
        this IntPtr<MIX_Mixer> mixer,
        ReadOnlySpan<char> tag,
        TimeSpan fadeOut
    )
    {
        using var tagStr = new UnmanagedString(tag);
        MIX_StopTag(mixer, tagStr.Ptr, (long)fadeOut.TotalMilliseconds)
            .AssertSdlSuccess();
    }
}