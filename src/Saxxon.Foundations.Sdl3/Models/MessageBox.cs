using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

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
        using var titleStr = new Utf8Span(title);
        using var messageStr = new Utf8Span(message);

        SDL_ShowSimpleMessageBox(
            flags,
            titleStr,
            messageStr,
            parent
        ).AssertSdlSuccess();
    }
}