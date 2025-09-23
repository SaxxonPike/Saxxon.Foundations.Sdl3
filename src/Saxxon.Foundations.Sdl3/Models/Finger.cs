using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Finger"/>.
/// </summary>
[PublicAPI]
public static class Finger
{
    /// <summary>
    /// Gets the X-coordinate of finger data.
    /// </summary>
    /// <param name="finger">
    /// Finger to query.
    /// </param>
    public static float GetX(this IntPtr<SDL_Finger> finger) =>
        finger.AsReadOnlyRef().x;

    /// <summary>
    /// Gets the Y-coordinate of finger data.
    /// </summary>
    /// <param name="finger">
    /// Finger to query.
    /// </param>
    public static float GetY(this IntPtr<SDL_Finger> finger) =>
        finger.AsReadOnlyRef().y;

    /// <summary>
    /// Gets the pressure amount of finger data.
    /// </summary>
    /// <param name="finger">
    /// Finger to query.
    /// </param>
    public static float GetPressure(this IntPtr<SDL_Finger> finger) =>
        finger.AsReadOnlyRef().pressure;

    /// <summary>
    /// Gets the ID of the finger data.
    /// </summary>
    /// <param name="finger">
    /// Finger to query.
    /// </param>
    public static SDL_FingerID GetId(this IntPtr<SDL_Finger> finger) =>
        finger.AsReadOnlyRef().id;

    /// <summary>
    /// Gets the coordinates of finger data.
    /// </summary>
    /// <param name="finger">
    /// Finger to query.
    /// </param>
    public static SDL_FPoint GetPoint(this IntPtr<SDL_Finger> finger)
    {
        var obj = finger.AsReadOnlyRef();
        return Point.CreateFloat(obj.x, obj.y);
    }
}