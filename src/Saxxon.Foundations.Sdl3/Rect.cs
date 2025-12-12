using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides object-oriented interfaces for <see cref="SDL_Rect"/> and
/// <see cref="SDL_FRect"/>.
/// </summary>
[PublicAPI]
public static class Rect
{
    /// <summary>
    /// Calculates the intersection of a rectangle and line segment.
    /// </summary>
    /// <param name="rect">
    /// An <see cref="SDL_FRect"/> structure representing the rectangle to intersect.
    /// </param>
    /// <param name="x1">
    /// The starting X-coordinate of the line.
    /// </param>
    /// <param name="y1">
    /// The starting Y-coordinate of the line.
    /// </param>
    /// <param name="x2">
    /// The ending X-coordinate of the line.
    /// </param>
    /// <param name="y2">
    /// The ending Y-coordinate of the line.
    /// </param>
    /// <returns>
    /// The clipped line segment if there is an intersection, null otherwise.
    /// </returns>
    /// <remarks>
    /// This function is used to clip a line segment to a rectangle. A line segment
    /// contained entirely within the rectangle or that does not intersect will remain unchanged.
    /// A line segment that crosses the rectangle at either or both ends will be clipped to
    /// the boundary of the rectangle and the new coordinates saved in X1, Y1, X2, and/or Y2
    /// as necessary.
    /// </remarks>
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

    /// <summary>
    /// Calculates the intersection of a rectangle and line segment.
    /// </summary>
    /// <param name="rect">
    /// An <see cref="SDL_FRect"/> structure representing the rectangle to intersect.
    /// </param>
    /// <param name="a">
    /// The starting coordinates of the line.
    /// </param>
    /// <param name="b">
    /// The ending coordinates of the line.
    /// </param>
    /// <returns>
    /// The clipped line segment if there is an intersection, null otherwise.
    /// </returns>
    /// <remarks>
    /// This function is used to clip a line segment to a rectangle. A line segment
    /// contained entirely within the rectangle or that does not intersect will remain unchanged.
    /// A line segment that crosses the rectangle at either or both ends will be clipped to
    /// the boundary of the rectangle and the new coordinates saved in A and/or B
    /// as necessary.
    /// </remarks>
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

    /// <inheritdoc cref="IntersectLine(SDL_FRect,float,float,float,float)"/>
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

    /// <inheritdoc cref="IntersectLine(SDL_FRect,SDL_FPoint,SDL_FPoint)"/>
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

    /// <summary>
    /// Calculates a minimal rectangle enclosing a set of points.
    /// </summary>
    /// <param name="points">
    /// Points to be enclosed.
    /// </param>
    /// <param name="clip">
    /// A rectangle to be used for clipping, or null to enclose all points.
    /// </param>
    /// <returns>
    /// The minimal enclosing rectangle, or null if all the points were outside the clipping rectangle.
    /// </returns>
    /// <remarks>
    /// If clip is not null, only points inside the clipping rectangle are considered.
    /// </remarks>
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

    /// <inheritdoc cref="Enclose(ReadOnlySpan{SDL_FPoint},SDL_FRect?)"/>
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

    /// <summary>
    /// Calculates the union of two rectangles.
    /// </summary>
    /// <param name="a">
    /// The first rectangle.
    /// </param>
    /// <param name="b">
    /// The second rectangle.
    /// </param>
    /// <returns>
    /// The union of the first and second rectangle.
    /// </returns>
    public static unsafe SDL_FRect Union(
        this SDL_FRect a,
        SDL_FRect b
    )
    {
        SDL_FRect result;
        SDL_GetRectUnionFloat(&a, &b, &result)
            .AssertSdlSuccess();

        return result;
    }

    /// <inheritdoc cref="Union(SDL_FRect,SDL_FRect)"/>
    public static unsafe SDL_Rect? Union(
        this SDL_Rect a,
        SDL_Rect b
    )
    {
        SDL_Rect result;
        SDL_GetRectUnion(&a, &b, &result)
            .AssertSdlSuccess();

        return result;
    }

    /// <summary>
    /// Calculates the intersection of two rectangles.
    /// </summary>
    /// <param name="a">
    /// The first rectangle.
    /// </param>
    /// <param name="b">
    /// The second rectangle.
    /// </param>
    /// <returns>
    /// The intersection rectangle of rectangles A and B, or null if the two input rectangles
    /// do not intersect.
    /// </returns>
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

    /// <inheritdoc cref="Intersect(SDL.SDL_FRect,SDL.SDL_FRect)"/>
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

    /// <summary>
    /// Determines whether two rectangles intersect.
    /// </summary>
    /// <param name="a">
    /// The first rectangle.
    /// </param>
    /// <param name="b">
    /// The second rectangle.
    /// </param>
    /// <returns>
    /// True if there is an intersection, false otherwise.
    /// </returns>
    public static unsafe bool Intersects(
        this SDL_FRect a,
        SDL_FRect b
    )
    {
        return SDL_HasRectIntersectionFloat(&a, &b);
    }

    /// <inheritdoc cref="Intersects(SDL_FRect,SDL_FRect)"/>
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
        this SDL_Rect rect,
        Span<SDL_Point> buffer
    )
    {
        buffer[0] = new SDL_Point { x = rect.x, y = rect.y };
        buffer[1] = new SDL_Point { x = rect.x + rect.w, y = rect.y };
        buffer[2] = new SDL_Point { x = rect.x + rect.w, y = rect.y + rect.h };
        buffer[3] = new SDL_Point { x = rect.x, y = rect.y + rect.h };
    }
}