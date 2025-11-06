using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

/// <summary>
/// A callback that fires when data is about to be fed to an audio device.
/// </summary>
/// <param name="func">
/// Target that will be invoked when the callback fires.
/// </param>
[PublicAPI]
public sealed unsafe class AudioDevicePostMixCallbackFunction(
    AudioDevicePostMixCallbackFunction.Del func
) : IDisposable
{
    /// <summary>
    /// Delegate for the callback target.
    /// </summary>
    public delegate void Del(
        SDL_AudioSpec spec,
        Span<float> buffer
    );

    /// <summary>
    /// SDL user data ID.
    /// </summary>
    public IntPtr UserData { get; } = 
        UserDataStore.Add(func);

    /// <summary>
    /// Pointer to the static function that receives calls from SDL.
    /// </summary>
    internal static delegate* unmanaged[Cdecl]<IntPtr, SDL_AudioSpec*, float*, int, void> Callback =>
        &Ingress;

    /// <summary>
    /// Ingress function from SDL.
    /// </summary>
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

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => 
        UserDataStore.Remove(UserData);
}