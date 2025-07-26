using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class Cursor
{
    public static unsafe IntPtr<SDL_Cursor> GetDefault()
    {
        return SDL_GetDefaultCursor();
    }

    public static unsafe IntPtr<SDL_Cursor> Get()
    {
        return SDL_GetCursor();
    }

    public static unsafe void Set(
        IntPtr<SDL_Cursor> cursor
    )
    {
        SDL_SetCursor(cursor)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<SDL_Cursor> CreateSystem(
        SDL_SystemCursor cursor
    )
    {
        return ((IntPtr<SDL_Cursor>)SDL_CreateSystemCursor(cursor))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Cursor> Create(
        IntPtr<SDL_Surface> surface,
        int hotX,
        int hotY
    )
    {
        return ((IntPtr<SDL_Cursor>)SDL_CreateColorCursor(surface, hotX, hotY))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Cursor> Create(
        ReadOnlySpan<byte> data,
        ReadOnlySpan<byte> mask,
        int w,
        int h,
        int hotX,
        int hotY
    )
    {
        fixed (byte* dataPtr = data)
        fixed (byte* maskPtr = mask)
        {
            return ((IntPtr<SDL_Cursor>)SDL_CreateCursor(dataPtr, maskPtr, w, h, hotX, hotY))
                .AssertSdlNotNull();
        }
    }
}