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
    /// <summary>
    /// Retrieves a message about the last error that occurred on the current thread.
    /// </summary>
    public static unsafe string Get()
    {
        return ((IntPtr<byte>)Unsafe_SDL_GetError())
            .GetString()!;
    }

    /// <summary>
    /// Sets the SDL error message for the current thread.
    /// </summary>
    public static unsafe void Set(string message)
    {
        using var messageStr = new UnmanagedString(message);
        fixed (byte* abc = "%s"u8)
            SDL_SetErrorV(abc, messageStr.Ptr);
    }

    /// <summary>
    /// Clears any previous error message for this thread.
    /// </summary>
    public static void Clear()
    {
        SDL_ClearError();
    }
}