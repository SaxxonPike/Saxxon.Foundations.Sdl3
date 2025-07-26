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
    public static int MajorVersion =>
        SDL_TTF_MAJOR_VERSION;

    public static int Version =>
        SDL_TTF_VERSION;

    public static bool VersionAtLeast(int x, int y, int z) =>
        SDL_IMAGE_VERSION_ATLEAST(x, y, z);

    public static unsafe (int Major, int Minor, int Patch) GetFreeTypeVersion()
    {
        int major, minor, patch;
        TTF_GetFreeTypeVersion(&major, &minor, &patch);
        return (major, minor, patch);
    }

    public static unsafe (int Major, int Minor, int Patch) GetHarfBuzzVersion()
    {
        int major, minor, patch;
        TTF_GetHarfBuzzVersion(&major, &minor, &patch);
        return (major, minor, patch);
    }

    public static void Init()
    {
        TTF_Init().AssertSdlSuccess();
    }

    public static uint StringToTag(ReadOnlySpan<char> @string)
    {
        using var stringStr = new Utf8Span(@string);
        return TTF_StringToTag(stringStr);
    }

    public static unsafe string TagToString(uint tag)
    {
        Span<byte> bytes = stackalloc byte[Encoding.UTF8.GetMaxByteCount(4)];
        fixed (byte* ptr = bytes)
            TTF_TagToString(tag, ptr, (UIntPtr)bytes.Length);
        return Encoding.UTF8.GetString(bytes);
    }

    public static void Quit()
    {
        TTF_Quit();
    }

    public static bool WasInit()
    {
        return TTF_WasInit() != 0;
    }
}