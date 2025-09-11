using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class Event
{
    public static unsafe string GetDescription(this ref SDL_Event ev)
    {
        Span<byte> str = stackalloc byte[512];
        fixed (byte* ptr = str)
        {
            _ = SDL_GetEventDescription((SDL_Event*)Unsafe.AsPointer(ref ev), ptr, str.Length);
            return ((IntPtr)ptr).GetString()!;
        }
    }

    public static unsafe IntPtr<SDL_Window> GetWindow(this ref SDL_Event ev)
    {
        return SDL_GetWindowFromEvent((SDL_Event*)Unsafe.AsPointer(ref ev));
    }

    public static unsafe EventFilterFunction? GetFilter()
    {
        delegate* unmanaged[Cdecl]<IntPtr, SDL_Event*, SDLBool> filter;
        IntPtr userData;
        return SDL_GetEventFilter(&filter, &userData)
            ? UserDataStore.Get<EventFilterFunction>(userData)
            : null;
    }

    public static unsafe void Push(this ref SDL_Event ev)
    {
        SDL_PushEvent((SDL_Event*)Unsafe.AsPointer(ref ev))
            .AssertSdlSuccess();
    }

    public static unsafe bool WaitTimeout(this ref SDL_Event ev, TimeSpan timeout)
    {
        var ms = timeout.ToMilliseconds();
        return SDL_WaitEventTimeout((SDL_Event*)Unsafe.AsPointer(ref ev), ms);
    }

    public static unsafe bool Wait(this ref SDL_Event ev)
    {
        return SDL_WaitEvent((SDL_Event*)Unsafe.AsPointer(ref ev));
    }

    public static unsafe SDL_Event? Poll()
    {
        SDL_Event ev;
        if (SDL_PollEvent(&ev))
            return ev;
        return null;
    }

    public static unsafe Span<SDL_Event> Peep(
        Span<SDL_Event> events,
        SDL_EventAction action,
        uint minType,
        uint maxType
    )
    {
        fixed (SDL_Event* evPtr = events)
        {
            var count = SDL_PeepEvents(evPtr, events.Length, action, minType, maxType);
            if (count < 0)
                throw new SdlException();

            return events[..count];
        }
    }
}