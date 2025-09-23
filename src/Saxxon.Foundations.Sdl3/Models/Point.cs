using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides object-oriented interfaces for <see cref="SDL_Point"/> and
/// <see cref="SDL_FPoint"/>.
/// </summary>
[PublicAPI]
public static class Point
{
    /// <summary>
    /// Creates a new <see cref="SDL_FPoint"/> with the specified values.
    /// </summary>
    /// <param name="x">
    /// X-coordinate.
    /// </param>
    /// <param name="y">
    /// Y-coordinate.
    /// </param>
    public static SDL_FPoint CreateFloat(float x, float y)
    {
        return new SDL_FPoint
        {
            x = x,
            y = y
        };
    }

    /// <summary>
    /// Creates a new <see cref="SDL_Point"/> with the specified values.
    /// </summary>
    /// <param name="x">
    /// X-coordinate.
    /// </param>
    /// <param name="y">
    /// Y-coordinate.
    /// </param>
    public static SDL_Point Create(int x, int y)
    {
        return new SDL_Point
        {
            x = x,
            y = y
        };
    }
}