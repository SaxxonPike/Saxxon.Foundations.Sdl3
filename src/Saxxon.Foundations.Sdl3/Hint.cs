using System.Runtime.InteropServices.Marshalling;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for SDL runtime hints.
/// </summary>
[PublicAPI]
public static class Hint
{
    /// <summary>
    /// Sets a hint with normal priority.
    /// </summary>
    public static void Set(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
    {
        using var nameStr = new UnmanagedString(name);
        using var valueStr = new UnmanagedString(value);
        SDL_SetHint(nameStr, valueStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets a hint with normal priority.
    /// </summary>
    public static void Set(ReadOnlySpan<byte> name, ReadOnlySpan<char> value)
    {
        using var valueStr = new UnmanagedString(value);
        SDL_SetHint(name, valueStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets a hint with a specific priority.
    /// </summary>
    public static void SetWithPriority(ReadOnlySpan<char> name, ReadOnlySpan<char> value, SDL_HintPriority priority)
    {
        using var nameStr = new UnmanagedString(name);
        using var valueStr = new UnmanagedString(value);
        SDL_SetHintWithPriority(nameStr, valueStr, priority)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets a hint with a specific priority.
    /// </summary>
    public static void SetWithPriority(ReadOnlySpan<byte> name, ReadOnlySpan<char> value, SDL_HintPriority priority)
    {
        using var valueStr = new UnmanagedString(value);
        SDL_SetHintWithPriority(name, valueStr, priority)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Gets the value of a hint.
    /// </summary>
    public static unsafe string? Get(ReadOnlySpan<char> name)
    {
        using var nameStr = new UnmanagedString(name);
        return Utf8StringMarshaller.ConvertToManaged(Unsafe_SDL_GetHint(nameStr.Ptr));
    }

    /// <summary>
    /// Gets the value of a hint.
    /// </summary>
    public static string? Get(ReadOnlySpan<byte> name)
    {
        return SDL_GetHint(name);
    }

    /// <summary>
    /// Resets a hint to the default value.
    /// </summary>
    public static void Reset(ReadOnlySpan<char> name)
    {
        using var nameStr = new UnmanagedString(name);
        SDL_ResetHint(nameStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Resets a hint to the default value.
    /// </summary>
    public static void Reset(ReadOnlySpan<byte> name)
    {
        SDL_ResetHint(name)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Reset all hints to the default values.
    /// </summary>
    public static void ResetAll()
    {
        SDL_ResetHints();
    }

    /// <summary>
    /// Adds a function to watch a particular hint.
    /// </summary>
    /// <param name="name">
    /// Hint to watch.
    /// </param>
    /// <param name="func">
    /// Function to call when the hint changes.
    /// </param>
    /// <remarks>
    /// The callback function is called immediately to provide it an initial
    /// value, and again each time the hint's value changes.
    /// </remarks>
    public static unsafe void AddCallback(
        ReadOnlySpan<char> name,
        HintCallbackFunction func
    )
    {
        using var nameStr = new UnmanagedString(name);
        SDL_AddHintCallback(nameStr, HintCallbackFunction.Callback, func.UserData)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Removes a function watching a particular hint.
    /// </summary>
    /// <param name="name">
    /// Hint being watched.
    /// </param>
    /// <param name="func">
    /// Function set up to watch the hint.
    /// </param>
    public static unsafe void RemoveCallback(
        ReadOnlySpan<char> name,
        HintCallbackFunction func
    )
    {
        using var nameStr = new UnmanagedString(name);
        SDL_RemoveHintCallback(nameStr, HintCallbackFunction.Callback, func.UserData);
    }
}