using System.Runtime.InteropServices;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

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
        var nameStr = new Utf8Span(name);
        return SDL_UnsetEnvironmentVariable(env, nameStr);
    }

    public static unsafe bool SetVariable(
        this IntPtr<SDL_Environment> env,
        ReadOnlySpan<char> name,
        ReadOnlySpan<char> value,
        bool overwrite = true
    )
    {
        var nameStr = new Utf8Span(name);
        var valueStr = new Utf8Span(value);
        return SDL_SetEnvironmentVariable(env, nameStr, valueStr, overwrite);
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
            if (Marshal.PtrToStringUTF8(vars[count]) is { } value)
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
        var nameStr = new Utf8Span(name);
        return Marshal.PtrToStringUTF8((IntPtr)Unsafe_SDL_GetEnvironmentVariable(env, nameStr.Ptr));
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