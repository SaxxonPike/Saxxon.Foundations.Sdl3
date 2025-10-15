using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for memory barriers.
/// 
/// Memory barriers are designed to prevent reads and writes from being
/// reordered by the compiler and being seen out of order on multicore CPUs.
/// </summary>
[PublicAPI]
public static class MemoryBarrier
{
    /// <summary>
    /// Insert a memory acquire barrier.
    /// </summary>
    public static void Acquire() => SDL_MemoryBarrierAcquireFunction();

    /// <summary>
    /// Insert a memory release barrier.
    /// </summary>
    public static void Release() => SDL_MemoryBarrierReleaseFunction();
}