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
        /// Gets or sets the color field of an SDL_Vertex reference.
        /// </summary>
        public SDL_FColor Color
        {
            get => vertex.AsReadOnlyRef().color;
            set => vertex.AsRef().color = value;
        }

        /// <summary>
        /// Gets or sets the position field of an SDL_Vertex reference.
        /// </summary>
        public SDL_FPoint Position
        {
            get => vertex.AsReadOnlyRef().position;
            set => vertex.AsRef().position = value;
        }

        /// <summary>
        /// Gets or sets the texture coordinate field of an SDL_Vertex reference.
        /// </summary>
        public SDL_FPoint TexCoord
        {
            get => vertex.AsReadOnlyRef().tex_coord;
            set => vertex.AsRef().tex_coord = value;
        }
    }
}