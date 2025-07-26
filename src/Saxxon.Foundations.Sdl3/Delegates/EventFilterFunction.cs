using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

[PublicAPI]
public sealed unsafe class EventFilterFunction(
    EventFilterFunction.Del func
) : IDisposable
{
    public delegate bool Del(
        ref SDL_Event @event
    );

    public IntPtr UserData { get; } = UserDataStore.Add(func);
    public static delegate* unmanaged[Cdecl]<IntPtr, SDL_Event*, SDLBool> Callback => &Ingress;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool Ingress(
        IntPtr userdata,
        SDL_Event* @event
    )
    {
        return !UserDataStore.TryGet<Del>(userdata, out var handler) ||
               handler!(ref Unsafe.AsRef<SDL_Event>(@event));
    }

    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}