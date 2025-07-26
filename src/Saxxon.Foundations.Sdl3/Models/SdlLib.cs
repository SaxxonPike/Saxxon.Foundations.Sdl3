using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an interface for uncategorized SDL functions.
/// </summary>
[PublicAPI]
public static class SdlLib
{
    public static void Init(SDL_InitFlags flags)
    {
        SDL_Init(flags).AssertSdlSuccess();
    }

    public static SDL_InitFlags WasInit(SDL_InitFlags flags)
    {
        return SDL_WasInit(flags);
    }

    public static void Quit()
    {
        SDL_Quit();
    }

    public static unsafe string? GetRevision()
    {
        return Marshal.PtrToStringUTF8((IntPtr)Unsafe_SDL_GetRevision());
    }

    public static int GetVersion()
    {
        return SDL_GetVersion();
    }

    public static unsafe string? GetPlatform()
    {
        return Marshal.PtrToStringUTF8((IntPtr)Unsafe_SDL_GetPlatform());
    }

    public static void OpenUrl(
        ReadOnlySpan<char> url
    )
    {
        using var urlStr = new Utf8Span(url);
        SDL_OpenURL(urlStr)
            .AssertSdlSuccess();
    }
}