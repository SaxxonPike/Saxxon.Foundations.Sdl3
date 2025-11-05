using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

/// <summary>
/// A callback that describes a function that will be called when audio data on a mixer is available.
/// </summary>
/// <param name="func">
/// Target that will be invoked when the callback fires.
/// </param>
[PublicAPI]
public sealed unsafe class MixerMixCallback(Action<IntPtr<MIX_Mixer>, SDL_AudioSpec, Span<float>> func) : IDisposable
{
    /// <summary>
    /// SDL user data ID.
    /// </summary>
    public IntPtr UserData { get; } = UserDataStore.Add(func);

    /// <summary>
    /// Pointer to the static function that receives calls from SDL.
    /// </summary>
    internal static delegate* unmanaged[Cdecl]<IntPtr, MIX_Mixer*, SDL_AudioSpec*, float*, int, void> Callback => &Ingress;

    /// <summary>
    /// Ingress function from SDL.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Ingress(
        IntPtr userdata,
        MIX_Mixer* mixer,
        SDL_AudioSpec* spec,
        float* pcm,
        int samples
    )
    {
        if (UserDataStore.TryGet<Action<IntPtr<MIX_Mixer>, SDL_AudioSpec, Span<float>>>(userdata, out var handler))
            handler!(mixer, *spec, new Span<float>(pcm, samples));
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}