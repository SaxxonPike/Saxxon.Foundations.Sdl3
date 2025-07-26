using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="Mix_Chunk"/>.
/// </summary>
[PublicAPI]
public static class Chunk
{
    public static unsafe IntPtr<Mix_Chunk> Load(
        ReadOnlySpan<char> fileName
    )
    {
        using var fileNameStr = new Utf8Span(fileName);
        IntPtr<Mix_Chunk> result = Mix_LoadWAV(fileNameStr);
        return result.AssertSdlNotNull();
    }

    public static unsafe IntPtr<Mix_Chunk> LoadIo(
        IntPtr<SDL_IOStream> src,
        bool closeIo
    )
    {
        IntPtr<Mix_Chunk> result = Mix_LoadWAV_IO(src, closeIo);
        return result.AssertSdlNotNull();
    }

    public static unsafe void Free(
        IntPtr<Mix_Chunk> chunk
    )
    {
        Mix_FreeChunk(chunk);
    }

    public static unsafe Mix_ChannelId? Play(
        this IntPtr<Mix_Chunk> chunk,
        int loops
    )
    {
        var result = (Mix_ChannelId)Mix_PlayChannel(-1, chunk, loops);
        return result < 0 ? null : result;
    }

    public static unsafe Mix_ChannelId? Play(
        this IntPtr<Mix_Chunk> chunk,
        Mix_ChannelId channel,
        int loops
    )
    {
        var result = (Mix_ChannelId)Mix_PlayChannel((int)channel, chunk, loops);
        return result < 0 ? null : result;
    }

    public static unsafe float GetVolume(
        this IntPtr<Mix_Chunk> chunk
    )
    {
        return Mix_VolumeChunk(chunk, -1) / (float)MIX_MAX_VOLUME;
    }

    public static unsafe void SetVolume(
        this IntPtr<Mix_Chunk> chunk,
        float volume
    )
    {
        _ = Mix_VolumeChunk(
            chunk,
            (int)(Math.Clamp(volume, 0, 1) * MIX_MAX_VOLUME)
        );
    }

    public static unsafe Mix_ChannelId FadeIn(
        this IntPtr<Mix_Chunk> chunk,
        int loops,
        TimeSpan fadeDuration,
        Mix_ChannelId channel = Mix_ChannelId.Default
    )
    {
        return (Mix_ChannelId)Mix_FadeInChannel(
            (int)channel,
            chunk,
            loops,
            (int)fadeDuration.TotalMilliseconds
        );
    }

    public static void FadeOut(
        TimeSpan fadeDuration
    )
    {
        Mix_FadeOutMusic((int)fadeDuration.TotalMilliseconds)
            .AssertSdlSuccess();
    }

    public static Mix_Fading IsFading()
    {
        return Mix_FadingMusic();
    }
    
    public static bool HasDecoder(ReadOnlySpan<char> name)
    {
        using var nameStr = new Utf8Span(name);
        return Mix_HasChunkDecoder(nameStr);
    }
}