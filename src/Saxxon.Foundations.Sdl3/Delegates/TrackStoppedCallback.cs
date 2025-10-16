using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

/// <summary>
/// A callback that describes a function that will be called when an audio track stops.
/// </summary>
/// <param name="func">
/// Target that will be invoked when the callback fires.
/// </param>
[PublicAPI]
public sealed unsafe class TrackStoppedCallback(Action<IntPtr<MIX_Track>> func) : IDisposable
{
    /// <summary>
    /// SDL user data ID.
    /// </summary>
    public IntPtr UserData { get; } = UserDataStore.Add(func);

    /// <summary>
    /// Pointer to the static function that receives calls from SDL.
    /// </summary>
    internal static delegate* unmanaged[Cdecl]<IntPtr, MIX_Track*, void> Callback => &Ingress;

    /// <summary>
    /// Ingress function from SDL.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Ingress(
        IntPtr userdata,
        MIX_Track* track
    )
    {
        if (UserDataStore.TryGet<Action<IntPtr<MIX_Track>>>(userdata, out var handler))
            handler!(track);
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}