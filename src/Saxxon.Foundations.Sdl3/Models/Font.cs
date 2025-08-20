using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="TTF_Font"/>.
/// </summary>
[PublicAPI]
public static class Font
{
    public static unsafe IntPtr<TTF_Font> Open(ReadOnlySpan<char> file, float ptSize)
    {
        var fileLen = file.MeasureUtf8();
        Span<byte> fileBytes = stackalloc byte[fileLen];
        file.EncodeUtf8(fileBytes);

        fixed (byte* filePtr = fileBytes)
            return ((IntPtr<TTF_Font>)TTF_OpenFont(filePtr, ptSize))
                .AssertSdlNotNull();
    }

    public static unsafe IntPtr<TTF_Font> OpenIo(IntPtr<SDL_IOStream> src, bool closeIo, float ptSize)
    {
        return ((IntPtr<TTF_Font>)TTF_OpenFontIO(src, closeIo, ptSize))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<TTF_Font> OpenWithProperties(SDL_PropertiesID props)
    {
        return ((IntPtr<TTF_Font>)TTF_OpenFontWithProperties(props))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<TTF_Font> Copy(this IntPtr<TTF_Font> existingFont)
    {
        return ((IntPtr<TTF_Font>)TTF_CopyFont(existingFont))
            .AssertSdlNotNull();
    }

    public static unsafe SDL_PropertiesID GetProperties(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontProperties(font);
    }

    public static unsafe uint GetGeneration(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontGeneration(font);
    }

    public static unsafe void AddFallback(this IntPtr<TTF_Font> font, IntPtr<TTF_Font> fallback)
    {
        TTF_AddFallbackFont(font, fallback)
            .AssertSdlSuccess();
    }

    public static unsafe void RemoveFallback(this IntPtr<TTF_Font> font, IntPtr<TTF_Font> fallback)
    {
        TTF_RemoveFallbackFont(font, fallback);
    }

    public static unsafe void ClearFallbacks(this IntPtr<TTF_Font> font)
    {
        TTF_ClearFallbackFonts(font);
    }

    public static unsafe void SetSize(this IntPtr<TTF_Font> font, float ptSize)
    {
        TTF_SetFontSize(font, ptSize)
            .AssertSdlSuccess();
    }

    public static unsafe void SetSizeDpi(this IntPtr<TTF_Font> font, float ptSize, int hDpi, int vDpi)
    {
        TTF_SetFontSizeDPI(font, ptSize, hDpi, vDpi)
            .AssertSdlSuccess();
    }

    public static unsafe float GetSize(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontSize(font);
    }

    public static unsafe (int HDpi, int VDpi) GetDpi(this IntPtr<TTF_Font> font)
    {
        int hDpi, vDpi;
        TTF_GetFontDPI(font, &hDpi, &vDpi)
            .AssertSdlSuccess();
        return (hDpi, vDpi);
    }

    public static unsafe void SetStyle(this IntPtr<TTF_Font> font, TTF_FontStyleFlags flags)
    {
        TTF_SetFontStyle(font, flags);
    }

    public static unsafe TTF_FontStyleFlags GetStyle(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontStyle(font);
    }

    public static unsafe void SetOutline(this IntPtr<TTF_Font> font, int outline)
    {
        TTF_SetFontOutline(font, outline)
            .AssertSdlSuccess();
    }

    public static unsafe int GetOutline(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontOutline(font);
    }

    public static unsafe void SetHinting(this IntPtr<TTF_Font> font, TTF_HintingFlags hinting)
    {
        TTF_SetFontHinting(font, hinting);
    }

    public static unsafe int GetNumFaces(this IntPtr<TTF_Font> font)
    {
        return TTF_GetNumFontFaces(font);
    }

    public static unsafe TTF_HintingFlags GetHinting(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontHinting(font);
    }

    public static unsafe void SetSdf(this IntPtr<TTF_Font> font, bool enabled)
    {
        TTF_SetFontSDF(font, enabled);
    }

    public static unsafe bool GetSdf(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontSDF(font);
    }

    public static unsafe int GetWeight(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontWeight(font);
    }

    public static unsafe void SetWrapAlignment(this IntPtr<TTF_Font> font, TTF_HorizontalAlignment align)
    {
        TTF_SetFontWrapAlignment(font, align);
    }

    public static unsafe TTF_HorizontalAlignment GetWrapAlignment(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontWrapAlignment(font);
    }

    public static unsafe int GetHeight(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontHeight(font);
    }

    public static unsafe int GetFontDescent(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontDescent(font);
    }

    public static unsafe void SetLineSkip(this IntPtr<TTF_Font> font, int lineSkip)
    {
        TTF_SetFontLineSkip(font, lineSkip);
    }

    public static unsafe int GetLineSkip(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontLineSkip(font);
    }

    public static unsafe void SetKerning(this IntPtr<TTF_Font> font, bool enabled)
    {
        TTF_SetFontKerning(font, enabled);
    }

    public static unsafe bool GetKerning(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontKerning(font);
    }

    public static unsafe bool IsFixedWidth(this IntPtr<TTF_Font> font)
    {
        return TTF_FontIsFixedWidth(font);
    }

    public static unsafe bool IsScalable(this IntPtr<TTF_Font> font)
    {
        return TTF_FontIsScalable(font);
    }

    public static unsafe string? GetFamilyName(this IntPtr<TTF_Font> font)
    {
        return Marshal.PtrToStringUTF8((IntPtr)Unsafe_TTF_GetFontFamilyName(font));
    }

    public static unsafe string? GetStyleName(this IntPtr<TTF_Font> font)
    {
        return Marshal.PtrToStringUTF8((IntPtr)Unsafe_TTF_GetFontStyleName(font));
    }

    public static unsafe void SetDirection(this IntPtr<TTF_Font> font, TTF_Direction direction)
    {
        TTF_SetFontDirection(font, direction)
            .AssertSdlSuccess();
    }

    public static unsafe TTF_Direction GetDirection(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontDirection(font);
    }

    public static unsafe void SetScript(this IntPtr<TTF_Font> font, uint script)
    {
        TTF_SetFontScript(font, script)
            .AssertSdlSuccess();
    }

    public static unsafe uint GetScript(this IntPtr<TTF_Font> font)
    {
        return TTF_GetFontScript(font);
    }

    public static unsafe void SetLanguage(this IntPtr<TTF_Font> font, ReadOnlySpan<char> language)
    {
        var languageLen = language.MeasureUtf8();
        Span<byte> languageBytes = stackalloc byte[languageLen];
        language.EncodeUtf8(languageBytes);

        fixed (byte* languagePtr = languageBytes)
            TTF_SetFontLanguage(font, languagePtr)
                .AssertSdlSuccess();
    }

    public static unsafe bool HasGlyph(this IntPtr<TTF_Font> font, uint ch)
    {
        return TTF_FontHasGlyph(font, ch);
    }

    public static unsafe (IntPtr<SDL_Surface> Surface, TTF_ImageType ImageType) GetGlyphImage(
        this IntPtr<TTF_Font> font,
        uint ch
    )
    {
        TTF_ImageType imageType;
        var surface = ((IntPtr<SDL_Surface>)TTF_GetGlyphImage(font, ch, &imageType))
            .AssertSdlNotNull();
        return (surface, imageType);
    }

    public static unsafe (IntPtr<SDL_Surface> Surface, TTF_ImageType ImageType) GetGlyphImageForIndex(
        this IntPtr<TTF_Font> font,
        uint glyphIndex
    )
    {
        TTF_ImageType imageType;
        var surface = ((IntPtr<SDL_Surface>)TTF_GetGlyphImageForIndex(font, glyphIndex, &imageType))
            .AssertSdlNotNull();
        return (surface, imageType);
    }

    public static unsafe (int MinX, int MaxX, int MinY, int MaxY, int Advance) GetGlyphMetrics(
        this IntPtr<TTF_Font> font,
        uint ch
    )
    {
        int minX, maxX, minY, maxY, advance;
        TTF_GetGlyphMetrics(font, ch, &minX, &maxX, &minY, &maxY, &advance)
            .AssertSdlSuccess();
        return (minX, maxX, minY, maxY, advance);
    }

    public static unsafe int GetGlyphKerning(
        this IntPtr<TTF_Font> font,
        uint previousCh,
        uint ch
    )
    {
        int kerning;
        TTF_GetGlyphKerning(font, previousCh, ch, &kerning)
            .AssertSdlSuccess();
        return kerning;
    }

    public static unsafe (int W, int H) GetStringSize(
        this IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text
    )
    {
        int w, h;

        var textLen = text.MeasureUtf8();
        Span<byte> textBytes = stackalloc byte[textLen];
        text.EncodeUtf8(textBytes);

        fixed (byte* textPtr = textBytes)
            TTF_GetStringSize(font, textPtr, (UIntPtr)(textLen - 1), &w, &h)
                .AssertSdlSuccess();

        return (w, h);
    }

    public static unsafe (int Width, int Height) GetStringSizeWrapped(
        this IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text,
        int wrapWidth
    )
    {
        int w, h;

        var textLen = text.MeasureUtf8();
        Span<byte> textBytes = stackalloc byte[textLen];
        text.EncodeUtf8(textBytes);

        fixed (byte* textPtr = textBytes)
            TTF_GetStringSizeWrapped(font, textPtr, (UIntPtr)(textLen - 1), wrapWidth, &w, &h)
                .AssertSdlSuccess();

        return (w, h);
    }

    public static unsafe (int Width, int Length) MeasureString(
        this IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text,
        int maxWidth
    )
    {
        int width;
        UIntPtr length;

        var textLen = text.MeasureUtf8();
        Span<byte> textBytes = stackalloc byte[textLen];
        text.EncodeUtf8(textBytes);

        fixed (byte* textPtr = textBytes)
            TTF_MeasureString(font, textPtr, (UIntPtr)(textLen - 1), maxWidth, &width, &length)
                .AssertSdlSuccess();

        return (width, (int)length);
    }

    public static unsafe IntPtr<SDL_Surface> RenderTextSolid(
        this IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text,
        SDL_Color fg
    )
    {
        var textLen = text.MeasureUtf8();
        Span<byte> textBytes = stackalloc byte[textLen];
        text.EncodeUtf8(textBytes);

        fixed (byte* textPtr = textBytes)
            return ((IntPtr<SDL_Surface>)TTF_RenderText_Solid(font, textPtr, (UIntPtr)(textLen - 1), fg))
                .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> RenderTextSolidWrapped(
        this IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text,
        SDL_Color fg,
        int wrapLength
    )
    {
        var textLen = text.MeasureUtf8();
        Span<byte> textBytes = stackalloc byte[textLen];
        text.EncodeUtf8(textBytes);

        fixed (byte* textPtr = textBytes)
            return ((IntPtr<SDL_Surface>)TTF_RenderText_Solid_Wrapped(font, textPtr, (UIntPtr)(textLen - 1), fg, wrapLength))
                .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> RenderGlyphSolid(
        this IntPtr<TTF_Font> font,
        uint ch,
        SDL_Color fg
    )
    {
        return ((IntPtr<SDL_Surface>)TTF_RenderGlyph_Solid(font, ch, fg))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> RenderTextShaded(
        this IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text,
        SDL_Color fg,
        SDL_Color bg
    )
    {
        var textLen = text.MeasureUtf8();
        Span<byte> textBytes = stackalloc byte[textLen];
        text.EncodeUtf8(textBytes);

        fixed (byte* textPtr = textBytes)
            return ((IntPtr<SDL_Surface>)TTF_RenderText_Shaded(font, textPtr, (UIntPtr)(textLen - 1), fg, bg))
                .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> RenderTextShadedWrapped(
        this IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text,
        SDL_Color fg,
        SDL_Color bg,
        int wrapLength
    )
    {
        var textLen = text.MeasureUtf8();
        Span<byte> textBytes = stackalloc byte[textLen];
        text.EncodeUtf8(textBytes);

        fixed (byte* textPtr = textBytes)
            return ((IntPtr<SDL_Surface>)TTF_RenderText_Shaded_Wrapped(font, textPtr, (UIntPtr)(textLen - 1), fg, bg,
                    wrapLength))
                .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> RenderGlyphShaded(
        this IntPtr<TTF_Font> font,
        uint ch,
        SDL_Color fg,
        SDL_Color bg
    )
    {
        return ((IntPtr<SDL_Surface>)TTF_RenderGlyph_Shaded(font, ch, fg, bg))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> RenderTextBlended(
        this IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text,
        SDL_Color fg
    )
    {
        var textLen = text.MeasureUtf8();
        Span<byte> textBytes = stackalloc byte[textLen];
        text.EncodeUtf8(textBytes);

        fixed (byte* textPtr = textBytes)
            return ((IntPtr<SDL_Surface>)TTF_RenderText_Blended(font, textPtr, (UIntPtr)(textLen - 1), fg))
                .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> RenderTextBlendedWrapped(
        this IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text,
        SDL_Color fg,
        int wrapLength
    )
    {
        var textLen = text.MeasureUtf8();
        Span<byte> textBytes = stackalloc byte[textLen];
        text.EncodeUtf8(textBytes);

        fixed (byte* textPtr = textBytes)
            return ((IntPtr<SDL_Surface>)TTF_RenderText_Blended_Wrapped(font, textPtr, (UIntPtr)(textLen - 1), fg, wrapLength))
                .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> RenderGlyphBlended(
        this IntPtr<TTF_Font> font,
        uint ch,
        SDL_Color fg
    )
    {
        return ((IntPtr<SDL_Surface>)TTF_RenderGlyph_Blended(font, ch, fg))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> RenderTextLcd(
        this IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text,
        SDL_Color fg,
        SDL_Color bg
    )
    {
        var textLen = text.MeasureUtf8();
        Span<byte> textBytes = stackalloc byte[textLen];
        text.EncodeUtf8(textBytes);

        fixed (byte* textPtr = textBytes)
            return ((IntPtr<SDL_Surface>)TTF_RenderText_LCD(font, textPtr, (UIntPtr)(textLen - 1), fg, bg))
                .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> RenderTextLcdWrapped(
        this IntPtr<TTF_Font> font,
        ReadOnlySpan<char> text,
        SDL_Color fg,
        SDL_Color bg,
        int wrapLength
    )
    {
        var textLen = text.MeasureUtf8();
        Span<byte> textBytes = stackalloc byte[textLen];
        text.EncodeUtf8(textBytes);

        fixed (byte* textPtr = textBytes)
            return ((IntPtr<SDL_Surface>)TTF_RenderText_LCD_Wrapped(font, textPtr, (UIntPtr)(textLen - 1), fg, bg, wrapLength))
                .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Surface> RenderGlyphLcd(
        this IntPtr<TTF_Font> font,
        uint ch,
        SDL_Color fg,
        SDL_Color bg
    )
    {
        return ((IntPtr<SDL_Surface>)TTF_RenderGlyph_LCD(font, ch, fg, bg))
            .AssertSdlNotNull();
    }

    public static unsafe void Close(this IntPtr<TTF_Font> font)
    {
        TTF_CloseFont(font);
    }
}