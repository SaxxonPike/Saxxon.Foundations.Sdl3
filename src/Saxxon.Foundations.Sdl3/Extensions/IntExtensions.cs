using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Extensions;

/// <summary>
/// Extensions for integers.
/// </summary>
[PublicAPI]
internal static class IntExtensions
{
    /// <summary>
    /// Throws an exception if the values are not equal after an SDL operation.
    /// </summary>
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public static int AssertSdlNotEqual(this int value, int check)
    {
        return value == check ? throw new SdlException() : value;
    }
    
    /// <summary>
    /// Throws an exception if the value is not zero after an SDL operation.
    /// </summary>
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public static int AssertSdlZero(this int value)
    {
        return value == 0 ? throw new SdlException() : value;
    }
}