using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

[PublicAPI]
public static class GpuRenderState
{
    public static unsafe void Destroy(this IntPtr<SDL_GPURenderState> state)
    {
        SDL_DestroyGPURenderState(state);
    }

    public static unsafe void SetFragmentUniforms(
        this IntPtr<SDL_GPURenderState> state,
        uint slotIndex,
        ReadOnlySpan<byte> data
    )
    {
        fixed (byte* dataPtr = data)
        {
            SDL_SetGPURenderStateFragmentUniforms(
                state,
                slotIndex,
                (IntPtr)dataPtr,
                unchecked((uint)data.Length)
            ).AssertSdlSuccess();
        }
    }
}