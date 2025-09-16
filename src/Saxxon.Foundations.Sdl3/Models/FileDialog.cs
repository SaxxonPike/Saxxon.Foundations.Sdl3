using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Helpers;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for open and save file dialogs.
/// </summary>
[PublicAPI]
public static class FileDialog
{
    public delegate void FileResultDelegate(List<string> files, int filter);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void HandleFile(IntPtr userData, byte** fileNames, int filterIndex)
    {
        var callback = UserDataStore.Get<FileResultDelegate>(userData);
        var resultFiles = new Utf8SpanArray((IntPtr<IntPtr>)fileNames).ToList();
        UserDataStore.Remove(userData);
        callback?.Invoke(resultFiles, filterIndex);
    }

    public static unsafe void ShowOpen(
        FileResultDelegate callback,
        IntPtr<SDL_Window> window,
        List<(string Name, string Pattern)>? filters,
        string? defaultLocation,
        bool allowMany
    )
    {
        using var defaultLocationStr = new UnmanagedString(defaultLocation);
        using var filterData = new DialogFileFilterList(filters ?? []);
        var userData = UserDataStore.Add(callback);

        SDL_ShowOpenFileDialog(
            &HandleFile,
            userData,
            window,
            filterData.Ptr,
            filterData.Count,
            defaultLocationStr,
            allowMany
        );
    }

    public static unsafe void ShowSave(
        FileResultDelegate callback,
        IntPtr<SDL_Window> window,
        List<(string Name, string Pattern)>? filters,
        string? defaultLocation
    )
    {
        using var defaultLocationStr = new UnmanagedString(defaultLocation);
        using var filterData = new DialogFileFilterList(filters ?? []);
        var userData = UserDataStore.Add(callback);

        SDL_ShowSaveFileDialog(
            &HandleFile,
            userData,
            window,
            filterData.Ptr,
            filterData.Count,
            defaultLocationStr
        );
    }

    public static unsafe void ShowWithProperties(
        SDL_FileDialogType type,
        FileResultDelegate callback,
        SDL_PropertiesID props
    )
    {
        var userData = UserDataStore.Add(callback);

        SDL_ShowFileDialogWithProperties(
            type,
            &HandleFile,
            userData,
            props
        );
    }
}

[PublicAPI]
public static class FolderDialog
{
    public delegate void FolderResultDelegate(List<string> files, int filter);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void HandleFolder(IntPtr userData, byte** fileNames, int filterIndex)
    {
        var callback = UserDataStore.Get<FolderResultDelegate>(userData);
        UserDataStore.Remove(userData);
        var resultFiles = new Utf8SpanArray((IntPtr<IntPtr>)fileNames).ToList();
        callback?.Invoke(resultFiles, filterIndex);
    }

    public static unsafe void Show(
        FolderResultDelegate callback,
        IntPtr<SDL_Window> window,
        string? defaultLocation,
        bool allowMany
    )
    {
        using var defaultLocationStr = new UnmanagedString(defaultLocation);
        var userData = UserDataStore.Add(callback);

        SDL_ShowOpenFolderDialog(
            &HandleFolder,
            userData,
            window,
            defaultLocationStr,
            allowMany
        );
    }
}