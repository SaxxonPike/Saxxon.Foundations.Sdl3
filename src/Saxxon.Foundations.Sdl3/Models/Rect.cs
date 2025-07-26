using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

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
}