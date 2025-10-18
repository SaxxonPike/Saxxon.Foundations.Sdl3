using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Condition"/>.
/// See <see cref="System.Threading.Channels.Channel"/> for a fully managed
/// alternative.
/// </summary>
[PublicAPI]
public static class ConditionExtensions
{
    extension(SDL_Condition)
    {
        public static unsafe IntPtr<SDL_Condition> Create() =>
            SDL_CreateCondition();
    }

    extension(IntPtr<SDL_Condition> cond)
    {
        /// <summary>
        /// Waits until a condition variable is signaled or a certain time has
        /// passed.
        /// </summary>
        /// <param name="cond">
        /// The condition variable to wait on.
        /// </param>
        /// <param name="mutex">
        /// The mutex used to coordinate thread access.
        /// </param>
        /// <param name="timeout">
        /// The maximum time to wait, or null to wait indefinitely.
        /// </param>
        /// <returns>
        /// True if the condition variable is signaled, false if the condition is
        /// not signaled in the allotted time.
        /// </returns>
        public unsafe bool WaitTimeout(
            IntPtr<SDL_Mutex> mutex,
            TimeSpan? timeout
        ) => SDL_WaitConditionTimeout(cond, mutex, timeout?.ToMilliseconds() ?? -1);

        /// <summary>
        /// Waits until a condition variable is signaled.
        /// </summary>
        /// <param name="cond">
        /// The condition variable to wait on.
        /// </param>
        /// <param name="mutex">
        /// The mutex used to coordinate thread access.
        /// </param>
        public unsafe void Wait(
            IntPtr<SDL_Mutex> mutex
        ) => SDL_WaitCondition(cond, mutex);

        public unsafe void Broadcast() =>
            SDL_BroadcastCondition(cond);

        public unsafe void Signal() => 
            SDL_SignalCondition(cond);

        public unsafe void Destroy() => 
            SDL_DestroyCondition(cond);
    }
}