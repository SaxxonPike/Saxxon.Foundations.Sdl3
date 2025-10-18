using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for SDL camera drivers.
/// </summary>
[PublicAPI]
public static class CameraDriverExtensions
{
    /// <summary>
    /// Extensions for <see cref="SDL_CameraDriver"/>.
    /// </summary>
    extension(SDL_CameraDriver)
    {
        public static string? Current => 
            SDL_GetCurrentCameraDriver();

        public static string? Get(
            int index
        ) => SDL_GetCameraDriver(index);
        
        public static int Count => 
            SDL_GetNumCameraDrivers();
    }
}