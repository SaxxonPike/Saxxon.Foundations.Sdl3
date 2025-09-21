using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Models;

namespace Saxxon.Foundations.Sdl3.Interop;

/// <summary>
/// Represents an exception that contains an SDL-specific error message.
/// </summary>
[PublicAPI]
public sealed class SdlException() : Exception(SdlError.Get());