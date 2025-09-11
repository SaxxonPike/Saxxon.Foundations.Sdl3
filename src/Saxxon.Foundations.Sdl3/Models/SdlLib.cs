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
    /// <summary>
    /// Initializes the SDL library.
    /// </summary>
    public static void Init(SDL_InitFlags flags)
    {
        SDL_Init(flags).AssertSdlSuccess();
    }

    /// <summary>
    /// Gets a mask of the specified subsystems which are currently initialized.
    /// </summary>
    public static SDL_InitFlags WasInit(SDL_InitFlags flags)
    {
        return SDL_WasInit(flags);
    }

    /// <summary>
    /// Cleans up all initialized subsystems.
    /// </summary>
    public static void Quit()
    {
        SDL_Quit();
    }

    /// <summary>
    /// Gets the code revision of the SDL library that is linked against your program.
    /// </summary>
    public static unsafe string? GetRevision()
    {
        return ((IntPtr)Unsafe_SDL_GetRevision()).GetString();
    }

    /// <summary>
    /// Gets the version of SDL that is linked against your program.
    /// </summary>
    public static int GetVersion()
    {
        return SDL_GetVersion();
    }

    /// <summary>
    /// Gets the name of the platform.
    /// </summary>
    public static unsafe string? GetPlatform()
    {
        return ((IntPtr)Unsafe_SDL_GetPlatform()).GetString();
    }

    /// <summary>
    /// Opens a URL/URI in the browser or other appropriate external application.
    /// </summary>
    public static void OpenUrl(
        ReadOnlySpan<char> url
    )
    {
        using var urlStr = new Utf8Span(url);
        SDL_OpenURL(urlStr)
            .AssertSdlSuccess();
    }
}