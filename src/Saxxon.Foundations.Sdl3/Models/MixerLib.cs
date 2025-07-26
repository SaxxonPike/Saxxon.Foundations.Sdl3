using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an interface for uncategorized SDL_mixer functions.
/// </summary>
[PublicAPI]
public static class MixerLib
{
    public static unsafe string? GetSoundFonts()
    {
        return Marshal.PtrToStringUTF8((IntPtr)Mix_GetSoundFonts());
    }

    public static unsafe string? GetTimidityConfig()
    {
        return Marshal.PtrToStringUTF8((IntPtr)Mix_GetTimidityCfg());
    }

    public static Mix_InitFlags GetInitFlags()
    {
        return (Mix_InitFlags)Mix_Init(0);
    }

    public static Mix_InitFlags Init(Mix_InitFlags flags)
    {
        return (Mix_InitFlags)Mix_Init((uint)flags);
    }
}