using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Models;

namespace Saxxon.Foundations.Sdl3.Helpers;

[PublicAPI]
internal static class Wchar
{
    /// <summary>
    /// Gets the encoding for wchar_t strings.
    /// </summary>
    /// <remarks>
    /// It is unsure how reliable this method is for the time being.
    /// </remarks>
    public static Encoding Encoding =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Encoding.Unicode
            : Encoding.UTF32;

    /// <summary>
    /// Gets the width of a single wchar_t.
    /// </summary>
    public static int Size =>
        Encoding.GetMaxByteCount(1);

    /// <summary>
    /// Gets the length of a wchar_t string in bytes.
    /// </summary>
    /// <returns>
    /// The length of the string in bytes.
    /// </returns>
    public static unsafe int StrLen(void* ptr)
    {
        var result = 0;

        switch (Size)
        {
            case 2:
            {
                var code = (ushort*)ptr;
                while (*code++ != 0)
                    result += 2;
                break;
            }
            case 4:
            {
                var code = (uint*)ptr;
                while (*code++ != 0)
                    result += 4;
                break;
            }
            default:
            {
                throw new NotSupportedException();
            }
        }

        return result;
    }

    /// <summary>
    /// Reads a wchar_t string.
    /// </summary>
    public static unsafe string? ReadString(void* ptr)
    {
        if (ptr == null)
            return null;

        var len = StrLen(ptr);
        return Encoding.GetString(new ReadOnlySpan<byte>(ptr, len));
    }

    /// <summary>
    /// Allocates a wchar_t string in SDL memory. This needs to be freed with
    /// <see cref="Mem.Free"/>.
    /// </summary>
    public static unsafe IntPtr AllocString(ReadOnlySpan<char> value)
    {
        var enc = Encoding;
        var charLen = value.Length;
        var maxBytes = enc.GetMaxByteCount(charLen) + 4;
        var bytePtr = Mem.Alloc(maxBytes);
        var bytes = new Span<byte>((void*)bytePtr, maxBytes);
        var byteLen = enc.GetBytes(value, bytes);
        bytes[byteLen..].Clear();
        return bytePtr;
    }
}