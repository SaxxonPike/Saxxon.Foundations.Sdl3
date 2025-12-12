using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

/// <summary>
/// A callback that fires when data passes through an <see cref="SDL_AudioStream"/>.
/// </summary>
/// <param name="func">
/// Target that will be invoked when the callback fires.
/// </param>
[PublicAPI]
public sealed unsafe class AudioStreamCallbackFunction(
    AudioStreamCallbackFunction.Del func
) : IDisposable
{
    /// <summary>
    /// Delegate for the callback target.
    /// </summary>
    public delegate void Del(
        IntPtr<SDL_AudioStream> audioStream,
        int additionalAmount,
        int totalAmount
    );

    /// <summary>
    /// SDL user data ID.
    /// </summary>
    public IntPtr UserData { get; } = 
        UserDataStore.Add(func);

    /// <summary>
    /// Pointer to the static function that receives calls from SDL.
    /// </summary>
    internal static delegate* unmanaged[Cdecl]<IntPtr, SDL_AudioStream*, int, int, void> Callback =>
        &Ingress;

    /// <summary>
    /// Ingress function from SDL.
    /// </summary>
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

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => 
        UserDataStore.Remove(UserData);
}