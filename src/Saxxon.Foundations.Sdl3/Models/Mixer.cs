using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an interface for manipulating the audio mixer.
/// </summary>
[PublicAPI]
public static class Mixer
{
    public static unsafe void Open(
        SDL_AudioDeviceID id,
        SDL_AudioSpec? format
    )
    {
        Mix_OpenAudio(id, format is { } formatVal ? &formatVal : null)
            .AssertSdlSuccess();
    }

    public static void Close()
    {
        Mix_CloseAudio();
    }

    // TODO: hookmusic

    public static float GetVolume()
    {
        return Mix_MasterVolume(-1) / (float)MIX_MAX_VOLUME;
    }

    public static void SetVolume(float volume)
    {
        _ = Mix_MasterVolume((int)(Math.Clamp(volume, 0, 1) * MIX_MAX_VOLUME));
    }

    public static void Pause()
    {
        Mix_PauseAudio(1);
    }

    public static void Resume()
    {
        Mix_PauseAudio(0);
    }
}