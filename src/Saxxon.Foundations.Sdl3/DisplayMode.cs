using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_DisplayMode"/>.
/// </summary>
[PublicAPI]
public static class DisplayMode
{
    /// <summary>
    /// Extensions for <see cref="SDL_DisplayMode"/> references.
    /// </summary>
    extension(IntPtr<SDL_DisplayMode> ptr)
    {
        public SDL_DisplayID DisplayId =>
            ptr.AsReadOnlyRef().displayID;

        public SDL_PixelFormat PixelFormat =>
            ptr.AsReadOnlyRef().format;

        public int Width =>
            ptr.AsReadOnlyRef().w;

        public int Height =>
            ptr.AsReadOnlyRef().h;

        public float RefreshRate =>
            ptr.AsReadOnlyRef().refresh_rate;

        public int RefreshRateNumerator =>
            ptr.AsReadOnlyRef().refresh_rate_numerator;

        public int RefreshRateDenominator =>
            ptr.AsReadOnlyRef().refresh_rate_denominator;
    }
}