using System.Text;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Extensions;

[PublicAPI]
internal static class SpanExtensions
{
    /// <summary>
    /// Measures the number of bytes required for the UTF-8 representation
    /// of the char span, adding 1 for the null-terminator.
    /// </summary>
    public static int MeasureUtf8(this ReadOnlySpan<char> span)
    {
        return Encoding.UTF8.GetByteCount(span) + 1;
    }

    /// <summary>
    /// Encodes a char span into a byte span, returning the number of bytes
    /// actually encoded, not including the null-terminator.
    /// </summary>
    public static int EncodeUtf8(this ReadOnlySpan<char> span, Span<byte> buffer)
    {
        var len = Encoding.UTF8.GetBytes(span, buffer);
        buffer[len] = 0;
        return len;
    }
}