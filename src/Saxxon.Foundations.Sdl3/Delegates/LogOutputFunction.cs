using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

[PublicAPI]
public sealed unsafe class LogOutputFunction(LogOutputFunction.Del func) : IDisposable
{
    public delegate void Del(
        int category,
        SDL_LogPriority priority,
        ReadOnlySpan<byte> message
    );

    public IntPtr UserData { get; } = UserDataStore.Add(func);
    public static delegate* unmanaged[Cdecl]<IntPtr, int, SDL_LogPriority, byte*, void> Callback => &Ingress;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Ingress(
        IntPtr userdata,
        int category,
        SDL_LogPriority priority,
        byte* message
    )
    {
        if (UserDataStore.TryGet<Del>(userdata, out var handler))
            handler!(
                category,
                priority,
                new ReadOnlySpan<byte>(message, (int)SDL_utf8strlen(message))
            );
    }

    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}