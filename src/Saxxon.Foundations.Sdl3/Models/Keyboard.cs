using System.Buffers;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class Keyboard
{
    public static unsafe ReadOnlySpan<SDLBool> GetState()
    {
        int count;
        var state = SDL_GetKeyboardState(&count);
        return new ReadOnlySpan<SDLBool>(state, count);
    }

    public static unsafe IntPtr<SDL_Window> GetFocus()
    {
        return SDL_GetKeyboardFocus();
    }

    public static unsafe string GetName(
        this SDL_KeyboardID instanceId
    )
    {
        return Marshal.PtrToStringUTF8((IntPtr)Unsafe_SDL_GetKeyboardNameForID(instanceId))
               ?? throw new SdlException();
    }

    [MustDisposeResource]
    public static unsafe IMemoryOwner<SDL_KeyboardID> GetAll()
    {
        int count;
        var keyboards = SDL_GetKeyboards(&count);
        return SdlMemoryManager.Owned(keyboards, count);
    }
}