using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

[PublicAPI]
public sealed unsafe class AudioDevicePostMixCallbackFunction(
    AudioDevicePostMixCallbackFunction.Del func
) : IDisposable
{
    public delegate void Del(
        SDL_AudioSpec spec,
        Span<float> buffer
    );

    public IntPtr UserData { get; } = UserDataStore.Add(func);
    public static delegate* unmanaged[Cdecl]<IntPtr, SDL_AudioSpec*, float*, int, void> Callback => &Ingress;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Ingress(
        IntPtr userdata,
        SDL_AudioSpec* spec,
        float* buffer,
        int bufferLength
    )
    {
        if (UserDataStore.TryGet<Del>(userdata, out var handler))
            handler!(
                *spec,
                new Span<float>(buffer, bufferLength / sizeof(float))
            );
    }

    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}