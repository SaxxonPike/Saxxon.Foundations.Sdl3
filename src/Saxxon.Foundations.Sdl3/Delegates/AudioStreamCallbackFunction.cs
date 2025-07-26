using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

[PublicAPI]
public sealed unsafe class AudioStreamCallbackFunction(
    AudioStreamCallbackFunction.Del func
) : IDisposable
{
    public delegate void Del(
        IntPtr<SDL_AudioStream> audioStream,
        int additionalAmount,
        int totalAmount
    );

    public IntPtr UserData { get; } = UserDataStore.Add(func);
    public static delegate* unmanaged[Cdecl]<IntPtr, SDL_AudioStream*, int, int, void> Callback => &Ingress;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Ingress(
        IntPtr userdata,
        SDL_AudioStream* stream,
        int additionalAmount,
        int totalAmount
    )
    {
        if (UserDataStore.TryGet<Del>(userdata, out var handler))
            handler!(
                stream,
                additionalAmount, totalAmount
            );
    }

    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}