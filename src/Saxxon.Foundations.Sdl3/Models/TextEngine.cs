using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="TTF_TextEngine"/>.
/// </summary>
[PublicAPI]
public static class TextEngine
{
    public static unsafe IntPtr<TTF_TextEngine> CreateSurfaceTextEngine()
    {
        return ((IntPtr<TTF_TextEngine>)TTF_CreateSurfaceTextEngine())
            .AssertSdlNotNull();
    }

    public static unsafe void DestroySurfaceTextEngine(this IntPtr<TTF_TextEngine> textEngine)
    {
        TTF_DestroySurfaceTextEngine(textEngine);
    }

    public static unsafe IntPtr<TTF_TextEngine> CreateRendererTextEngine(IntPtr<SDL_Renderer> renderer)
    {
        return ((IntPtr<TTF_TextEngine>)TTF_CreateRendererTextEngine(renderer))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<TTF_TextEngine> CreateRendererTextEngineWithProperties(SDL_PropertiesID props)
    {
        return ((IntPtr<TTF_TextEngine>)TTF_CreateRendererTextEngineWithProperties(props))
            .AssertSdlNotNull();
    }

    public static unsafe void DestroyRendererTextEngine(this IntPtr<TTF_TextEngine> textEngine)
    {
        TTF_DestroyRendererTextEngine(textEngine);
    }

    public static unsafe IntPtr<TTF_TextEngine> CreateGpuTextEngine(IntPtr<SDL_GPUDevice> device)
    {
        return ((IntPtr<TTF_TextEngine>)TTF_CreateGPUTextEngine(device))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<TTF_TextEngine> CreateGpuTextEngineWithProperties(SDL_PropertiesID props)
    {
        return TTF_CreateGPUTextEngineWithProperties(props);
    }

    public static unsafe void DestroyGpuTextEngine(this IntPtr<TTF_TextEngine> engine)
    {
        TTF_DestroyGPUTextEngine(engine);
    }
    
    public static unsafe void SetWinding(this IntPtr<TTF_TextEngine> engine, TTF_GPUTextEngineWinding winding)
    {
        TTF_SetGPUTextEngineWinding(engine, winding);
    }

    public static unsafe TTF_GPUTextEngineWinding GetWinding(this IntPtr<TTF_TextEngine> engine)
    {
        return TTF_GetGPUTextEngineWinding(engine);
    }

    public static unsafe IntPtr<TTF_Text> CreateText(
        this IntPtr<TTF_TextEngine> engine,
        IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text
    )
    {
        using var textStr = new UnmanagedString(text);
        return ((IntPtr<TTF_Text>)TTF_CreateText(engine, font, textStr, SDL_strlen(textStr)))
            .AssertSdlNotNull();
    }

    

}