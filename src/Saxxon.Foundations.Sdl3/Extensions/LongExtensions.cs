using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Extensions;

/// <summary>
/// Extensions for long integers.
/// </summary>
[PublicAPI]
internal static class LongExtensions
{
    /// <summary>
    /// Throws an exception if the values are not equal after an SDL operation.
    /// </summary>
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public static long AssertSdlNotEqual(this long value, long check)
    {
        if (value == check)
            throw new SdlException();
        return value;
    }
}