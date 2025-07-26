using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class Condition
{
    public static unsafe bool WaitTimeout(
        this IntPtr<SDL_Condition> cond,
        IntPtr<SDL_Mutex> mutex,
        TimeSpan timeout
    )
    {
        return SDL_WaitConditionTimeout(cond, mutex, timeout.ToMilliseconds());
    }

    public static unsafe void Wait(
        this IntPtr<SDL_Condition> cond,
        IntPtr<SDL_Mutex> mutex
    )
    {
        SDL_WaitCondition(cond, mutex);
    }

    public static unsafe void Broadcast(
        this IntPtr<SDL_Condition> cond
    )
    {
        SDL_BroadcastCondition(cond);
    }

    public static unsafe void Signal(
        this IntPtr<SDL_Condition> cond
    )
    {
        SDL_SignalCondition(cond);
    }

    public static unsafe void Destroy(
        this IntPtr<SDL_Condition> cond
    )
    {
        SDL_DestroyCondition(cond);
    }

    public static unsafe IntPtr<SDL_Condition> Create()
    {
        return SDL_CreateCondition();
    }
}