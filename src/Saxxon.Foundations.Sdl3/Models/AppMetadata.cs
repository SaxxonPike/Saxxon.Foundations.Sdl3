using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for application metadata.
/// </summary>
[PublicAPI]
public static class AppMetadata
{
    /// <summary>
    /// Specifies basic metadata about your app.
    /// </summary>
    /// <param name="appName">
    /// The name of the application ("My Game 2: Bad Guy's Revenge!").
    /// </param>
    /// <param name="appVersion">
    /// The version of the application ("1.0.0beta5" or a git hash, or
    /// whatever makes sense).
    /// </param>
    /// <param name="appIdentifier">
    /// A unique string in reverse-domain format that identifies this app
    /// ("com.example.mygame2").
    /// </param>
    /// <remarks>
    /// You can optionally provide metadata about your app to SDL. This is not
    /// required, but strongly encouraged.
    /// 
    /// There are several locations where SDL can make use of metadata
    /// (an "About" box in the macOS menu bar, the name of the app can be shown
    /// on some audio mixers, etc). Any piece of metadata can be left as NULL,
    /// if a specific detail doesn't make sense for the app.
    /// 
    /// This function should be called as early as possible, before SDL_Init.
    /// 
    /// Multiple calls to this function are allowed, but various state might
    /// not change once it has been set up with a previous call to this
    /// function.
    /// 
    /// Passing null removes any previous metadata.
    /// 
    /// This is a simplified interface for the most important information. You
    /// can supply significantly more detailed metadata with
    /// <see cref="SetProperty"/>.
    /// </remarks>
    public static void Set(
        ReadOnlySpan<char> appName,
        ReadOnlySpan<char> appVersion,
        ReadOnlySpan<char> appIdentifier
    )
    {
        using var appNameStr = new UnmanagedString(appName);
        using var appVersionStr = new UnmanagedString(appVersion);
        using var appIdentifierStr = new UnmanagedString(appIdentifier);

        SDL_SetAppMetadata(appNameStr, appVersionStr, appIdentifierStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Specifies metadata about your app through a set of properties.
    /// </summary>
    /// <param name="key">
    /// The name of the metadata property to set.
    /// </param>
    /// <param name="value">
    /// The value of the property, or null to remove that property.
    /// </param>
    public static void SetProperty(
        ReadOnlySpan<char> key,
        ReadOnlySpan<char> value
    )
    {
        using var keyStr = new UnmanagedString(key);
        using var valueStr = new UnmanagedString(value);

        SDL_SetAppMetadataProperty(keyStr, valueStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Gets metadata about your app.
    /// </summary>
    /// <param name="key">
    /// The name of the metadata property to get.
    /// </param>
    /// <returns>
    /// The current value of the metadata property, or the default if it is not
    /// set, or null for properties with no default.
    /// </returns>
    public static unsafe string? GetProperty(
        ReadOnlySpan<char> key
    )
    {
        using var keyStr = new UnmanagedString(key);

        return ((IntPtr<byte>)Unsafe_SDL_GetAppMetadataProperty(keyStr.Ptr))
            .GetString();
    }
}