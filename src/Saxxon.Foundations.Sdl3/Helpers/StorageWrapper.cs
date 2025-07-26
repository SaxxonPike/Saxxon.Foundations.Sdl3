using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Helpers;

[PublicAPI]
public sealed unsafe class StorageWrapper : IDisposable
{
    public IntPtr UserData { get; }
    public IntPtr<SDL_Storage> SdlStorage { get; }
    public Stream BaseStream { get; }

    public void Dispose()
    {
    }
}