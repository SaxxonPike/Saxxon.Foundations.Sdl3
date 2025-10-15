using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_MouseID"/> as well
/// as global mouse state.
/// </summary>
[PublicAPI]
public static class Mouse
{
    public static unsafe (float X, float Y, SDL_MouseButtonFlags Flags) GetState()
    {
        float x, y;
        var flags = SDL_GetMouseState(&x, &y);
        return (x, y, flags);
    }
    
    public static unsafe (float X, float Y, SDL_MouseButtonFlags Flags) GetGlobalState()
    {
        float x, y;
        var flags = SDL_GetGlobalMouseState(&x, &y);
        return (x, y, flags);
    }

    public static unsafe IntPtr<SDL_Window> GetFocus()
    {
        return SDL_GetMouseFocus();
    }

    public static unsafe string? GetName(
        this SDL_MouseID instanceId
    )
    {
        return ((IntPtr<byte>)Unsafe_SDL_GetMouseNameForID(instanceId)).GetString();
    }

    public static unsafe IMemoryOwner<SDL_MouseID> GetAll()
    {
        int count;
        var mice = SDL_GetMice(&count);
        return SdlMemoryManager.Owned(mice, count);
    }

    public static unsafe IntPtr CreateMetalView(
        this IntPtr<SDL_Window> window
    )
    {
        return SDL_Metal_CreateView(window);
    }
}