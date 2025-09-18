using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Delegates;

[PublicAPI]
public sealed unsafe class HintCallbackFunction(HintCallbackFunction.Del func) : IDisposable
{
    public delegate void Del(
        ReadOnlySpan<char> name,
        ReadOnlySpan<char> oldValue,
        ReadOnlySpan<char> newValue
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
        if (!UserDataStore.TryGet<Del>(userdata, out var handler)) 
            return;

        var nameLength = (int)SDL_strlen(name);
        var oldValueLength = (int)SDL_strlen(oldValue);
        var newValueLength = (int)SDL_strlen(newValue);
            
        var nameBytes = new Span<byte>(name, nameLength);
        var oldValueBytes = new Span<byte>(oldValue, oldValueLength);
        var newValueBytes = new Span<byte>(newValue, newValueLength);
            
        Span<char> nameChars = stackalloc char[Encoding.UTF8.GetCharCount(name, nameLength)];
        Span<char> oldValueChars = stackalloc char[Encoding.UTF8.GetCharCount(oldValue, oldValueLength)];
        Span<char> newValueChars = stackalloc char[Encoding.UTF8.GetCharCount(newValue, newValueLength)];

        Encoding.UTF8.GetChars(nameBytes, nameChars);
        Encoding.UTF8.GetChars(oldValueBytes, oldValueChars);
        Encoding.UTF8.GetChars(newValueBytes, newValueChars);
            
        handler!(
            nameChars,
            oldValueChars,
            newValueChars
        );
    }

    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}