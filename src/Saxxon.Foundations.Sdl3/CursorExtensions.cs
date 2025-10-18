using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Cursor"/>.
/// </summary>
[PublicAPI]
public static class CursorExtensions
{
    extension(SDL_Cursor)
    {
        public static unsafe IntPtr<SDL_Cursor> Default =>
            SDL_GetDefaultCursor();

        public static unsafe IntPtr<SDL_Cursor> Current
        {
            get => SDL_GetCursor();
            set => SDL_SetCursor(value)
                .AssertSdlSuccess();
        }

        public static unsafe IntPtr<SDL_Cursor> CreateSystem(
            SDL_SystemCursor cursor
        ) => ((IntPtr<SDL_Cursor>)SDL_CreateSystemCursor(cursor))
            .AssertSdlNotNull();
        
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

        public static bool Visible =>
            SDL_CursorVisible();
    }

    extension(IntPtr<SDL_Cursor> cursor)
    {
        public unsafe void Destroy() => 
            SDL_DestroyCursor(cursor);
    }
}