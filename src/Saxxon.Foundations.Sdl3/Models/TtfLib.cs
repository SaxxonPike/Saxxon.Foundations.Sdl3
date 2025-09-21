using System.Text;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an interface for uncategorized SDL_ttf functions.
/// </summary>
[PublicAPI]
public static class TtfLib
{
    /// <summary>
    /// Gets the major component of the SDL_ttf version.
    /// </summary>
    public static int MajorVersion =>
        SDL_TTF_MAJOR_VERSION;

    /// <summary>
    /// Gets the full value of the SDL_ttf version.
    /// </summary>
    public static int Version =>
        SDL_TTF_VERSION;

    /// <summary>
    /// Checks if the current SDL_ttf version is at least as high as the specified value.
    /// </summary>
    /// <param name="major">
    /// Major component of the version comparison.
    /// </param>
    /// <param name="minor">
    /// Minor component of the version comparison.
    /// </param>
    /// <param name="patch">
    /// Patch component of the version comparison.
    /// </param>
    /// <returns>
    /// True if the SDL_ttf version is at least as high as the specified value, false otherwise.
    /// </returns>
    public static bool VersionAtLeast(int major, int minor, int patch) =>
        SDL_IMAGE_VERSION_ATLEAST(major, minor, patch);

    /// <summary>
    /// Gets the version information of FreeType.
    /// </summary>
    public static unsafe (int Major, int Minor, int Patch) GetFreeTypeVersion()
    {
        int major, minor, patch;
        TTF_GetFreeTypeVersion(&major, &minor, &patch);
        return (major, minor, patch);
    }

    /// <summary>
    /// Gets the version information of HarfBuzz.
    /// </summary>
    public static unsafe (int Major, int Minor, int Patch) GetHarfBuzzVersion()
    {
        int major, minor, patch;
        TTF_GetHarfBuzzVersion(&major, &minor, &patch);
        return (major, minor, patch);
    }

    /// <summary>
    /// Initializes the SDL_ttf library.
    /// </summary>
    public static void Init()
    {
        TTF_Init().AssertSdlSuccess();
    }

    /// <summary>
    /// Converts from a 4 character string to a 32-bit tag.
    /// </summary>
    /// <param name="string">
    /// The 4 character string to convert.
    /// </param>
    /// <returns>
    /// The 32-bit representation of the string.
    /// </returns>
    public static uint StringToTag(ReadOnlySpan<char> @string)
    {
        using var stringStr = new UnmanagedString(@string);
        return TTF_StringToTag(stringStr);
    }

    /// <summary>
    /// Converts from a 32-bit tag to a 4 character string.
    /// </summary>
    /// <param name="tag">
    /// The 32-bit tag to convert.
    /// </param>
    /// <returns>
    /// The 4 character representation of the tag.
    /// </returns>
    public static unsafe string TagToString(uint tag)
    {
        Span<byte> bytes = stackalloc byte[Encoding.UTF8.GetMaxByteCount(4)];
        fixed (byte* ptr = bytes)
            TTF_TagToString(tag, ptr, (UIntPtr)bytes.Length);
        return bytes.GetString();
    }

    /// <summary>
    /// Deinitializes SDL_ttf.
    /// </summary>
    public static void Quit()
    {
        TTF_Quit();
    }

    /// <summary>
    /// Checks the number of times SDL_ttf has been initialized.
    /// </summary>
    /// <returns>
    /// True if SDL_ttf is currently initialized.
    /// </returns>
    /// <remarks>
    /// This is the number of times <see cref="Quit"/> needs to be called to fully unload SDL_ttf.
    /// </remarks>
    public static int WasInit()
    {
        return TTF_WasInit();
    }
}