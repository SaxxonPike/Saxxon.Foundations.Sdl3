using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="Mix_Music"/>.
/// </summary>
[PublicAPI]
public static class Music
{
    public static unsafe IntPtr<Mix_Music> Load(
        ReadOnlySpan<char> fileName
    )
    {
        using var fileNameStr = new Utf8Span(fileName);
        IntPtr<Mix_Music> result = Mix_LoadMUS(fileNameStr);
        return result.AssertSdlNotNull();
    }

    public static unsafe IntPtr<Mix_Music> LoadIo(
        IntPtr<SDL_IOStream> src,
        bool closeIo
    )
    {
        IntPtr<Mix_Music> result = Mix_LoadMUS_IO(src, closeIo);
        return result.AssertSdlNotNull();
    }

    public static bool IsPaused()
    {
        return Mix_PausedMusic();
    }

    public static void Pause()
    {
        Mix_PauseMusic();
    }

    public static void Resume()
    {
        Mix_ResumeMusic();
    }

    public static bool IsPlaying()
    {
        return Mix_PlayingMusic();
    }

    public static unsafe void FadeIn(
        this IntPtr<Mix_Music> music,
        int loops,
        TimeSpan fadeDuration
    )
    {
        Mix_FadeInMusic(music, loops, (int)fadeDuration.TotalMilliseconds)
            .AssertSdlSuccess();
    }

    public static unsafe void FadeInPos(
        this IntPtr<Mix_Music> music,
        int loops,
        TimeSpan fadeDuration,
        double position
    )
    {
        Mix_FadeInMusicPos(music, loops, (int)fadeDuration.TotalMilliseconds, position)
            .AssertSdlSuccess();
    }

    public static void FadeOut(TimeSpan fadeDuration)
    {
        Mix_FadeOutMusic((int)fadeDuration.TotalMilliseconds)
            .AssertSdlSuccess();
    }

    public static Mix_Fading IsFading()
    {
        return Mix_FadingMusic();
    }

    public static unsafe void Free(
        this IntPtr<Mix_Music> music
    )
    {
        Mix_FreeMusic(music);
    }

    public static unsafe string GetAlbumTag(
        this IntPtr<Mix_Music> music
    )
    {
        return Marshal.PtrToStringUTF8((IntPtr)Mix_GetMusicAlbumTag(music))!;
    }

    public static unsafe string GetArtistTag(
        this IntPtr<Mix_Music> music
    )
    {
        return Marshal.PtrToStringUTF8((IntPtr)Mix_GetMusicArtistTag(music))!;
    }

    public static unsafe string GetCopyrightTag(
        this IntPtr<Mix_Music> music
    )
    {
        return Marshal.PtrToStringUTF8((IntPtr)Mix_GetMusicCopyrightTag(music))!;
    }

    public static unsafe string GetTitleTag(
        this IntPtr<Mix_Music> music
    )
    {
        return Marshal.PtrToStringUTF8((IntPtr)Mix_GetMusicTitleTag(music))!;
    }

    public static IntPtr GetHookData()
    {
        return Mix_GetMusicHookData();
    }

    public static unsafe double GetLoopEndTime(
        IntPtr<Mix_Music> music
    )
    {
        return Mix_GetMusicLoopEndTime(music);
    }

    public static unsafe double GetLoopLengthTime(
        IntPtr<Mix_Music> music
    )
    {
        return Mix_GetMusicLoopLengthTime(music);
    }

    public static unsafe double GetLoopStartTime(
        IntPtr<Mix_Music> music
    )
    {
        return Mix_GetMusicLoopStartTime(music);
    }

    public static unsafe double GetPosition(
        IntPtr<Mix_Music> music
    )
    {
        return Mix_GetMusicPosition(music);
    }

    public static unsafe string GetTitle(
        IntPtr<Mix_Music> music
    )
    {
        return Marshal.PtrToStringUTF8((IntPtr)Mix_GetMusicTitle(music))!;
    }

    public static unsafe Mix_MusicType GetFormat(
        IntPtr<Mix_Music> music
    )
    {
        return Mix_GetMusicType(music);
    }

    public static unsafe float GetVolume(
        IntPtr<Mix_Music> music
    )
    {
        return Mix_GetMusicVolume(music) / (float)MIX_MAX_VOLUME;
    }

    public static unsafe int GetTrackCount(
        IntPtr<Mix_Music> music
    )
    {
        return Mix_GetNumTracks(music);
    }

    public static void Halt()
    {
        Mix_HaltMusic();
    }

    public static void JumpToModOrder(int order)
    {
        Mix_ModMusicJumpToOrder(order)
            .AssertSdlSuccess();
    }

    public static unsafe double GetDuration(
        this IntPtr<Mix_Music> music
    )
    {
        return Mix_MusicDuration(music);
    }

    public static unsafe void Play(
        this IntPtr<Mix_Music> music,
        int loops
    )
    {
        Mix_PlayMusic(music, loops)
            .AssertSdlSuccess();
    }

    public static unsafe SDL_AudioSpec? QuerySpec()
    {
        int frequency, channels;
        SDL_AudioFormat format;

        if (!Mix_QuerySpec(&frequency, &format, &channels))
            return new SDL_AudioSpec
            {
                freq = frequency,
                channels = channels,
                format = format
            };

        return null;
    }

    // todo: registereffect

    public static void Rewind()
    {
        Mix_RewindMusic();
    }

    public static bool SetPosition(double seconds)
    {
        return Mix_SetMusicPosition(seconds);
    }

    public static bool HasDecoder(ReadOnlySpan<char> name)
    {
        using var nameStr = new Utf8Span(name);
        return Mix_HasMusicDecoder(nameStr);
    }
}