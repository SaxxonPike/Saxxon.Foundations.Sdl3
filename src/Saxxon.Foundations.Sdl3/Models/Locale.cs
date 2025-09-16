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
    public static unsafe string? GetCountry(this IntPtr<SDL_Locale> locale) =>
        ((IntPtr)locale.AsReadOnlyRef().country).GetString();

    public static unsafe string? GetLanguage(this IntPtr<SDL_Locale> locale) =>
        ((IntPtr)locale.AsReadOnlyRef().language).GetString();

    [MustDisposeResource]
    public static unsafe IMemoryOwner<IntPtr<SDL_Locale>> GetPreferred()
    {
        int count;
        var locales = SDL_GetPreferredLocales(&count);
        return SdlMemoryManager.Owned(locales, count);
    }
}