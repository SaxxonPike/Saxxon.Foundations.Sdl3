using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an interface for uncategorized SDL_mixer functions.
/// </summary>
[PublicAPI]
public static class MixerLib
{
    public static int GetVersion()
    {
        return MIX_Version();
    }

    public static void Init()
    {
        MIX_Init()
            .AssertSdlSuccess();
    }
}