using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_CursorFrameInfo"/>.
/// </summary>
[PublicAPI]
public static class CursorFrameInfo
{
    /// <summary>
    /// Extensions for use with <see cref="SDL_CursorFrameInfo"/> references.
    /// </summary>
    extension(SDL_CursorFrameInfo info)
    {
        /// <summary>
        /// Gets or sets the surface property.
        /// </summary>
        public unsafe IntPtr<SDL_Surface> Surface
        {
            get => info.surface;
            set => info.surface = value;
        }
    }
}