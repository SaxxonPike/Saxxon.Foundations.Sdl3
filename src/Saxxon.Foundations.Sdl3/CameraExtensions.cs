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
public static class CameraExtensions
{
    extension(SDL_CameraID)
    {
        public static unsafe IMemoryOwner<SDL_CameraID> GetAll()
        {
            int count;
            var cameras = SDL_GetCameras(&count);
            return SdlMemoryManager.Owned(cameras, count);
        }
    }

    extension(SDL_CameraID id)
    {
        public string Name =>
            SDL_GetCameraName(id) ?? throw new SdlException();

        public SDL_CameraPosition Position =>
            SDL_GetCameraPosition(id);

        public unsafe IMemoryOwner<IntPtr<SDL_CameraSpec>> GetSupportedFormats()
        {
            int count;
            var formats = SDL_GetCameraSupportedFormats(id, &count);
            return SdlMemoryManager.Owned(formats, count);
        }

        public unsafe IntPtr<SDL_Camera> Open(
            SDL_CameraSpec? desiredFormat
        ) => ((IntPtr<SDL_Camera>)SDL_OpenCamera(id, desiredFormat is { } df ? &df : null))
            .AssertSdlNotNull();
    }

    extension(IntPtr<SDL_Camera> camera)
    {
        public string Name => 
            camera.Id.Name;

        public SDL_CameraPosition Position => 
            camera.Id.Position;

        public IMemoryOwner<IntPtr<SDL_CameraSpec>> GetSupportedFormats() =>
            camera.Id.GetSupportedFormats();
        
        public unsafe (IntPtr<SDL_Surface> Surface, TimeSpan Duration)? AcquireFrame()
        {
            var duration = 0UL;
            var result = (IntPtr<SDL_Surface>)SDL_AcquireCameraFrame(camera, &duration);
            if (result.IsNull)
                return null;

            return (result, Time.GetFromNanoseconds(duration));
        }

        public unsafe void Close() =>
            SDL_CloseCamera(camera);

        public unsafe SDL_CameraSpec Format
        {
            get
            {
                SDL_CameraSpec spec;
                SDL_GetCameraFormat(camera, &spec)
                    .AssertSdlSuccess();
                return spec;
            }
        }

        public unsafe SDL_CameraID Id
        {
            get
            {
                var result = SDL_GetCameraID(camera);
                return result == 0 ? throw new SdlException() : result;
            }
        }

        public unsafe SDL_CameraPermissionState PermissionState
        {
            get
            {
                var result = SDL_GetCameraPermissionState(camera);
                return (int)result == -1 ? throw new SdlException() : result;
            }
        }

        public unsafe SDL_PropertiesID Properties =>
            SDL_GetCameraProperties(camera);

        public unsafe void ReleaseFrame(
            IntPtr<SDL_Surface> frame
        )
        {
            SDL_ReleaseCameraFrame(camera, frame);
        }
    }
}