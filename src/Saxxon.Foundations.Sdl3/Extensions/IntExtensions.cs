using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Extensions;

[PublicAPI]
internal static class IntExtensions
{
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public static int AssertSdlNotEqual(this int value, int check)
    {
        if (value == check)
            throw new SdlException();
        return value;
    }
}