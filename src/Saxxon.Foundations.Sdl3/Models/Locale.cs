using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Locale"/>.
/// </summary>
[PublicAPI]
public static class Locale
{
    /// <summary>
    /// Gets the country string for a locale.
    /// </summary>
    /// <param name="locale">
    /// Locale to query.
    /// </param>
    public static unsafe string? GetCountry(this IntPtr<SDL_Locale> locale) =>
        ((IntPtr<byte>)locale.AsReadOnlyRef().country).GetString();

    /// <summary>
    /// Gets the language string for a locale.
    /// </summary>
    /// <param name="locale">
    /// Locale to query.
    /// </param>
    public static unsafe string? GetLanguage(this IntPtr<SDL_Locale> locale) =>
        ((IntPtr<byte>)locale.AsReadOnlyRef().language).GetString();

    /// <summary>
    /// Reports the user's preferred locale.
    /// </summary>
    /// <returns>
    /// Locale data.
    /// </returns>
    [MustDisposeResource]
    public static unsafe IMemoryOwner<IntPtr<SDL_Locale>> GetPreferred()
    {
        int count;
        var locales = SDL_GetPreferredLocales(&count);
        return SdlMemoryManager.Owned(locales, count);
    }
}