using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an interface for uncategorized SDL_image functions.
/// </summary>
[PublicAPI]
public static class ImageLib
{
    /// <summary>
    /// Gets the version of the dynamically linked SDL_image library.
    /// </summary>
    public static int GetVersion()
    {
        return IMG_Version();
    }
}