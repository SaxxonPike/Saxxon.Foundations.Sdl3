using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="Mix_GroupId"/>.
/// </summary>
[PublicAPI]
public static class MixGroup
{
    public static Mix_ChannelId? GetAvailableChannel(this Mix_GroupId tag)
    {
        var result = Mix_GroupAvailable((int)tag);
        if (result >= 0)
            return (Mix_ChannelId)result;
        return null;
    }

    public static void Add(this Mix_GroupId tag, Mix_ChannelId channel)
    {
        Mix_GroupChannel((int)channel, (int)tag)
            .AssertSdlSuccess();
    }

    public static void AddRange(this Mix_GroupId tag, Mix_ChannelId from, Mix_ChannelId to)
    {
        Mix_GroupChannels((int)from, (int)to, (int)tag)
            .AssertSdlSuccess();
    }

    public static int GetChannelCount(this Mix_GroupId tag)
    {
        return Mix_GroupCount((int)tag);
    }

    public static Mix_ChannelId? GetNewestChannel(this Mix_GroupId tag)
    {
        var result = Mix_GroupNewer((int)tag);
        return result >= 0 ? (Mix_ChannelId)result : null;
    }
    
    public static Mix_ChannelId? GetOldestChannel(this Mix_GroupId tag)
    {
        var result = Mix_GroupOldest((int)tag);
        return result >= 0 ? (Mix_ChannelId)result : null;
    }

    public static void Halt(this Mix_GroupId tag)
    {
        Mix_HaltGroup((int)tag);
    }

    public static void Pause(this Mix_GroupId tag)
    {
        Mix_PauseGroup((int)tag);
    }

    public static void Resume(this Mix_GroupId tag)
    {
        Mix_ResumeGroup((int)tag);
    }
    
    
}