namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides object-oriented interfaces for <see cref="SDL_Size"/> and
/// <see cref="SDL_FSize"/>.
/// </summary>
public static class Size
{
    /// <summary>
    /// Creates a new <see cref="SDL_FSize"/> with the specified values.
    /// </summary>
    /// <param name="w">
    /// X-coordinate.
    /// </param>
    /// <param name="h">
    /// Y-coordinate.
    /// </param>
    public static SDL_FSize CreateFloat(float w, float h)
    {
        return new SDL_FSize
        {
            w = w,
            h = h
        };
    }

    /// <summary>
    /// Creates a new <see cref="SDL_Size"/> with the specified values.
    /// </summary>
    /// <param name="w">
    /// X-coordinate.
    /// </param>
    /// <param name="h">
    /// Y-coordinate.
    /// </param>
    public static SDL_Size Create(int w, int h)
    {
        return new SDL_Size
        {
            w = w,
            h = h
        };
    }

}