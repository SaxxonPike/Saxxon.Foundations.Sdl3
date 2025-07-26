using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;
using Saxxon.Foundations.Sdl3.Models;

namespace Saxxon.Foundations.Sdl3.Helpers;

[PublicAPI]
[MustDisposeResource]
public sealed unsafe class IoStreamWrapper : IDisposable
{
    public IntPtr UserData { get; }
    public IntPtr<SDL_IOStream> SdlIoStream { get; }
    public Stream BaseStream { get; }

    public IoStreamWrapper(
        Stream stream
    )
    {
        var io = IoStream.InitInterface();

        io.close = &IngressClose;
        io.flush = &IngressFlush;
        io.read = &IngressRead;
        io.seek = &IngressSeek;
        io.size = &IngressSize;
        io.write = &IngressWrite;

        BaseStream = stream;
        UserData = UserDataStore.Add(stream);
        SdlIoStream = IoStream.Open(io, UserData);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static long IngressSize(
        IntPtr userdata)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            return stream.Length;
        }
        catch
        {
            return -1;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static long IngressSeek(
        IntPtr userdata,
        long offset,
        SDL_IOWhence whence)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            return stream.Seek(offset, whence switch
            {
                SDL_IOWhence.SDL_IO_SEEK_SET => SeekOrigin.Begin,
                SDL_IOWhence.SDL_IO_SEEK_CUR => SeekOrigin.Current,
                SDL_IOWhence.SDL_IO_SEEK_END => SeekOrigin.End,
                _ => throw new ArgumentOutOfRangeException(nameof(whence), whence, null)
            });
        }
        catch
        {
            return -1;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static UIntPtr IngressRead(
        IntPtr userdata,
        IntPtr ptr,
        UIntPtr size,
        SDL_IOStatus* status)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            if (!stream.CanRead)
            {
                *status = SDL_IOStatus.SDL_IO_STATUS_WRITEONLY;
                return 0;
            }

            if (stream.CanSeek && stream.Position >= stream.Length)
            {
                *status = SDL_IOStatus.SDL_IO_STATUS_EOF;
                return 0;
            }

            var amount = (UIntPtr)stream.Read(
                new Span<byte>((void*)ptr, (int)size)
            );

            return amount;
        }
        catch
        {
            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            return 0;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static UIntPtr IngressWrite(
        IntPtr userdata,
        IntPtr ptr,
        UIntPtr size,
        SDL_IOStatus* status)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            if (!stream.CanWrite)
            {
                *status = SDL_IOStatus.SDL_IO_STATUS_READONLY;
                return 0;
            }

            if (stream.CanSeek && stream.Position >= stream.Length)
            {
                *status = SDL_IOStatus.SDL_IO_STATUS_EOF;
                return 0;
            }

            stream.Write(
                new Span<byte>((void*)ptr, (int)size)
            );

            return size;
        }
        catch
        {
            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            return 0;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool IngressFlush(
        IntPtr userdata,
        SDL_IOStatus* status)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            stream.Flush();
            return true;
        }
        catch
        {
            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            return false;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDLBool IngressClose(
        IntPtr userdata)
    {
        try
        {
            var stream = UserDataStore.Get<Stream>(userdata)!;

            stream.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        UserDataStore.Remove(UserData);
    }
}