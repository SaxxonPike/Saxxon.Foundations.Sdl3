using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Vertex"/>.
/// </summary>
[PublicAPI]
public static class Vertex
{
    /// <summary>
    /// Extensions for <see cref="SDL_Vertex"/> references.
    /// </summary>
    extension(IntPtr<SDL_Vertex> vertex)
    {
        /// <summary>
        /// Color of an <see cref="SDL_Vertex"/>.
        /// </summary>
        public ref SDL_FColor Color =>
            ref vertex.AsRef().color;

        /// <summary>
        /// Position of an <see cref="SDL_Vertex"/>.
        /// </summary>
        public ref SDL_FPoint Position =>
            ref vertex.AsRef().position;

        /// <summary>
        /// Texture coordinate of an <see cref="SDL_Vertex"/>.
        /// </summary>
        public ref SDL_FPoint TexCoord =>
            ref vertex.AsRef().tex_coord;
    }
}