using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

[PublicAPI]
public sealed unsafe class EffectDoneCallbackFunction(
    EffectDoneCallbackFunction.Del func
) : IDisposable
{
    public delegate void Del(
        int channel
    );

    public IntPtr UserData { get; } = UserDataStore.Add(func);
    public static delegate* unmanaged[Cdecl]<int, IntPtr, void> Callback => &Ingress;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Ingress(
        int channel,
        IntPtr userdata
    )
    {
        if (UserDataStore.TryGet<Del>(userdata, out var handler))
            handler!(channel);
    }

    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}