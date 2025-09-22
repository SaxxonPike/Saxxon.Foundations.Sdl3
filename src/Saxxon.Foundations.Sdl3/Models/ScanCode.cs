using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Scancode"/>.
/// </summary>
[PublicAPI]
public static class ScanCode
{
    public static SDL_Scancode GetFromName(
        ReadOnlySpan<char> name
    )
    {
        using var nameStr = new UnmanagedString(name);
        var result = SDL_GetScancodeFromName(nameStr);
        if (result == SDL_Scancode.SDL_SCANCODE_UNKNOWN)
            throw new SdlException();
        return result;
    }

    public static unsafe string GetName(
        this SDL_Scancode scan
    )
    {
        return Ptr.ToUtf8String(Unsafe_SDL_GetScancodeName(scan))!;
    }

    public static void SetName(
        this SDL_Scancode scan,
        ReadOnlySpan<char> name
    )
    {
        using var nameStr = new UnmanagedString(name);
        SDL_SetScancodeName(scan, nameStr)
            .AssertSdlSuccess();
    }

    public static SDL_Keycode GetKey(
        this SDL_Scancode scan,
        SDL_Keymod mod,
        bool isForEvents
    )
    {
        return SDL_GetKeyFromScancode(scan, mod, isForEvents);
    }
}