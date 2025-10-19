using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Pointer to a null-terminated UTF-8 formatted byte string.
/// </summary>
[PublicAPI]
public readonly record struct Utf8StringPtr(IntPtr<byte> Bytes)
{
    public static implicit operator string?(Utf8StringPtr value) =>
        value.Bytes.GetString();

    public static implicit operator ReadOnlySpan<byte>(Utf8StringPtr value) =>
        value.Bytes.GetUtf8Span();

    /// <summary>
    /// Converts the bytes to a .NET string.
    /// </summary>
    public override string? ToString() =>
        Bytes.GetString();
}