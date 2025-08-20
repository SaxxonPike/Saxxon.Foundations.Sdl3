using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Extensions;

[PublicAPI]
internal static class LongExtensions
{
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public static long AssertSdlNotEqual(this long value, long check)
    {
        if (value == check)
            throw new SdlException();
        return value;
    }
}