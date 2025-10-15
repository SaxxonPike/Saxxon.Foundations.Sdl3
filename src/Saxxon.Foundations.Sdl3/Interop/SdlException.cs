using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Represents an exception that contains an SDL-specific error message.
/// </summary>
[PublicAPI]
public sealed class SdlException : Exception
{
    internal SdlException() : base(SdlError.Get())
    {
    }

    internal SdlException(string message) : base(message)
    {
    }

    internal static void ThrowIfError()
    {
        var error = SdlError.Get();
        if (error != null)
            throw new SdlException(error);
    }
}