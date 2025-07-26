using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_IOStream"/>.
/// </summary>
[PublicAPI]
public static class IoStream
{
    public static SDL_IOStreamInterface InitInterface()
    {
        SDL_INIT_INTERFACE(out SDL_IOStreamInterface @interface);
        return @interface;
    }
    
    public static unsafe IntPtr<SDL_IOStream> Open(
        SDL_IOStreamInterface @interface,
        IntPtr userData)
    {
        return ((IntPtr)SDL_OpenIO(&@interface, userData))
            .AssertSdlNotNull();
    }
}