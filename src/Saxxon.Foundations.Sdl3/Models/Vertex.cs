using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Vertex"/>.
/// </summary>
[PublicAPI]
public static class Vertex
{
    /// <summary>
    /// Retrieves the color field of an SDL_Vertex.
    /// </summary>
    public static SDL_FColor GetColor(this IntPtr<SDL_Vertex> vertex) =>
        vertex.AsReadOnlyRef().color;

    /// <summary>
    /// Retrieves the position of an SDL_Vertex.
    /// </summary>
    public static SDL_FPoint GetPosition(this IntPtr<SDL_Vertex> vertex) =>
        vertex.AsReadOnlyRef().position;

    /// <summary>
    /// Retrieves the texture coordinate of an SDL_Vertex.
    /// </summary>
    public static SDL_FPoint GetTexCoord(this IntPtr<SDL_Vertex> vertex) =>
        vertex.AsReadOnlyRef().tex_coord;

    /// <summary>
    /// Sets the color of an SDL_Vertex.
    /// </summary>
    public static void SetColor(this IntPtr<SDL_Vertex> vertex, SDL_FColor color) =>
        vertex.AsRef().color = color;

    /// <summary>
    /// Sets the position of an SDL_Vertex.
    /// </summary>
    public static void SetPosition(this IntPtr<SDL_Vertex> vertex, SDL_FPoint position) =>
        vertex.AsRef().position = position;

    /// <summary>
    /// Sets the texture coordinate of an SDL_Vertex.
    /// </summary>
    public static void SetTexCoord(this IntPtr<SDL_Vertex> vertex, SDL_FPoint texCoord) =>
        vertex.AsRef().tex_coord = texCoord;
}