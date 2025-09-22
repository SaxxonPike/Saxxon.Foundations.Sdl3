using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Extensions;

/// <summary>
/// Extensions for floats.
/// </summary>
[PublicAPI]
internal static class FloatExtensions
{
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public static float AssertSdlNotEqual(this float value, float check)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return value == check ? throw new SdlException() : value;
    }
}