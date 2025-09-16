using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="TTF_Text"/>.
/// </summary>
[PublicAPI]
public static class Text
{
    public static unsafe void DrawToSurface(this IntPtr<TTF_Text> text, int x, int y, IntPtr<SDL_Surface> surface)
    {
        TTF_DrawSurfaceText(text, x, y, surface)
            .AssertSdlSuccess();
    }

    public static unsafe void Draw(this IntPtr<TTF_Text> text, float x, float y)
    {
        TTF_DrawRendererText(text, x, y)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<TTF_GPUAtlasDrawSequence> GetGpuDrawData(this IntPtr<TTF_Text> text)
    {
        return ((IntPtr<TTF_GPUAtlasDrawSequence>)TTF_GetGPUTextDrawData(text))
            .AssertSdlNotNull();
    }

    public static unsafe SDL_PropertiesID GetProperties(this IntPtr<TTF_Text> text)
    {
        return TTF_GetTextProperties(text);
    }

    public static unsafe void SetTextEngine(this IntPtr<TTF_Text> text, IntPtr<TTF_TextEngine> engine)
    {
        TTF_SetTextEngine(text, engine)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<TTF_TextEngine> GetTextEngine(this IntPtr<TTF_Text> text)
    {
        return TTF_GetTextEngine(text);
    }

    public static unsafe void SetFont(this IntPtr<TTF_Text> text, IntPtr<TTF_Font> font)
    {
        TTF_SetTextFont(text, font)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<TTF_Font> GetTextFont(this IntPtr<TTF_Text> text)
    {
        return TTF_GetTextFont(text);
    }

    public static unsafe void SetDirection(this IntPtr<TTF_Text> text, TTF_Direction direction)
    {
        TTF_SetTextDirection(text, direction)
            .AssertSdlSuccess();
    }

    public static unsafe TTF_Direction GetDirection(this IntPtr<TTF_Text> text)
    {
        return TTF_GetTextDirection(text);
    }

    public static unsafe void SetScript(this IntPtr<TTF_Text> text, uint script)
    {
        TTF_SetTextScript(text, script)
            .AssertSdlSuccess();
    }

    public static unsafe uint GetScript(this IntPtr<TTF_Text> text)
    {
        return TTF_GetTextScript(text);
    }

    public static unsafe void SetColor(this IntPtr<TTF_Text> text, byte r, byte g, byte b, byte a)
    {
        TTF_SetTextColor(text, r, g, b, a)
            .AssertSdlSuccess();
    }

    public static unsafe void SetColor(this IntPtr<TTF_Text> text, SDL_Color color)
    {
        TTF_SetTextColor(text, color.r, color.g, color.b, color.a)
            .AssertSdlSuccess();
    }

    public static unsafe void SetColorFloat(this IntPtr<TTF_Text> text, float r, float g, float b, float a)
    {
        TTF_SetTextColorFloat(text, r, g, b, a)
            .AssertSdlSuccess();
    }

    public static unsafe void SetColorFloat(this IntPtr<TTF_Text> text, SDL_FColor color)
    {
        TTF_SetTextColorFloat(text, color.r, color.g, color.b, color.a)
            .AssertSdlSuccess();
    }

    public static unsafe SDL_Color GetColor(this IntPtr<TTF_Text> text)
    {
        byte r, g, b, a;
        TTF_GetTextColor(text, &r, &g, &b, &a)
            .AssertSdlSuccess();
        return new SDL_Color
        {
            r = r,
            g = g,
            b = b,
            a = a
        };
    }

    public static unsafe SDL_FColor GetColorFloat(this IntPtr<TTF_Text> text)
    {
        float r, g, b, a;
        TTF_GetTextColorFloat(text, &r, &g, &b, &a)
            .AssertSdlSuccess();
        return new SDL_FColor
        {
            r = r,
            g = g,
            b = b,
            a = a
        };
    }

    public static unsafe void SetPosition(this IntPtr<TTF_Text> text, int x, int y)
    {
        TTF_SetTextPosition(text, x, y)
            .AssertSdlSuccess();
    }

    public static unsafe (int X, int Y) GetPosition(this IntPtr<TTF_Text> text)
    {
        int x, y;
        TTF_GetTextPosition(text, &x, &y)
            .AssertSdlSuccess();
        return (x, y);
    }

    public static unsafe void SetWrapWidth(this IntPtr<TTF_Text> text, int wrapWidth)
    {
        TTF_SetTextWrapWidth(text, wrapWidth)
            .AssertSdlSuccess();
    }

    public static unsafe int GetWrapWidth(this IntPtr<TTF_Text> text)
    {
        int wrapWidth;
        TTF_GetTextWrapWidth(text, &wrapWidth)
            .AssertSdlSuccess();
        return wrapWidth;
    }

    public static unsafe void SetWrapWhitespaceVisible(this IntPtr<TTF_Text> text, bool visible)
    {
        TTF_SetTextWrapWhitespaceVisible(text, visible)
            .AssertSdlSuccess();
    }

    public static unsafe bool IsWrapWhitespaceVisible(this IntPtr<TTF_Text> text)
    {
        return TTF_TextWrapWhitespaceVisible(text);
    }

    public static unsafe void SetString(this IntPtr<TTF_Text> text, ReadOnlySpan<char> @string)
    {
        using var stringStr = new UnmanagedString(@string);
        TTF_SetTextString(text, stringStr, SDL_strlen(stringStr));
    }

    public static unsafe void InsertString(this IntPtr<TTF_Text> text, int offset, ReadOnlySpan<char> @string)
    {
        using var stringStr = new UnmanagedString(@string);
        TTF_InsertTextString(text, offset, stringStr, SDL_strlen(stringStr))
            .AssertSdlSuccess();
    }

    public static unsafe void AppendString(this IntPtr<TTF_Text> text, ReadOnlySpan<char> @string)
    {
        using var stringStr = new UnmanagedString(@string);
        TTF_AppendTextString(text, stringStr, SDL_strlen(stringStr))
            .AssertSdlSuccess();
    }
    
    public static unsafe void DeleteString(this IntPtr<TTF_Text> text, int offset, int length)
    {
        TTF_DeleteTextString(text, offset, length)
            .AssertSdlSuccess();
    }

    public static unsafe (int Width, int Height) GetSize(this IntPtr<TTF_Text> text)
    {
        int width, height;
        TTF_GetTextSize(text, &width, &height)
            .AssertSdlSuccess();
        return (width, height);
    }

    public static unsafe TTF_SubString GetSubstring(this IntPtr<TTF_Text> text, int offset)
    {
        TTF_SubString substring;
        TTF_GetTextSubString(text, offset, &substring)
            .AssertSdlSuccess();
        return substring;
    }

    public static unsafe TTF_SubString GetSubstringForLine(this IntPtr<TTF_Text> text, int line)
    {
        TTF_SubString substring;
        TTF_GetTextSubStringForLine(text, line, &substring)
            .AssertSdlSuccess();
        return substring;
    }

    public static unsafe IMemoryOwner<IntPtr<TTF_SubString>> GetSubstringsForRange(
        this IntPtr<TTF_Text> text,
        int offset,
        int length
    )
    {
        int count;
        var result = (IntPtr<IntPtr<TTF_SubString>>)(void*)TTF_GetTextSubStringsForRange(text, offset, length, &count);
        return SdlMemoryPool<IntPtr<TTF_SubString>>.Shared.Own(result, count);
    }

    public static unsafe TTF_SubString GetSubstringForPoint(this IntPtr<TTF_Text> text, int x, int y)
    {
        TTF_SubString substring;
        TTF_GetTextSubStringForPoint(text, x, y, &substring)
            .AssertSdlSuccess();
        return substring;
    }

    public static unsafe TTF_SubString GetPreviousSubstring(this IntPtr<TTF_Text> text, TTF_SubString substring)
    {
        TTF_SubString previous;
        TTF_GetPreviousTextSubString(text, &substring, &previous)
            .AssertSdlSuccess();
        return previous;
    }

    public static unsafe TTF_SubString GetNextSubstring(this IntPtr<TTF_Text> text, TTF_SubString substring)
    {
        TTF_SubString next;
        TTF_GetNextTextSubString(text, &substring, &next)
            .AssertSdlSuccess();
        return next;
    }

    public static unsafe void Update(this IntPtr<TTF_Text> text)
    {
        TTF_UpdateText(text)
            .AssertSdlSuccess();
    }

    public static unsafe void Destroy(this IntPtr<TTF_Text> text)
    {
        TTF_DestroyText(text);
    }

}