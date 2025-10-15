using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for initialization state during the
/// application lifetime.
/// </summary>
[PublicAPI]
public static class InitState
{
    public static unsafe void SetInitialized(
        this IntPtr<SDL_InitState> state,
        bool initialized
    )
    {
        SDL_SetInitialized(state, initialized);
    }

    public static unsafe bool ShouldQuit(
        this IntPtr<SDL_InitState> state
    )
    {
        return SDL_ShouldQuit(state);
    }

    public static unsafe bool ShouldInit(
        this IntPtr<SDL_InitState> state
    )
    {
        return SDL_ShouldInit(state);
    }
}