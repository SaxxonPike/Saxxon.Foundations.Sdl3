using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class KeyCode
{
    public static SDL_Keycode GetFromName(
        ReadOnlySpan<char> name
    )
    {
        using var nameStr = new Utf8Span(name);
        var result = SDL_GetKeyFromName(nameStr);
        if (result == SDL_Keycode.SDLK_UNKNOWN)
            throw new SdlException();
        return result;
    }

    public static unsafe string GetName(
        this SDL_Keycode key
    )
    {
        return Marshal.PtrToStringUTF8((IntPtr)Unsafe_SDL_GetKeyName(key))!;
    }

    public static unsafe (SDL_Scancode Scan, SDL_Keymod Mod) GetScan(
        this SDL_Keycode keycode
    )
    {
        SDL_Keymod mod;
        var scan = SDL_GetScancodeFromKey(keycode, &mod);
        return (scan, mod);
    }
}