using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides object-oriented interfaces for <see cref="SDL_Rect"/> and
/// <see cref="SDL_FRect"/>.
/// </summary>
[PublicAPI]
public static class Rect
{
    public static unsafe (float X1, float Y1, float X2, float Y2)? IntersectLine(
        this SDL_FRect rect,
        float x1,
        float y1,
        float x2,
        float y2
    )
    {
        return SDL_GetRectAndLineIntersectionFloat(&rect, &x1, &y1, &x2, &y2)
            ? (x1, y1, x2, y2)
            : null;
    }

    public static unsafe (SDL_FPoint A, SDL_FPoint B)? IntersectLine(
        this SDL_FRect rect,
        SDL_FPoint a,
        SDL_FPoint b
    )
    {
        return SDL_GetRectAndLineIntersectionFloat(&rect, &a.x, &a.y, &b.x, &b.y)
            ? (Point.CreateFloat(a.x, a.y), Point.CreateFloat(b.x, b.y))
            : null;
    }

    public static unsafe (int X1, int Y1, int X2, int Y2)? IntersectLine(
        this SDL_Rect rect,
        int x1,
        int y1,
        int x2,
        int y2
    )
    {
        return SDL_GetRectAndLineIntersection(&rect, &x1, &y1, &x2, &y2)
            ? (x1, y1, x2, y2)
            : null;
    }

    public static unsafe (SDL_Point A, SDL_Point B)? IntersectLine(
        this SDL_Rect rect,
        SDL_Point a,
        SDL_Point b
    )
    {
        return SDL_GetRectAndLineIntersection(&rect, &a.x, &a.y, &b.x, &b.y)
            ? (Point.Create(a.x, a.y), Point.Create(b.x, b.y))
            : null;
    }

    public static unsafe SDL_FRect? Enclose(
        this ReadOnlySpan<SDL_FPoint> points,
        SDL_FRect? clip
    )
    {
        fixed (SDL_FPoint* pointsPtr = points)
        {
            SDL_FRect result;

            return SDL_GetRectEnclosingPointsFloat(
                pointsPtr,
                points.Length,
                clip is { } cr ? &cr : null,
                &result
            )
                ? result
                : null;
        }
    }

    public static unsafe SDL_Rect? Enclose(
        this ReadOnlySpan<SDL_Point> points,
        SDL_Rect? clip
    )
    {
        fixed (SDL_Point* pointsPtr = points)
        {
            SDL_Rect result;

            return SDL_GetRectEnclosingPoints(
                pointsPtr,
                points.Length,
                clip is { } cr ? &cr : null,
                &result
            )
                ? result
                : null;
        }
    }

    public static unsafe SDL_FRect? Union(
        this SDL_FRect a,
        SDL_FRect b
    )
    {
        SDL_FRect result;
        return SDL_GetRectUnionFloat(&a, &b, &result)
            ? result
            : null;
    }

    public static unsafe SDL_Rect? Union(
        this SDL_Rect a,
        SDL_Rect b
    )
    {
        SDL_Rect result;
        return SDL_GetRectUnion(&a, &b, &result)
            ? result
            : null;
    }

    public static unsafe SDL_FRect? Intersect(
        this SDL_FRect a,
        SDL_FRect b
    )
    {
        SDL_FRect result;
        return SDL_GetRectIntersectionFloat(&a, &b, &result)
            ? result
            : null;
    }

    public static unsafe SDL_Rect? Intersect(
        this SDL_Rect a,
        SDL_Rect b
    )
    {
        SDL_Rect result;
        return SDL_GetRectIntersection(&a, &b, &result)
            ? result
            : null;
    }

    public static unsafe bool Intersects(
        this SDL_FRect a,
        SDL_FRect b
    )
    {
        return SDL_HasRectIntersectionFloat(&a, &b);
    }

    public static unsafe bool Intersects(
        this SDL_Rect a,
        SDL_Rect b
    )
    {
        return SDL_HasRectIntersection(&a, &b);
    }

    /// <summary>
    /// Creates a new <see cref="SDL_FRect"/> from the given values.
    /// </summary>
    /// <param name="x">
    /// X-coordinate.
    /// </param>
    /// <param name="y">
    /// Y-coordinate.
    /// </param>
    /// <param name="width">
    /// Width of the rectangle.
    /// </param>
    /// <param name="height">
    /// Height of the rectangle.
    /// </param>
    public static SDL_FRect CreateFloat(float x, float y, float width, float height)
    {
        return new SDL_FRect
        {
            x = x,
            y = y,
            w = width,
            h = height
        };
    }

    /// <summary>
    /// Creates a new <see cref="SDL_Rect"/> from the given values.
    /// </summary>
    /// <param name="x">
    /// X-coordinate.
    /// </param>
    /// <param name="y">
    /// Y-coordinate.
    /// </param>
    /// <param name="width">
    /// Width of the rectangle.
    /// </param>
    /// <param name="height">
    /// Height of the rectangle.
    /// </param>
    public static SDL_Rect Create(int x, int y, int width, int height)
    {
        return new SDL_Rect
        {
            x = x,
            y = y,
            w = width,
            h = height
        };
    }

    /// <summary>
    /// Copies the points of a rectangle, in clockwise order from the upper
    /// left, to a buffer.
    /// </summary>
    /// <param name="rect">
    /// Rectangle to copy points from.
    /// </param>
    /// <param name="buffer">
    /// Buffer to copy points to.
    /// </param>
    public static void CopyPoints(
        this SDL_FRect rect,
        Span<SDL_FPoint> buffer
    )
    {
        buffer[0] = new SDL_FPoint { x = rect.x, y = rect.y };
        buffer[1] = new SDL_FPoint { x = rect.x + rect.w, y = rect.y };
        buffer[2] = new SDL_FPoint { x = rect.x + rect.w, y = rect.y + rect.h };
        buffer[3] = new SDL_FPoint { x = rect.x, y = rect.y + rect.h };
    }
}