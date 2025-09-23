using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an interface for manipulating the clipboard.
/// </summary>
[PublicAPI]
public static class Clipboard
{
    public static void ClearData()
    {
        SDL_ClearClipboardData()
            .AssertSdlSuccess();
    }

    public static unsafe IMemoryOwner<byte> GetData(ReadOnlySpan<char> mimeType)
    {
        using var mimeTypeStr = new UnmanagedString(mimeType);
        UIntPtr size;
        var data = ((IntPtr<byte>)SDL_GetClipboardData(mimeTypeStr.Ptr, &size))
            .AssertSdlNotNull();
        return SdlMemoryManager.Owned(data, (int)size);
    }

    public static unsafe List<string?> GetMimeTypes()
    {
        UIntPtr count;
        var mimeTypes = ((IntPtr<IntPtr<byte>>)SDL_GetClipboardMimeTypes(&count))
            .AssertSdlNotNull();

        var result = mimeTypes.GetStrings((int)count);
        SDL_free(mimeTypes.Ptr);
        return result;
    }

    public static string? GetPrimarySelectionText() =>
        SDL_GetPrimarySelectionText();

    public static string? GetText() =>
        SDL_GetClipboardText();

    public static bool HasData(ReadOnlySpan<char> mimeType)
    {
        var mimeTypeStr = new UnmanagedString(mimeType);
        return SDL_HasClipboardData(mimeTypeStr);
    }

    public static bool HasPrimarySelectionText() =>
        SDL_HasPrimarySelectionText();

    public static bool HasText() =>
        SDL_HasClipboardText();

    private class SetClipboardDataCallbackData
    {
        public required IReadOnlyDictionary<string, IntPtr> Data { get; init; }
        public required Action Cleanup { get; init; }
    }

    public static unsafe void SetClipboardData(
        IReadOnlyDictionary<string, IntPtr> data,
        Action cleanup
    )
    {
        var userData = new SetClipboardDataCallbackData
        {
            Data = data,
            Cleanup = cleanup
        };

        var userDataValue = UserDataStore.Add(userData);
        var mimeTypes = data.Keys.ToArray();
        using var mimeTypeArray = new Utf8ByteStrings(mimeTypes);

        SDL_SetClipboardData(
            &SetClipboardDataGetCallback,
            &SetClipboardDataCleanupCallback,
            userDataValue,
            (byte**)mimeTypeArray.Ptr,
            (UIntPtr)mimeTypes.Length
        ).AssertSdlSuccess();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void SetClipboardDataCleanupCallback(IntPtr userData)
    {
        var data = UserDataStore.Get<SetClipboardDataCallbackData>(userData);
        data?.Cleanup();
        UserDataStore.Remove(userData);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe IntPtr SetClipboardDataGetCallback(
        IntPtr userData,
        byte* mimeType,
        UIntPtr* size
    )
    {
        var data = UserDataStore.Get<SetClipboardDataCallbackData>(userData);
        var result = IntPtr.Zero;
        data?.Data.TryGetValue(Ptr.ToUtf8String(mimeType)!, out result);
        return result;
    }

    public static void SetClipboardText(ReadOnlySpan<char> text)
    {
        using var textStr = new UnmanagedString(text);
        SDL_SetClipboardText(textStr)
            .AssertSdlSuccess();
    }

    public static void SetPrimarySelectionText(ReadOnlySpan<char> text)
    {
        using var textStr = new UnmanagedString(text);
        SDL_SetPrimarySelectionText(textStr)
            .AssertSdlSuccess();
    }
}