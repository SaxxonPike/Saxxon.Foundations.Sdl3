using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="Mix_ChannelId"/>.
/// </summary>
[PublicAPI]
public static class Channel
{
    public static void SetFinishedCallback(ChannelFinishedCallbackFunction func)
    {
        if (func.Handler != null)
            ChannelFinishedCallbackFunction.Add(func.Handler);
    }

    public static int Expire(
        this Mix_ChannelId channel,
        int ticks
    )
    {
        return Mix_ExpireChannel((int)channel, ticks);
    }

    public static unsafe int FadeIn(
        this Mix_ChannelId channel,
        IntPtr<Mix_Chunk> chunk,
        int loops,
        int ms
    )
    {
        return Mix_FadeInChannel((int)channel, chunk, loops, ms);
    }

    public static unsafe int FadeInTimed(
        this Mix_ChannelId channel,
        IntPtr<Mix_Chunk> chunk,
        int loops,
        int ms,
        int ticks
    )
    {
        return Mix_FadeInChannelTimed((int)channel, chunk, loops, ms, ticks);
    }

    public static Mix_Fading IsFading(this Mix_ChannelId channel)
    {
        return Mix_FadingChannel((int)channel);
    }

    public static bool IsPlaying(this Mix_ChannelId channel)
    {
        return Mix_Playing((int)channel) > 0;
    }

    public static int GetCountInUse()
    {
        return Mix_Playing(-1);
    }

    public static int GetCountInUse(this Mix_ChannelId channel)
    {
        return Mix_Playing((int)channel);
    }

    public static unsafe IntPtr<Mix_Chunk> GetChunk(this Mix_ChannelId channel)
    {
        return Mix_GetChunk((int)channel);
    }

    public static unsafe List<string> GetChunkDecoders()
    {
        var count = Mix_GetNumChunkDecoders();
        var result = new List<string>(count);

        for (var i = 0; i < count; i++)
            result.Add(Marshal.PtrToStringUTF8((IntPtr)Mix_GetChunkDecoder(i))!);

        return result;
    }

    public static unsafe Mix_ChannelId? Play(
        this Mix_ChannelId channel,
        IntPtr<Mix_Chunk> chunk,
        int loops
    )
    {
        var result = Mix_PlayChannel((int)channel, chunk, loops);
        return result < 0 ? null : (Mix_ChannelId)result;
    }

    public static int Reserve(int count)
    {
        return Mix_ReserveChannels(count);
    }

    public static void SetDistance(this Mix_ChannelId channel, float distance)
    {
        Mix_SetDistance((int)channel, (byte)(Math.Clamp(distance, 0, 1) * 255))
            .AssertSdlSuccess();
    }

    public static void SetPanning(
        this Mix_ChannelId channel,
        float panning
    )
    {
        var rightValue = (byte)(Math.Clamp(panning, 0, 1) * 255);
        var leftValue = (byte)(255 - rightValue);
        Mix_SetPanning((int)channel, leftValue, rightValue)
            .AssertSdlSuccess();
    }

    public static void Pause(
        this Mix_ChannelId channel
    )
    {
        Mix_Pause((int)channel);
    }

    public static void PauseAll()
    {
        Mix_Pause(-1);
    }

    public static void Resume(
        this Mix_ChannelId channel
    )
    {
        Mix_Resume((int)channel);
    }

    public static void ResumeAll()
    {
        Mix_Resume(-1);
    }

    public static float GetVolume(this Mix_ChannelId channel)
    {
        return Mix_Volume((int)channel, -1) / (float)MIX_MAX_VOLUME;
    }

    public static void SetVolume(this Mix_ChannelId channel, float volume)
    {
        _ = Mix_Volume((int)channel, (int)(Math.Clamp(volume, 0, 1) * MIX_MAX_VOLUME));
    }

    public static void Halt(this Mix_ChannelId channel)
    {
        Mix_HaltChannel((int)channel);
    }
}