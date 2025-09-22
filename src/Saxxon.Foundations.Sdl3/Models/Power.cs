using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for retrieving power state.
/// </summary>
[PublicAPI]
public static class Power
{
    public static unsafe (SDL_PowerState State, TimeSpan Time, int Percent) GetInfo()
    {
        int seconds, percent;
        var result = SDL_GetPowerInfo(&seconds, &percent);
        return result == SDL_PowerState.SDL_POWERSTATE_ERROR 
            ? throw new SdlException() 
            : (result, TimeSpan.FromSeconds(seconds), percent);
    }
}