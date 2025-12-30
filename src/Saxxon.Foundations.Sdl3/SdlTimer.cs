using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_TimerID"/>.
/// </summary>
[PublicAPI]
public static class SdlTimer
{
    public delegate uint TimerMsCallback(SDL_TimerID timer, uint elapsed);

    public delegate ulong TimerNsCallback(SDL_TimerID timer, ulong elapsed);

    public delegate TimeSpan? TimerCallback(SDL_TimerID timer, TimeSpan elapsed);

    public static unsafe SDL_TimerID CreateMilliseconds(TimerMsCallback? callback, uint interval)
    {
        if (callback == null)
            return default;

        var userData = UserDataStore.Add(callback);
        return SDL_AddTimer(interval, &Execute, userData);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static uint Execute(IntPtr userData, SDL_TimerID timer, uint elapsed)
        {
            return UserDataStore.TryGet<TimerMsCallback>(userData, out var target)
                ? target!.Invoke(timer, elapsed)
                : 0;
        }
    }

    public static unsafe SDL_TimerID CreateNanoseconds(TimerNsCallback? callback, ulong interval)
    {
        if (callback == null)
            return default;

        var userData = UserDataStore.Add(callback);
        return SDL_AddTimerNS(interval, &Execute, userData);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static ulong Execute(IntPtr userData, SDL_TimerID timer, ulong elapsed)
        {
            return UserDataStore.TryGet<TimerNsCallback>(userData, out var target)
                ? target!.Invoke(timer, elapsed)
                : 0;
        }
    }
    
    public static unsafe SDL_TimerID Create(TimerCallback? callback, TimeSpan interval)
    {
        if (callback == null)
            return default;

        var userData = UserDataStore.Add(callback);
        return SDL_AddTimerNS(interval.ToNanoseconds(), &Execute, userData);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static ulong Execute(IntPtr userData, SDL_TimerID timer, ulong elapsed)
        {
            return UserDataStore.TryGet<TimerCallback>(userData, out var target)
                ? target!.Invoke(timer, Time.GetFromNanoseconds(elapsed))?.ToNanoseconds() ?? elapsed
                : 0;
        }
    }
}