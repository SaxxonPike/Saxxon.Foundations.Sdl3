using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

/// <summary>
/// A callback that describes a function that will be called when audio data on a track is available.
/// </summary>
/// <param name="func">
/// Target that will be invoked when the callback fires.
/// </param>
[PublicAPI]
public sealed unsafe class TrackMixCallback(TrackMixCallback.Del func) : IDisposable
{
    /// <summary>
    /// Delegate for the callback target.
    /// </summary>
    public delegate void Del(
        IntPtr<MIX_Track> track,
        SDL_AudioSpec spec,
        Span<float> pcm
    );

    /// <summary>
    /// SDL user data ID.
    /// </summary>
    public IntPtr UserData { get; } = UserDataStore.Add(func);

    /// <summary>
    /// Pointer to the static function that receives calls from SDL.
    /// </summary>
    internal static delegate* unmanaged[Cdecl]<IntPtr, MIX_Track*, SDL_AudioSpec*, float*, int, void> Callback => &Ingress;

    /// <summary>
    /// Ingress function from SDL.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Ingress(
        IntPtr userdata,
        MIX_Track* track,
        SDL_AudioSpec* spec,
        float* pcm,
        int samples
    )
    {
        if (UserDataStore.TryGet<Del>(userdata, out var handler))
            handler!(track, *spec, new Span<float>(pcm, samples));
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}