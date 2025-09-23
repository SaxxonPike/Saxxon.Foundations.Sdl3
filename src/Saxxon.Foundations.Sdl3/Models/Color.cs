using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Color"/> and
/// <see cref="SDL_FColor"/>.
/// </summary>
[PublicAPI]
public static class Color
{
    /// <summary>
    /// Converts from <see cref="SDL_FColor"/> to <see cref="SDL_Color"/>.
    /// </summary>
    /// <remarks>
    /// The result of any input value outside the range 0-1 is undefined.
    /// </remarks>
    public static SDL_Color ToColor(this SDL_FColor color)
    {
        var value = Unsafe.As<SDL_FColor, Vector4>(ref color) * byte.MaxValue;

        return new SDL_Color
        {
            r = unchecked((byte)value.X),
            g = unchecked((byte)value.Y),
            b = unchecked((byte)value.Z),
            a = unchecked((byte)value.W)
        };
    }

    /// <summary>
    /// Converts from <see cref="SDL_Color"/> to <see cref="SDL_FColor"/>.
    /// </summary>
    public static SDL_FColor ToFColor(this SDL_Color color)
    {
        var value = new Vector4(color.r, color.g, color.b, color.a) / byte.MaxValue;
        return Unsafe.As<Vector4, SDL_FColor>(ref value);
    }

    /// <summary>
    /// Converts from <see cref="SDL_FColor"/> to <see cref="Vector3"/>.
    /// </summary>
    public static Vector3 ToVector3(this SDL_FColor color)
    {
        return Unsafe.As<SDL_FColor, Vector3>(ref color);
    }

    /// <summary>
    /// Converts from <see cref="SDL_Color"/> to <see cref="Vector3"/>.
    /// </summary>
    public static Vector3 ToVector3(this SDL_Color color)
    {
        return new Vector3(color.r, color.g, color.b) / byte.MaxValue;
    }

    /// <summary>
    /// Converts from <see cref="SDL_FColor"/> to <see cref="Vector4"/>.
    /// </summary>
    public static Vector4 ToVector4(this SDL_FColor color)
    {
        return Unsafe.As<SDL_FColor, Vector4>(ref color);
    }

    /// <summary>
    /// Converts from <see cref="SDL_Color"/> to <see cref="Vector4"/>.
    /// </summary>
    public static Vector4 ToVector4(this SDL_Color color)
    {
        return new Vector4(color.r, color.g, color.b, color.a) / byte.MaxValue;
    }

    /// <summary>
    /// Creates a new <see cref="SDL_FColor"/> from the given RGBA float values.
    /// </summary>
    /// <param name="r">
    /// Red component.
    /// </param>
    /// <param name="g">
    /// Green component.
    /// </param>
    /// <param name="b">
    /// Blue component.
    /// </param>
    /// <param name="a">
    /// Alpha component.
    /// </param>
    public static SDL_FColor CreateFloat(float r, float g, float b, float a = 1)
    {
        return new SDL_FColor
        {
            r = r,
            g = g,
            b = b,
            a = a
        };
    }

    /// <summary>
    /// Creates a new <see cref="SDL_Color"/> from the given RGBA byte values.
    /// </summary>
    /// <param name="r">
    /// Red component.
    /// </param>
    /// <param name="g">
    /// Green component.
    /// </param>
    /// <param name="b">
    /// Blue component.
    /// </param>
    /// <param name="a">
    /// Alpha component.
    /// </param>
    public static SDL_Color Create(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        return new SDL_Color
        {
            r = r,
            g = g,
            b = b,
            a = a
        };
    }
}