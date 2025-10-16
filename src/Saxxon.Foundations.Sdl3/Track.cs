using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="MIX_Track"/>.
/// </summary>
[PublicAPI]
public static class Track
{
    public static unsafe void Destroy(
        this IntPtr<MIX_Track> track
    )
    {
        MIX_DestroyTrack(track);
    }

    public static unsafe long FramesToMs(
        this IntPtr<MIX_Track> track,
        long frames
    )
    {
        return MIX_TrackFramesToMS(track, frames);
    }

    public static unsafe MIX_Point3D Get3dPosition(
        this IntPtr<MIX_Track> track
    )
    {
        MIX_Point3D result;
        MIX_GetTrack3DPosition(track, &result)
            .AssertSdlSuccess();
        return result;
    }

    public static unsafe IntPtr<MIX_Audio> GetAudio(
        this IntPtr<MIX_Track> track
    )
    {
        return ((IntPtr<MIX_Audio>)MIX_GetTrackAudio(track))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_AudioStream> GetAudioStream(
        this IntPtr<MIX_Track> track
    )
    {
        return ((IntPtr<SDL_AudioStream>)MIX_GetTrackAudioStream(track))
            .AssertSdlNotNull();
    }

    public static unsafe float GetFrequencyRatio(
        this IntPtr<MIX_Track> track
    )
    {
        return MIX_GetTrackFrequencyRatio(track)
            .AssertSdlNotEqual(0);
    }

    public static unsafe float GetGain(
        this IntPtr<MIX_Track> track
    )
    {
        return MIX_GetTrackGain(track);
    }

    public static unsafe IntPtr<MIX_Mixer> GetMixer(
        this IntPtr<MIX_Track> track
    )
    {
        return ((IntPtr<MIX_Mixer>)MIX_GetTrackMixer(track))
            .AssertSdlNotNull();
    }

    public static unsafe long GetPlaybackPosition(
        this IntPtr<MIX_Track> track
    )
    {
        return MIX_GetTrackPlaybackPosition(track)
            .AssertSdlNotEqual(-1);
    }

    public static unsafe SDL_PropertiesID GetProperties(
        this IntPtr<MIX_Track> track
    )
    {
        var result = MIX_GetTrackProperties(track);
        return result == 0 ? throw new SdlException() : result;
    }

    public static unsafe long? GetRemaining(
        this IntPtr<MIX_Track> track
    )
    {
        return MIX_GetTrackRemaining(track) switch
        {
            < 0 => null,
            var x => x
        };
    }

    public static unsafe bool IsLooping(
        this IntPtr<MIX_Track> track
    )
    {
        return MIX_TrackLooping(track);
    }

    public static unsafe bool IsPaused(
        this IntPtr<MIX_Track> track
    )
    {
        return MIX_TrackPaused(track);
    }

    public static unsafe bool IsPlaying(
        this IntPtr<MIX_Track> track
    )
    {
        return MIX_TrackPlaying(track);
    }

    public static unsafe long MsToFrames(
        this IntPtr<MIX_Track> track,
        long ms
    )
    {
        return MIX_TrackMSToFrames(track, ms);
    }

    public static unsafe void Pause(
        this IntPtr<MIX_Track> track
    )
    {
        MIX_PauseTrack(track)
            .AssertSdlSuccess();
    }

    public static unsafe void Play(
        this IntPtr<MIX_Track> track,
        SDL_PropertiesID props = 0
    )
    {
        MIX_PlayTrack(track, props)
            .AssertSdlSuccess();
    }

    public static unsafe void Resume(
        this IntPtr<MIX_Track> track
    )
    {
        MIX_ResumeTrack(track)
            .AssertSdlSuccess();
    }

    public static unsafe void Set3dPosition(
        this IntPtr<MIX_Track> track,
        MIX_Point3D position
    )
    {
        MIX_SetTrack3DPosition(track, &position)
            .AssertSdlSuccess();
    }

    public static unsafe void SetAudio(
        this IntPtr<MIX_Track> track,
        IntPtr<MIX_Audio> audio
    )
    {
        MIX_SetTrackAudio(track, audio)
            .AssertSdlSuccess();
    }

    public static unsafe void SetAudioStream(
        this IntPtr<MIX_Track> track,
        IntPtr<SDL_AudioStream> stream
    )
    {
        MIX_SetTrackAudioStream(track, stream)
            .AssertSdlSuccess();
    }

    public static unsafe void SetFrequencyRatio(
        this IntPtr<MIX_Track> track,
        float ratio
    )
    {
        MIX_SetTrackFrequencyRatio(track, ratio)
            .AssertSdlSuccess();
    }

    public static unsafe void SetGain(
        this IntPtr<MIX_Track> track,
        float gain
    )
    {
        MIX_SetTrackGain(track, gain)
            .AssertSdlSuccess();
    }

    public static unsafe void SetGroup(
        this IntPtr<MIX_Track> track,
        IntPtr<MIX_Group> group
    )
    {
        MIX_SetTrackGroup(track, group)
            .AssertSdlSuccess();
    }

    public static unsafe void SetIoStream(
        this IntPtr<MIX_Track> track,
        IntPtr<SDL_IOStream> stream,
        bool closeIo
    )
    {
        MIX_SetTrackIOStream(track, stream, closeIo)
            .AssertSdlSuccess();
    }

    public static unsafe void SetOutputChannelMap(
        this IntPtr<MIX_Track> track,
        ReadOnlySpan<int> channelMap
    )
    {
        fixed (int* channelMapPtr = channelMap)
        {
            MIX_SetTrackOutputChannelMap(track, channelMapPtr, channelMap.Length)
                .AssertSdlSuccess();
        }
    }

    public static unsafe void SetPlaybackPosition(
        this IntPtr<MIX_Track> track,
        long frames
    )
    {
        MIX_SetTrackPlaybackPosition(track, frames)
            .AssertSdlSuccess();
    }

    public static unsafe void SetRawIoStream(
        this IntPtr<MIX_Track> track,
        IntPtr<SDL_IOStream> stream,
        SDL_AudioSpec spec,
        bool closeIo
    )
    {
        MIX_SetTrackRawIOStream(track, stream, &spec, closeIo)
            .AssertSdlSuccess();
    }

    public static unsafe void SetStereo(
        this IntPtr<MIX_Track> track,
        MIX_StereoGains? gains
    )
    {
        MIX_SetTrackStereo(
            track,
            gains is { } g ? &g : null
        ).AssertSdlSuccess();
    }

    public static unsafe void SetStoppedCallback(
        this IntPtr<MIX_Track> track,
        TrackStoppedCallback callback
    )
    {
        MIX_SetTrackStoppedCallback(
            track,
            TrackStoppedCallback.Callback,
            callback.UserData
        ).AssertSdlSuccess();
    }

    public static unsafe void Stop(
        this IntPtr<MIX_Track> track,
        TimeSpan? fadeOut
    )
    {
        MIX_StopTrack(
            track,
            MsToFrames(track, fadeOut is { } time ? MsToFrames(track, (long)time.TotalMilliseconds) : 0)
        ).AssertSdlSuccess();
    }

    public static unsafe void Tag(
        this IntPtr<MIX_Track> track,
        ReadOnlySpan<char> tag
    )
    {
        using var tagStr = new UnmanagedString(tag);
        MIX_TagTrack(track, tagStr.Ptr)
            .AssertSdlSuccess();
    }

    public static unsafe void Untag(
        this IntPtr<MIX_Track> track,
        ReadOnlySpan<char> tag
    )
    {
        using var tagStr = new UnmanagedString(tag);
        MIX_UntagTrack(track, tagStr.Ptr);
    }
}