using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class Locale
{
    [MustDisposeResource]
    public static unsafe IMemoryOwner<IntPtr<SDL_Locale>> GetPreferred()
    {
        int count;
        var locales = SDL_GetPreferredLocales(&count);
        return SdlMemoryManager.Owned(locales, count);
    }
}