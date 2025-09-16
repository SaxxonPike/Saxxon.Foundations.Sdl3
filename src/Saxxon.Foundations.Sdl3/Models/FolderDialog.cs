using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for open folder dialogs.
/// </summary>
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