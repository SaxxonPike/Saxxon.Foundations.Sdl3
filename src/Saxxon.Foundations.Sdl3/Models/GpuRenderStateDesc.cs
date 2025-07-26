using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class GpuRenderStateDesc
{
    public static SDL_GPURenderStateDesc Init()
    {
        SDL_INIT_INTERFACE<SDL_GPURenderStateDesc>(out var result);
        return result;
    }
}