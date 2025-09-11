using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an interface for getting or setting the SDL error.
/// </summary>
[PublicAPI]
public static class SdlError
{
    public static unsafe string Get()
    {
        return ((IntPtr<byte>)Unsafe_SDL_GetError())
            .GetString()!;
    }

    public static unsafe void Set(string message)
    {
        using var messageStr = new Utf8Span(message);
        fixed (byte* abc = "%s"u8)
            SDL_SetErrorV(abc, messageStr.Ptr);
    }
}