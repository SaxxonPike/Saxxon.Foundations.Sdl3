using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class AppMetadata
{
    public static void Set(
        ReadOnlySpan<char> appName,
        ReadOnlySpan<char> appVersion,
        ReadOnlySpan<char> appIdentifier
    )
    {
        using var appNameStr = new Utf8Span(appName);
        using var appVersionStr = new Utf8Span(appVersion);
        using var appIdentifierStr = new Utf8Span(appIdentifier);

        SDL_SetAppMetadata(appNameStr, appVersionStr, appIdentifierStr)
            .AssertSdlSuccess();
    }

    public static void SetProperty(
        ReadOnlySpan<char> key,
        ReadOnlySpan<char> value
    )
    {
        using var keyStr = new Utf8Span(key);
        using var valueStr = new Utf8Span(value);

        SDL_SetAppMetadataProperty(keyStr, valueStr)
            .AssertSdlSuccess();
    }

    public static unsafe string? GetProperty(
        ReadOnlySpan<char> key
    )
    {
        using var keyStr = new Utf8Span(key);

        return Marshal.PtrToStringUTF8(
            (IntPtr)Unsafe_SDL_GetAppMetadataProperty(keyStr.Ptr)
        );
    }
}