using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Extensions;

[PublicAPI]
public static class GeometryExtensions
{
    public static Vector4 ToVector4(this SDL_FRect rect)
    {
        return Unsafe.As<SDL_FRect, Vector4>(ref rect);
    }

    public static Vector4 ToVector4(this SDL_Rect rect)
    {
        return Vector128.ConvertToSingle(
            Vector128.Create(rect.x, rect.y, rect.w, rect.h)
        ).AsVector4();
    }

    public static bool IsEqualTo(this SDL_FRect a, SDL_FRect b)
    {
        return Unsafe.As<SDL_FRect, Vector128<float>>(ref a) == Unsafe.As<SDL_FRect, Vector128<float>>(ref b);
    }

    public static bool IsEqualTo(this SDL_Rect a, SDL_Rect b)
    {
        return Unsafe.As<SDL_Rect, Vector128<int>>(ref a) == Unsafe.As<SDL_Rect, Vector128<int>>(ref b);
    }

    public static SDL_FRect ToFRect(this Vector4 vec4)
    {
        return Unsafe.As<Vector4, SDL_FRect>(ref vec4);
    }

    public static SDL_Rect ToRect(this Vector4 vec4)
    {
        var val = Vector128.ConvertToInt32(
            vec4.AsVector128()
        );
        return Unsafe.As<Vector128<int>, SDL_Rect>(ref val);
    }

    public static Vector2 ToVector2(this SDL_FPoint rect)
    {
        return Unsafe.As<SDL_FPoint, Vector2>(ref rect);
    }

    public static Vector2 ToVector2(this SDL_Point rect)
    {
        var val = Vector64.ConvertToSingle(
            Vector64.Create(rect.x, rect.y)
        );
        return Unsafe.As<Vector64<float>, Vector2>(ref val);
    }

    public static bool IsEqualTo(this SDL_FPoint a, SDL_FPoint b)
    {
        return Unsafe.As<SDL_FPoint, Vector64<float>>(ref a) == Unsafe.As<SDL_FPoint, Vector64<float>>(ref b);
    }

    public static bool IsEqualTo(this SDL_Point a, SDL_Point b)
    {
        return Unsafe.As<SDL_Point, Vector64<int>>(ref a) == Unsafe.As<SDL_Point, Vector64<int>>(ref b);
    }

    public static SDL_FPoint ToFPoint(this Vector2 vec2)
    {
        return Unsafe.As<Vector2, SDL_FPoint>(ref vec2);
    }

    public static SDL_Point ToPoint(this Vector2 vec2)
    {
        var val = Vector128.ConvertToInt32(
            vec2.AsVector128()
        );
        return Unsafe.As<Vector128<int>, SDL_Point>(ref val);
    }
}