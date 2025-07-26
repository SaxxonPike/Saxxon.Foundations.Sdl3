using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Models;

namespace Saxxon.Foundations.Sdl3.Interop;

[PublicAPI]
public sealed class SdlException() : Exception(SdlError.Get());