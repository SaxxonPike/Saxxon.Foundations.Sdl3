using System.Collections.Concurrent;

namespace Saxxon.Foundations.Sdl3.Interop;

internal static class SdlMemory
{
    public static readonly ConcurrentDictionary<Type, object> Pools = [];
}