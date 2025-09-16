using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

[PublicAPI]
public sealed unsafe class HintCallbackFunction(HintCallbackFunction.Del func) : IDisposable
{
    public delegate void Del(
        string name,
        string? oldValue,
        string? newValue
    );

    public IntPtr UserData { get; } = UserDataStore.Add(func);
    public static delegate* unmanaged[Cdecl]<IntPtr, byte*, byte*, byte*, void> Callback => &Ingress;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Ingress(
        IntPtr userdata,
        byte* name,
        byte* oldValue,
        byte* newValue
    )
    {
        if (UserDataStore.TryGet<Del>(userdata, out var handler))
        {
            handler!(
                Ptr.ToUtf8String(name)!,
                Ptr.ToUtf8String(oldValue),
                Ptr.ToUtf8String(newValue)
            );
        }
    }

    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}