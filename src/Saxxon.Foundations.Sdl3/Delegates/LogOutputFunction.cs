using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

/// <summary>
/// A callback for when messages are sent to the log.
/// </summary>
/// <param name="func">
/// Target that will be invoked when the callback fires.
/// </param>
[PublicAPI]
public sealed unsafe class LogOutputFunction(
    LogOutputFunction.Del func
) : IDisposable
{
    /// <summary>
    /// Delegate for the callback target.
    /// </summary>
    public delegate void Del(
        int category,
        SDL_LogPriority priority,
        ReadOnlySpan<char> message
    );

    /// <summary>
    /// SDL user data ID.
    /// </summary>
    public IntPtr UserData { get; } =
        UserDataStore.Add(func);

    /// <summary>
    /// Pointer to the static function that receives calls from SDL.
    /// </summary>
    internal static delegate* unmanaged[Cdecl]<IntPtr, int, SDL_LogPriority, byte*, void> Callback =>
        &Ingress;

    /// <summary>
    /// Ingress function from SDL.
    /// </summary>
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

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => 
        UserDataStore.Remove(UserData);
}