using System.Runtime.InteropServices.Marshalling;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for SDL runtime hints.
/// </summary>
[PublicAPI]
public static class Hints
{
    public static void Set(string name, string value)
    {
        using var nameStr = new Utf8Span(name);
        using var valueStr = new Utf8Span(value);
        SDL_SetHint(nameStr, valueStr)
            .AssertSdlSuccess();
    }

    public static void Set(ReadOnlySpan<byte> name, string value)
    {
        using var nameStr = new Utf8Span(name);
        using var valueStr = new Utf8Span(value);
        SDL_SetHint(nameStr, valueStr)
            .AssertSdlSuccess();
    }

    public static unsafe string? Get(string name)
    {
        using var nameStr = new Utf8Span(name);
        return Utf8StringMarshaller.ConvertToManaged(Unsafe_SDL_GetHint(nameStr.Ptr));
    }
}