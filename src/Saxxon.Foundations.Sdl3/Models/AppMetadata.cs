using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class AppMetadata
{
    public static unsafe void Set(
        ReadOnlySpan<char> appName,
        ReadOnlySpan<char> appVersion,
        ReadOnlySpan<char> appIdentifier
    )
    {
        var appNameLen = appName.MeasureUtf8();
        var appVersionLen = appVersion.MeasureUtf8();
        var appIdentifierLen = appIdentifier.MeasureUtf8();
        
        Span<byte> appNameBytes = stackalloc byte[appNameLen];
        Span<byte> appVersionBytes = stackalloc byte[appVersionLen];
        Span<byte> appIdentifierBytes = stackalloc byte[appIdentifierLen];

        appName.EncodeUtf8(appNameBytes);
        appVersion.EncodeUtf8(appVersionBytes);
        appIdentifier.EncodeUtf8(appIdentifierBytes);

        fixed (byte* appNamePtr = appNameBytes)
        fixed (byte* appVersionPtr = appVersionBytes)
        fixed (byte* appIdentifierPtr = appIdentifierBytes)
            SDL_SetAppMetadata(appNamePtr, appVersionPtr, appIdentifierPtr)
                .AssertSdlSuccess();
    }

    public static unsafe void SetProperty(
        ReadOnlySpan<char> key,
        ReadOnlySpan<char> value
    )
    {
        var keyLen = key.MeasureUtf8();
        var valueLen = value.MeasureUtf8();
        
        Span<byte> keyBytes = stackalloc byte[keyLen];
        Span<byte> valueBytes = stackalloc byte[valueLen];
        
        key.EncodeUtf8(keyBytes);
        value.EncodeUtf8(valueBytes);

        fixed (byte* keyPtr = keyBytes)
        fixed (byte* valuePtr = valueBytes)
            SDL_SetAppMetadataProperty(keyPtr, valuePtr)
                .AssertSdlSuccess();
    }

    public static unsafe string? GetProperty(
        ReadOnlySpan<char> key
    )
    {
        var keyLen = key.MeasureUtf8();
        
        Span<byte> keyBytes = stackalloc byte[keyLen];
        
        key.EncodeUtf8(keyBytes);

        fixed (byte* keyPtr = keyBytes)
            return Marshal.PtrToStringUTF8(
                (IntPtr)Unsafe_SDL_GetAppMetadataProperty(keyPtr)
        );
    }
}