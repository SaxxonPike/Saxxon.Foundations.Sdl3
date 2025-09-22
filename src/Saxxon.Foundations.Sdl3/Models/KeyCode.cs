using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Keycode"/>.
/// </summary>
[PublicAPI]
public static class KeyCode
{
    public static SDL_Keycode GetFromName(
        ReadOnlySpan<char> name
    )
    {
        using var nameStr = new UnmanagedString(name);
        var result = SDL_GetKeyFromName(nameStr);
        return result == SDL_Keycode.SDLK_UNKNOWN ? throw new SdlException() : result;
    }

    public static unsafe string GetName(
        this SDL_Keycode key
    )
    {
        return Ptr.ToUtf8String(Unsafe_SDL_GetKeyName(key))!;
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