using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

/// <summary>
/// A callback that is used to filter SDL events based on type.
/// </summary>
/// <param name="func">
/// Target that will be invoked when the callback fires.
/// </param>
[PublicAPI]
public sealed unsafe class EventFilterFunction(
    EventFilterFunction.Del func
) : IDisposable
{
    /// <summary>
    /// Delegate for the callback target.
    /// </summary>
    public delegate bool Del(
        ref SDL_Event @event
    );

    /// <summary>
    /// SDL user data ID.
    /// </summary>
    public IntPtr UserData { get; } =
        UserDataStore.Add(func);

    /// <summary>
    /// Pointer to the static function that receives calls from SDL.
    /// </summary>
    internal static delegate* unmanaged[Cdecl]<IntPtr, SDL_Event*, SDLBool> Callback =>
        &Ingress;

    /// <summary>
    /// Ingress function from SDL.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool Ingress(
        IntPtr userdata,
        SDL_Event* @event) =>
        !UserDataStore.TryGet<Del>(userdata, out var handler) ||
        handler!(ref Unsafe.AsRef<SDL_Event>(@event));

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() =>
        UserDataStore.Remove(UserData);
}