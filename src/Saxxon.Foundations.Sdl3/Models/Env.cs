using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

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

    public static unsafe List<string> GetVariables(
        this IntPtr<SDL_Environment> env
    )
    {
        var vars = Ptr.FromArray(SDL_GetEnvironmentVariables(env));
        if (vars.IsNull)
            throw new SdlException();

        var count = 0;
        var result = new List<string>();

        while (vars[count] != IntPtr.Zero)
        {
            if (vars[count].GetString() is { } value)
                result.Add(value);

            count++;
        }

        return result;
    }

    public static unsafe string? GetVariable(
        this IntPtr<SDL_Environment> env,
        ReadOnlySpan<char> name
    )
    {
        var nameLen = name.MeasureUtf8();
        Span<byte> nameBytes = stackalloc byte[nameLen];
        name.EncodeUtf8(nameBytes);

        fixed (byte* namePtr = nameBytes)
            return ((IntPtr)Unsafe_SDL_GetEnvironmentVariable(env, namePtr)).GetString();
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