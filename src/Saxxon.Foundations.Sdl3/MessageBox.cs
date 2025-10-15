using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for displaying message boxes.
/// </summary>
[PublicAPI]
public static class MessageBox
{
    public static unsafe int Show(
        SDL_MessageBoxData data
    )
    {
        int buttonId;
        SDL_ShowMessageBox(&data, &buttonId)
            .AssertSdlSuccess();
        return buttonId;
    }

    public static unsafe void Show(
        SDL_MessageBoxFlags flags,
        ReadOnlySpan<char> title,
        ReadOnlySpan<char> message,
        IntPtr<SDL_Window> parent = default
    )
    {
        using var titleStr = new UnmanagedString(title);
        using var messageStr = new UnmanagedString(message);

        SDL_ShowSimpleMessageBox(
            flags,
            titleStr,
            messageStr,
            parent
        ).AssertSdlSuccess();
    }
}