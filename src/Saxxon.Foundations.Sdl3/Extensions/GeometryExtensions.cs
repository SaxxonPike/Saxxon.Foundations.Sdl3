using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Extensions;

/// <summary>
/// Extensions for conversion and reinterpretation of geometry types.
/// </summary>
[PublicAPI]
public static class GeometryExtensions
{
    /// <summary>
    /// Reinterprets a span of <see cref="SDL_FRect"/> as a span of <see cref="Vector4"/>.
    /// </summary>
    public static Span<Vector4> AsVector4(this Span<SDL_FRect> rects)
    {
        return MemoryMarshal.Cast<SDL_FRect, Vector4>(rects);
    }

    /// <summary>
    /// Reinterprets a span of <see cref="SDL_FRect"/> as a span of <see cref="Vector4"/>.
    /// </summary>
    public static ReadOnlySpan<Vector4> AsVector4(this ReadOnlySpan<SDL_FRect> rects)
    {
        return MemoryMarshal.Cast<SDL_FRect, Vector4>(rects);
    }

    /// <summary>
    /// Reinterprets an <see cref="SDL_FRect"/> as a <see cref="Vector4"/>.
    /// </summary>
    public static ref Vector4 AsVector4(this ref SDL_FRect rect)
    {
        return ref Unsafe.As<SDL_FRect, Vector4>(ref rect);
    }

    /// <summary>
    /// Converts an <see cref="SDL_FRect"/> to a <see cref="Vector4"/>.
    /// </summary>
    public static Vector4 ToVector4(this SDL_FRect rect)
    {
        return Unsafe.As<SDL_FRect, Vector4>(ref rect);
    }

    /// <summary>
    /// Converts an <see cref="SDL_Rect"/> to a <see cref="Vector4"/>.
    /// </summary>
    public static Vector4 ToVector4(this SDL_Rect rect)
    {
        return Vector128.ConvertToSingle(
            Vector128.Create(rect.x, rect.y, rect.w, rect.h)
        ).AsVector4();
    }

    /// <summary>
    /// Determines whether two <see cref="SDL_FRect"/> are equal.
    /// </summary>
    public static bool IsEqualTo(this SDL_FRect a, SDL_FRect b)
    {
        return Unsafe.As<SDL_FRect, Vector128<float>>(ref a) == Unsafe.As<SDL_FRect, Vector128<float>>(ref b);
    }

    /// <summary>
    /// Determines whether two <see cref="SDL_Rect"/> are equal.
    /// </summary>
    public static bool IsEqualTo(this SDL_Rect a, SDL_Rect b)
    {
        return Unsafe.As<SDL_Rect, Vector128<int>>(ref a) == Unsafe.As<SDL_Rect, Vector128<int>>(ref b);
    }

    /// <summary>
    /// Reinterprets a span of <see cref="Vector4"/> as a span of <see cref="SDL_FRect"/>.
    /// </summary>
    public static Span<SDL_FRect> AsFRect(this Span<Vector4> rects)
    {
        return MemoryMarshal.Cast<Vector4, SDL_FRect>(rects);
    }

    /// <summary>
    /// Reinterprets a span of <see cref="Vector4"/> as a span of <see cref="SDL_FRect"/>.
    /// </summary>
    public static ReadOnlySpan<SDL_FRect> AsFRect(this ReadOnlySpan<Vector4> rects)
    {
        return MemoryMarshal.Cast<Vector4, SDL_FRect>(rects);
    }

    /// <summary>
    /// Reinterprets an <see cref="Vector4"/> as a <see cref="SDL_FRect"/>.
    /// </summary>
    public static ref SDL_FRect AsFRect(this ref Vector4 rect)
    {
        return ref Unsafe.As<Vector4, SDL_FRect>(ref rect);
    }

    /// <summary>
    /// Converts a <see cref="Vector4"/> to an <see cref="SDL_FRect"/>.
    /// </summary>
    public static SDL_FRect ToFRect(this Vector4 vec4)
    {
        return Unsafe.As<Vector4, SDL_FRect>(ref vec4);
    }

    /// <summary>
    /// Converts a <see cref="Vector4"/> to an <see cref="SDL_Rect"/>.
    /// </summary>
    public static SDL_Rect ToRect(this Vector4 vec4)
    {
        var val = Vector128.ConvertToInt32(
            vec4.AsVector128()
        );
        return Unsafe.As<Vector128<int>, SDL_Rect>(ref val);
    }

    /// <summary>
    /// Reinterprets a span of <see cref="SDL_FPoint"/> as a span of <see cref="Vector2"/>.
    /// </summary>
    public static Span<Vector2> AsVector2(this Span<SDL_FPoint> points)
    {
        return MemoryMarshal.Cast<SDL_FPoint, Vector2>(points);
    }

    /// <summary>
    /// Reinterprets a span of <see cref="SDL_FPoint"/> as a span of <see cref="Vector2"/>.
    /// </summary>
    public static ReadOnlySpan<Vector2> AsVector2(this ReadOnlySpan<SDL_FPoint> points)
    {
        return MemoryMarshal.Cast<SDL_FPoint, Vector2>(points);
    }

    /// <summary>
    /// Reinterprets an <see cref="SDL_FPoint"/> as a <see cref="Vector2"/>.
    /// </summary>
    public static ref Vector2 AsVector2(this ref SDL_FPoint point)
    {
        return ref Unsafe.As<SDL_FPoint, Vector2>(ref point);
    }

    /// <summary>
    /// Converts an <see cref="SDL_FPoint"/> to an <see cref="Vector2"/>.
    /// </summary>
    public static Vector2 ToVector2(this SDL_FPoint rect)
    {
        return Unsafe.As<SDL_FPoint, Vector2>(ref rect);
    }

    /// <summary>
    /// Converts an <see cref="SDL_Point"/> to an <see cref="Vector2"/>.
    /// </summary>
    public static Vector2 ToVector2(this SDL_Point rect)
    {
        var val = Vector64.ConvertToSingle(
            Vector64.Create(rect.x, rect.y)
        );
        return Unsafe.As<Vector64<float>, Vector2>(ref val);
    }

    /// <summary>
    /// Determines whether two <see cref="SDL_FPoint"/> are equal.
    /// </summary>
    public static bool IsEqualTo(this SDL_FPoint a, SDL_FPoint b)
    {
        return Unsafe.As<SDL_FPoint, Vector64<float>>(ref a) == Unsafe.As<SDL_FPoint, Vector64<float>>(ref b);
    }

    /// <summary>
    /// Determines whether two <see cref="SDL_Point"/> are equal.
    /// </summary>
    public static bool IsEqualTo(this SDL_Point a, SDL_Point b)
    {
        return Unsafe.As<SDL_Point, Vector64<int>>(ref a) == Unsafe.As<SDL_Point, Vector64<int>>(ref b);
    }

    /// <summary>
    /// Reinterprets a span of <see cref="Vector2"/> as a span of <see cref="SDL_FPoint"/>.
    /// </summary>
    public static Span<SDL_FPoint> AsFPoint(this Span<Vector2> points)
    {
        return MemoryMarshal.Cast<Vector2, SDL_FPoint>(points);
    }

    /// <summary>
    /// Reinterprets a span of <see cref="Vector2"/> as a span of <see cref="SDL_FPoint"/>.
    /// </summary>
    public static ReadOnlySpan<SDL_FPoint> AsFPoint(this ReadOnlySpan<Vector2> points)
    {
        return MemoryMarshal.Cast<Vector2, SDL_FPoint>(points);
    }

    /// <summary>
    /// Converts a <see cref="Vector2"/> to an <see cref="SDL_FPoint"/>.
    /// </summary>
    public static SDL_FPoint ToFPoint(this Vector2 vec2)
    {
        return Unsafe.As<Vector2, SDL_FPoint>(ref vec2);
    }

    /// <summary>
    /// Converts a <see cref="Vector2"/> to an <see cref="SDL_Point"/>.
    /// </summary>
    public static SDL_Point ToPoint(this Vector2 vec2)
    {
        var val = Vector128.ConvertToInt32(
            vec2.AsVector128()
        );
        return Unsafe.As<Vector128<int>, SDL_Point>(ref val);
    }
}