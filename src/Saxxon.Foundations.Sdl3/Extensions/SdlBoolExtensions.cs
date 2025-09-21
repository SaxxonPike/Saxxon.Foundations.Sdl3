using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Extensions;

/// <summary>
/// Extensions for <see cref="SDLBool"/>.
/// </summary>
[PublicAPI]
public static class SdlBoolExtensions
{
    /// <summary>
    /// Throws an exception if the value is false after an SDL operation.
    /// </summary>
    internal static void AssertSdlSuccess([DoesNotReturnIf(false)] this SDLBool value)
    {
        if (value == false)
            throw new SdlException();
    }
}