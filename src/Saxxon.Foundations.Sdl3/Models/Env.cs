using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for interacting with environment
/// variables.
/// </summary>
[PublicAPI]
public static class Env
{
    public static unsafe void Destroy(
        this IntPtr<SDL_Environment> env
    )
    {
        SDL_DestroyEnvironment(env);
    }

    public static unsafe bool UnsetVariable(
        this IntPtr<SDL_Environment> env,
        ReadOnlySpan<char> name
    )
    {
        var nameLen = name.MeasureUtf8();
        Span<byte> nameBytes = stackalloc byte[nameLen];
        name.EncodeUtf8(nameBytes);

        fixed (byte* namePtr = nameBytes)
            return SDL_UnsetEnvironmentVariable(env, namePtr);
    }

    public static unsafe bool SetVariable(
        this IntPtr<SDL_Environment> env,
        ReadOnlySpan<char> name,
        ReadOnlySpan<char> value,
        bool overwrite = true
    )
    {
        var nameLen = name.MeasureUtf8();
        var valueLen = value.MeasureUtf8();
        Span<byte> nameBytes = stackalloc byte[nameLen];
        Span<byte> valueBytes = stackalloc byte[valueLen];
        name.EncodeUtf8(nameBytes);
        value.EncodeUtf8(valueBytes);

        fixed (byte* namePtr = nameBytes)
        fixed (byte* valuePtr = valueBytes)
            return SDL_SetEnvironmentVariable(env, namePtr, valuePtr, overwrite);
    }

    /// <summary>
    /// Gets all variables in the environment.
    /// </summary>
    /// <param name="env">
    /// The environment to query.
    /// </param>
    /// <returns>
    /// List of environment variables in the form "variable=value".
    /// </returns>
    public static unsafe List<string> GetVariables(
        this IntPtr<SDL_Environment> env
    )
    {
        var vars = ((IntPtr<IntPtr<byte>>)SDL_GetEnvironmentVariables(env))
            .AssertSdlNotNull();

        var result = new List<string>();
        
        var count = 0;
        while (vars[count] != IntPtr.Zero)
        {
            var str = vars[count].GetString();
            if (str != null)
                result.Add(str);
            count++;
        }
        
        return result;
    }

    public static unsafe string? GetVariable(
        this IntPtr<SDL_Environment> env,
        ReadOnlySpan<char> name
    )
    {
        using var nameStr = new UnmanagedString(name);
        return SDL_GetEnvironmentVariable(env, nameStr);
    }

    public static unsafe IntPtr<SDL_Environment> Create(bool populated)
    {
        return ((IntPtr<SDL_Environment>)SDL_CreateEnvironment(populated))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Environment> Get()
    {
        return SDL_GetEnvironment();
    }
}