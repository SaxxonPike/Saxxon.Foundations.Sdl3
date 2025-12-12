using System.Text;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Extensions;

/// <summary>
/// Extensions for <see cref="StringBuilder"/>.
/// </summary>
[PublicAPI]
internal static class StringBuilderExtensions
{
    /// <summary>
    /// Appends a UTF8 unmanaged zero-terminated string to the end of the builder.
    /// </summary>
    public static unsafe void Append(this StringBuilder builder, byte* str)
    {
        var byteLen = (int)SDL_strlen(str);
        if (byteLen <= 0)
            return;

        Span<char> chars = stackalloc char[Encoding.UTF8.GetCharCount(str, byteLen)];

        fixed (char* charsPtr = chars)
            Encoding.UTF8.GetChars(str, byteLen, charsPtr, chars.Length);

        builder.Append(chars);
    }
}