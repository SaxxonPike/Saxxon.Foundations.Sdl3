using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class MainThread
{
    public static unsafe void Run(Action func, bool waitComplete)
    {
        var userData = UserDataStore.Add(func);

        SDL_RunOnMainThread(&Execute, userData, waitComplete)
            .AssertSdlSuccess();

        return;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static void Execute(IntPtr userData)
        {
            UserDataStore.Remove<Action>(userData, out var target);
            target?.Invoke();
        }
    }

    public static bool IsCurrent()
    {
        return SDL_IsMainThread();
    }
}