using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Extensions;

[PublicAPI]
public static class SdlBoolExtensions
{
    public static void AssertSdlSuccess([DoesNotReturnIf(false)] this SDLBool value)
    {
        if (value == false)
            throw new SdlException();
    }
}