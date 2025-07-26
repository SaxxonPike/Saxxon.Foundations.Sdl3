using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Represents SDL_mixer init flags.
///
/// This is not an actual SDL_mixer type. It wraps "int" for the purpose of
/// providing the fluent interface.
/// </summary>
// ReSharper disable InconsistentNaming
[PublicAPI]
[Flags]
public enum Mix_InitFlags : uint
{
    MIX_INIT_FLAC = SDL3_mixer.MIX_INIT_FLAC,
    MIX_INIT_MOD = SDL3_mixer.MIX_INIT_MOD,
    MIX_INIT_MP3 = SDL3_mixer.MIX_INIT_MP3,
    MIX_INIT_OGG = SDL3_mixer.MIX_INIT_OGG,
    MIX_INIT_MID = SDL3_mixer.MIX_INIT_MID,
    MIX_INIT_OPUS = SDL3_mixer.MIX_INIT_OPUS,
    MIX_INIT_WAVPACK = SDL3_mixer.MIX_INIT_WAVPACK
}