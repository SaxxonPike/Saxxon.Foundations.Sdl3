using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Camera"/> and
/// <see cref="SDL_CameraID"/>.
/// </summary>
[PublicAPI]
public static class Camera
{
    public static unsafe (IntPtr<SDL_Surface> Surface, TimeSpan Duration)? AcquireFrame(
        this IntPtr<SDL_Camera> camera
    )
    {
        var duration = 0UL;
        var result = (IntPtr<SDL_Surface>)SDL_AcquireCameraFrame(camera, &duration);
        if (result.IsNull)
            return null;

        return (result, Time.GetFromNanoseconds(duration));
    }

    public static unsafe void Close(
        this IntPtr<SDL_Camera> camera
    )
    {
        SDL_CloseCamera(camera);
    }

    public static unsafe IMemoryOwner<SDL_CameraID> GetAll()
    {
        int count;
        var cameras = SDL_GetCameras(&count);
        return SdlMemoryManager.Owned(cameras, count);
    }

    public static string? GetCurrentDriver()
    {
        return SDL_GetCurrentCameraDriver();
    }

    public static string? GetDriver(
        int index
    )
    {
        return SDL_GetCameraDriver(index);
    }

    public static unsafe SDL_CameraSpec GetFormat(
        this IntPtr<SDL_Camera> camera
    )
    {
        SDL_CameraSpec spec;
        SDL_GetCameraFormat(camera, &spec)
            .AssertSdlSuccess();
        return spec;
    }

    public static unsafe SDL_CameraID GetId(
        this IntPtr<SDL_Camera> camera
    )
    {
        var result = SDL_GetCameraID(camera);
        return result == 0 ? throw new SdlException() : result;
    }

    public static string GetName(
        this SDL_CameraID id
    )
    {
        return SDL_GetCameraName(id) ?? throw new SdlException();
    }

    public static int GetNumDrivers()
    {
        return SDL_GetNumCameraDrivers();
    }

    public static unsafe SDL_CameraPermissionState GetPermissionState(
        this IntPtr<SDL_Camera> camera
    )
    {
        var result = SDL_GetCameraPermissionState(camera);
        return (int)result == -1 ? throw new SdlException() : result;
    }

    public static SDL_CameraPosition GetPosition(
        this SDL_CameraID id
    )
    {
        return SDL_GetCameraPosition(id);
    }

    public static unsafe SDL_PropertiesID GetProperties(
        this IntPtr<SDL_Camera> camera
    )
    {
        return SDL_GetCameraProperties(camera);
    }

    public static unsafe IMemoryOwner<IntPtr<SDL_CameraSpec>> GetSupportedFormats(
        this SDL_CameraID camera
    )
    {
        int count;
        var formats = SDL_GetCameraSupportedFormats(camera, &count);
        return SdlMemoryManager.Owned(formats, count);
    }

    public static unsafe IntPtr<SDL_Camera> Open(
        this SDL_CameraID id,
        SDL_CameraSpec? desiredFormat
    )
    {
        return ((IntPtr<SDL_Camera>)SDL_OpenCamera(id, desiredFormat is { } df ? &df : null))
            .AssertSdlNotNull();
    }

    public static unsafe void ReleaseFrame(
        this IntPtr<SDL_Camera> camera,
        IntPtr<SDL_Surface> frame
    )
    {
        SDL_ReleaseCameraFrame(camera, frame);
    }
}