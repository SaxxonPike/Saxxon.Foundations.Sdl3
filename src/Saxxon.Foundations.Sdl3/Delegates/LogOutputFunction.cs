using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

[PublicAPI]
public sealed unsafe class LogOutputFunction(LogOutputFunction.Del func) : IDisposable
{
    public delegate void Del(
        int category,
        SDL_LogPriority priority,
        ReadOnlySpan<char> message
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
        if (!UserDataStore.TryGet<Del>(userdata, out var handler))
            return;

        var messageLength = (int)SDL_strlen(message);
        var messageBytes = new Span<byte>(message, messageLength);
        Span<char> messageChars = stackalloc char[Encoding.UTF8.GetCharCount(message, messageLength)];
        Encoding.UTF8.GetChars(messageBytes, messageChars);

        handler!(
            category,
            priority,
            messageChars
        );
    }

    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}