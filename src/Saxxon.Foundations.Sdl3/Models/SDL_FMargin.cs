namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Represents margins within a rectangle.
///
/// This is not an actual SDL type, but it is an encapsulation of the
/// rectangular margins used for nine-grid rendering.
/// </summary>
// ReSharper disable InconsistentNaming
public record struct SDL_FMargin
{
    public float l;
    public float t;
    public float r;
    public float b;
}