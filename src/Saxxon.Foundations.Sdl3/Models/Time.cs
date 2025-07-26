using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an interface for querying time in SDL. Also includes nanosecond
/// conversion functions and extensions for <see cref="TimeSpan"/>.
/// </summary>
[PublicAPI]
public static class Time
{
    /// <summary>
    /// Gets the number of milliseconds elapsed since app initialization.
    /// </summary>
    public static ulong GetNowMilliseconds()
    {
        return SDL_GetTicks();
    }

    /// <summary>
    /// Gets the number of nanoseconds elapsed since app initialization.
    /// </summary>
    public static ulong GetNowNanoseconds()
    {
        return SDL_GetTicksNS();
    }

    /// <summary>
    /// Gets the span of time elapsed since app initialization.
    /// </summary>
    public static TimeSpan GetNow()
    {
        return GetFromNanoseconds(SDL_GetTicksNS());
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> value to nanoseconds.
    /// </summary>
    public static ulong ToNanoseconds(this TimeSpan timeSpan)
    {
        return unchecked((ulong)timeSpan.Ticks * TimeSpan.NanosecondsPerTick);
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> value to milliseconds.
    /// </summary>
    public static int ToMilliseconds(this TimeSpan timeSpan)
    {
        return unchecked((int)(timeSpan.Ticks / TimeSpan.TicksPerMillisecond));
    }

    public static TimeSpan GetFromNanoseconds(ulong nanoseconds)
    {
        return new TimeSpan(unchecked((long)(nanoseconds / TimeSpan.NanosecondsPerTick)));
    }

    public static TimeSpan GetFromNanoseconds(long nanoseconds)
    {
        return new TimeSpan(nanoseconds / TimeSpan.NanosecondsPerTick);
    }

    public static TimeSpan AddNanoseconds(this TimeSpan timeSpan, ulong nanoseconds)
    {
        var diff = unchecked((long)(nanoseconds / TimeSpan.NanosecondsPerTick));
        return timeSpan.Add(new TimeSpan(diff));
    }

    public static TimeSpan AddNanoseconds(this TimeSpan timeSpan, long nanoseconds)
    {
        var diff = nanoseconds / TimeSpan.NanosecondsPerTick;
        return timeSpan.Add(new TimeSpan(diff));
    }

    public static void Delay(TimeSpan timeSpan)
    {
        SDL_DelayNS(timeSpan.ToNanoseconds());
    }

    public static void DelayPrecise(TimeSpan timeSpan)
    {
        SDL_DelayPrecise(timeSpan.ToNanoseconds());
    }

    public static void DelayMilliseconds(uint milliseconds)
    {
        SDL_Delay(milliseconds);
    }

    public static void DelayNanoseconds(ulong nanoseconds)
    {
        SDL_DelayNS(nanoseconds);
    }
    
    public static void DelayNanosecondsPrecise(ulong nanoseconds)
    {
        SDL_DelayPrecise(nanoseconds);
    }
}