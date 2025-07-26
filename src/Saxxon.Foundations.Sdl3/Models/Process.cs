using System.Buffers;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

public static class Process
{
    public static unsafe void Destroy(
        this IntPtr<SDL_Process> process
    )
    {
        SDL_DestroyProcess(process);
    }

    public static unsafe int Wait(
        this IntPtr<SDL_Process> process,
        bool block
    )
    {
        int exitCode;
        SDL_WaitProcess(process, block, &exitCode)
            .AssertSdlSuccess();
        return exitCode;
    }

    public static unsafe void Kill(
        this IntPtr<SDL_Process> process,
        bool force
    )
    {
        SDL_KillProcess(process, force)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<SDL_IOStream> GetOutput(
        this IntPtr<SDL_Process> process
    )
    {
        return ((IntPtr<SDL_IOStream>)SDL_GetProcessOutput(process))
            .AssertSdlNotNull();
    }
    
    public static unsafe IntPtr<SDL_IOStream> GetInput(
        this IntPtr<SDL_Process> process
    )
    {
        return ((IntPtr<SDL_IOStream>)SDL_GetProcessInput(process))
            .AssertSdlNotNull();
    }

    public static unsafe (IMemoryOwner<byte> Output, int ExitCode) Read(
        this IntPtr<SDL_Process> process
    )
    {
        UIntPtr dataSize;
        int exitCode;
        var result = SDL_ReadProcess(process, &dataSize, &exitCode)
            .AssertSdlNotNull();
        return (SdlMemoryManager.Owned((byte*)result, (int)dataSize), exitCode);
    }

    public static unsafe SDL_PropertiesID GetProperties(
        this IntPtr<SDL_Process> process
    )
    {
        var result = SDL_GetProcessProperties(process);
        if (result == 0)
            throw new SdlException();
        return result;
    }

    public static unsafe IntPtr<SDL_Process> CreateWithProperties(
        SDL_PropertiesID props
    )
    {
        return ((IntPtr<SDL_Process>)SDL_CreateProcessWithProperties(props))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Process> Create(
        bool pipeStdIo,
        params string[] args
    )
    {
        using var bs = new Utf8ByteStrings(args);
        return ((IntPtr<SDL_Process>)SDL_CreateProcess((byte**)bs.Ptr, pipeStdIo))
            .AssertSdlNotNull();
    }
}