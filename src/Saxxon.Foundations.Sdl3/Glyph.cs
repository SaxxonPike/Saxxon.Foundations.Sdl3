using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for font glyphs.
/// </summary>
[PublicAPI]
public static class Glyph
{
    public static uint GetScript(uint ch)
    {
        return TTF_GetGlyphScript(ch);
    }

}