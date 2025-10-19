using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3;

public static partial class Sdl
{
    public static int GetMixVersion()
    {
        return MIX_Version();
    }

    public static void InitMix()
    {
        MIX_Init()
            .AssertSdlSuccess();
    }

    public static void QuitMix()
    {
        MIX_Quit();
    }
}