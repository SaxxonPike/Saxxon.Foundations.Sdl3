using System.Collections.Concurrent;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Provides access to SDL memory pools.
/// </summary>
internal static class SdlMemory
{
    public static readonly ConcurrentDictionary<Type, object> Pools = [];
}