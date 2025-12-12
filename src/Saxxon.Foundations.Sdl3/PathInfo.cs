using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Extensions for use with <see cref="SDL_PathInfo"/>.
/// </summary>
[PublicAPI]
public static class PathInfo
{
    /// <summary>
    /// Convert to nanoseconds-since-epoch.
    /// </summary>
    private static SDL_Time ConvertToSdlTime(DateTime dto)
    {
        var diff = dto - DateTime.UnixEpoch;
        if (diff < TimeSpan.Zero)
            return 0;

        return (SDL_Time)diff.ToNanoseconds();
    }

    /// <summary>
    /// Convert from nanoseconds-since-epoch.
    /// </summary>
    private static DateTime ConvertFromSdlTime(SDL_Time time) =>
        DateTime.UnixEpoch + Time.GetFromNanoseconds(unchecked((ulong)time));

    /// <summary>
    /// Extensions for <see cref="SDL_PathInfo"/>.
    /// </summary>
    extension(SDL_PathInfo info)
    {
        [PublicAPI]
        public DateTime AccessTimeUtc
        {
            get => ConvertFromSdlTime(info.access_time);
            set => info.access_time = ConvertToSdlTime(value);
        }

        [PublicAPI]
        public DateTime CreateTimeUtc
        {
            get => ConvertFromSdlTime(info.create_time);
            set => info.create_time = ConvertToSdlTime(value);
        }

        [PublicAPI]
        public DateTime ModifyTimeUtc
        {
            get => ConvertFromSdlTime(info.modify_time);
            set => info.modify_time = ConvertToSdlTime(value);
        }
    }
}