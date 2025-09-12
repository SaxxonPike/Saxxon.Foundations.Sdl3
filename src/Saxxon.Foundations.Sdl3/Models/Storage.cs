using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Storage"/>.
/// </summary>
[PublicAPI]
public static class Storage
{
    public static unsafe List<string> GlobDirectory(
        this IntPtr<SDL_Storage> storage,
        ReadOnlySpan<char> path,
        ReadOnlySpan<char> pattern,
        SDL_GlobFlags flags
    )
    {
        using var pathStr = new Utf8Span(path);
        using var patternStr = new Utf8Span(pattern);

        int count;
        var names = SDL_GlobStorageDirectory(storage, pathStr, patternStr, flags, &count);

        var result = new List<string>();

        if (names == null)
            return result;

        for (var i = 0; i < count; i++)
        {
            if (((IntPtr)names[i]).GetString() is not { } name)
                continue;

            result.Add(name);
        }

        return result;
    }

    public static unsafe ulong GetSpaceRemaining(
        this IntPtr<SDL_Storage> storage
    )
    {
        return SDL_GetStorageSpaceRemaining(storage);
    }

    public static unsafe SDL_PathInfo GetPathInfo(
        this IntPtr<SDL_Storage> storage,
        ReadOnlySpan<char> path
    )
    {
        SDL_PathInfo info;
        using var pathStr = new Utf8Span(path);
        SDL_GetStoragePathInfo(storage, pathStr, &info)
            .AssertSdlSuccess();
        return info;
    }

    public static unsafe void CopyFile(
        this IntPtr<SDL_Storage> storage,
        ReadOnlySpan<char> src,
        ReadOnlySpan<char> dst
    )
    {
        using var srcStr = new Utf8Span(src);
        using var dstStr = new Utf8Span(dst);

        SDL_CopyStorageFile(storage, srcStr, dstStr)
            .AssertSdlSuccess();
    }

    public static unsafe void RenamePath(
        this IntPtr<SDL_Storage> storage,
        ReadOnlySpan<char> src,
        ReadOnlySpan<char> dst
    )
    {
        using var srcStr = new Utf8Span(src);
        using var dstStr = new Utf8Span(dst);

        SDL_RenameStoragePath(storage, srcStr, dstStr)
            .AssertSdlSuccess();
    }

    public static unsafe void RemovePath(
        this IntPtr<SDL_Storage> storage,
        ReadOnlySpan<char> path
    )
    {
        using var pathStr = new Utf8Span(path);

        SDL_RemoveStoragePath(storage, pathStr)
            .AssertSdlSuccess();
    }

    public delegate SDL_EnumerationResult EnumerateDelegate(string directory, string file);

    public static unsafe void EnumerateDirectory(
        this IntPtr<SDL_Storage> storage,
        ReadOnlySpan<char> path,
        EnumerateDelegate func
    )
    {
        using var pathStr = new Utf8Span(path);
        var userData = UserDataStore.Add(func);

        SDL_EnumerateStorageDirectory(
            storage,
            pathStr,
            &Execute,
            userData
        ).AssertSdlSuccess();

        UserDataStore.Remove(userData);
        return;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static SDL_EnumerationResult Execute(IntPtr userData, byte* dirName, byte* fileName)
        {
            var dir = Ptr.ToUtf8String(dirName);
            var file = Ptr.ToUtf8String(fileName);
            return UserDataStore.Get<EnumerateDelegate>(userData)!.Invoke(dir!, file!);
        }
    }

    public static unsafe void CreateDirectory(
        this IntPtr<SDL_Storage> storage,
        ReadOnlySpan<char> path
    )
    {
        using var pathStr = new Utf8Span(path);
        SDL_CreateStorageDirectory(storage, pathStr)
            .AssertSdlSuccess();
    }

    public static unsafe void WriteFile(
        this IntPtr<SDL_Storage> storage,
        ReadOnlySpan<char> path,
        ReadOnlySpan<byte> data
    )
    {
        using var pathStr = new Utf8Span(path);
        fixed (byte* dataPtr = data)
        {
            SDL_WriteStorageFile(storage, pathStr, (IntPtr)dataPtr, (ulong)data.Length)
                .AssertSdlSuccess();
        }
    }

    public static unsafe void ReadFile(
        this IntPtr<SDL_Storage> storage,
        ReadOnlySpan<char> path,
        Span<byte> data
    )
    {
        using var pathStr = new Utf8Span(path);
        fixed (byte* dataPtr = data)
        {
            SDL_ReadStorageFile(storage, pathStr, (IntPtr)dataPtr, (ulong)data.Length)
                .AssertSdlSuccess();
        }
    }

    public static unsafe ulong GetFileSize(
        this IntPtr<SDL_Storage> storage,
        ReadOnlySpan<char> path
    )
    {
        using var pathStr = new Utf8Span(path);
        ulong length;
        SDL_GetStorageFileSize(storage, pathStr, &length)
            .AssertSdlSuccess();
        return length;
    }

    public static unsafe bool IsReady(
        this IntPtr<SDL_Storage> storage
    )
    {
        return SDL_StorageReady(storage);
    }

    public static unsafe void Close(
        this IntPtr<SDL_Storage> storage
    )
    {
        SDL_CloseStorage(storage);
    }

    public static SDL_StorageInterface InitInterface()
    {
        SDL_INIT_INTERFACE(out SDL_StorageInterface @interface);
        return @interface;
    }

    public static unsafe IntPtr<SDL_Storage> Open(SDL_StorageInterface @interface, IntPtr userData)
    {
        return SDL_OpenStorage(&@interface, userData);
    }

    public static unsafe IntPtr<SDL_Storage> OpenUser(
        ReadOnlySpan<char> org,
        ReadOnlySpan<char> app,
        SDL_PropertiesID props = 0
    )
    {
        using var orgStr = new Utf8Span(org);
        using var appStr = new Utf8Span(app);

        return SDL_OpenUserStorage(orgStr, appStr, props);
    }

    public static unsafe IntPtr<SDL_Storage> OpenTitle(
        ReadOnlySpan<char> @override = default,
        SDL_PropertiesID props = 0
    )
    {
        using var overrideStr = new Utf8Span(@override);

        return SDL_OpenTitleStorage(overrideStr, props);
    }
    
    
}