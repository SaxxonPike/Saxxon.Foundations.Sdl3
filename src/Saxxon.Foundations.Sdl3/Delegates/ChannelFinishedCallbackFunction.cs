using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Models;

namespace Saxxon.Foundations.Sdl3.Delegates;

[PublicAPI]
public sealed unsafe class ChannelFinishedCallbackFunction(
    ChannelFinishedCallbackFunction.Del func
) : IDisposable
{
    public delegate void Del(
        Mix_ChannelId channel
    );

    public Del? Handler { get; private set; } = func;

    private static List<Del> _handlers = [];
    
    public static delegate* unmanaged[Cdecl]<int, void> Callback => &Ingress;

    internal static void Add(Del handler)
    {
        if (_handlers.Count < 1)
            Mix_ChannelFinished(Callback);
        _handlers.Add(handler);
    }

    internal static void Remove(Del handler)
    {
        if (_handlers.Remove(handler) && _handlers.Count < 1)
            Mix_ChannelFinished(null);
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Ingress(
        int channel
    )
    {
        var channelValue = (Mix_ChannelId)channel;
        foreach (var handler in _handlers)
            handler(channelValue);
    }

    public void Dispose()
    {
        if (Handler == null)
            return;

        Remove(Handler);

        Handler = null;
    }
}