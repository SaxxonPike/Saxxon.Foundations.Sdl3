using System.Buffers;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Sensor"/>.
/// </summary>
[PublicAPI]
public static class Sensor
{
    public static unsafe void Close(this IntPtr<SDL_Sensor> sensor)
    {
        SDL_CloseSensor(sensor);
    }

    public static unsafe void GetData(
        this IntPtr<SDL_Sensor> sensor,
        Span<float> data
    )
    {
        fixed (float* dataPtr = data)
            SDL_GetSensorData(sensor, dataPtr, data.Length)
                .AssertSdlSuccess();
    }

    public static unsafe SDL_SensorID GetId(this IntPtr<SDL_Sensor> sensor)
    {
        return SDL_GetSensorID(sensor);
    }

    public static unsafe int GetNonPortableType(this IntPtr<SDL_Sensor> sensor)
    {
        return SDL_GetSensorNonPortableType(sensor);
    }

    public static unsafe SDL_SensorType GetSensorType(this IntPtr<SDL_Sensor> sensor)
    {
        return SDL_GetSensorType(sensor);
    }

    public static unsafe string? GetName(this IntPtr<SDL_Sensor> sensor)
    {
        return Ptr.ToUtf8String(Unsafe_SDL_GetSensorName(sensor));
    }

    public static unsafe string? GetName(this SDL_SensorID sensorId)
    {
        return Ptr.ToUtf8String(Unsafe_SDL_GetSensorNameForID(sensorId));
    }

    public static unsafe SDL_PropertiesID GetProperties(this IntPtr<SDL_Sensor> sensor)
    {
        return SDL_GetSensorProperties(sensor);
    }

    public static unsafe IntPtr<SDL_Sensor> Open(this SDL_SensorID sensorId)
    {
        return SDL_OpenSensor(sensorId);
    }

    [MustDisposeResource]
    public static unsafe IMemoryOwner<SDL_SensorID> GetAll()
    {
        int count;
        var sensors = SDL_GetSensors(&count);
        return SdlMemoryManager.Owned(sensors, count);
    }
}