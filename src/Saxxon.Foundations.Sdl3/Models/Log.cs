using System.Diagnostics;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for the SDL logger.
/// </summary>
[PublicAPI]
public static class Log
{
    public const SDL_LogCategory AppCategory = SDL_LogCategory.SDL_LOG_CATEGORY_APPLICATION;

    public static SDL_LogPriority GetPriority(SDL_LogCategory category)
    {
        return SDL_GetLogPriority(category);
    }

    public static void Critical(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_CRITICAL, exception);
    }

    [Conditional("DEBUG")]
    [StringFormatMethod(nameof(format))]
    public static void Debug(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_DEBUG, format, args);
    }

    [Conditional("DEBUG")]
    public static void Debug(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_DEBUG, exception);
    }

    [StringFormatMethod(nameof(format))]
    public static void Error(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_ERROR, format, args);
    }

    public static void Error(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_ERROR, exception);
    }

    [StringFormatMethod(nameof(format))]
    public static void Info(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_INFO, format, args);
    }

    public static void Info(
        SDL_LogCategory category,
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_INFO, exception);
    }

    public static unsafe void Exception(
        SDL_LogCategory category,
        SDL_LogPriority priority,
        Exception exception
    )
    {
        using var sbStr0 = new Utf8Span("An exception occurred.");
        using var sbStr1 = new Utf8Span(exception.ToString());
        SDL_LogMessageV((int)category, priority, sbStr0, null);
        SDL_LogMessageV((int)category, priority, sbStr1, null);
    }

    [StringFormatMethod(nameof(format))]
    public static unsafe void Message(
        SDL_LogCategory category,
        SDL_LogPriority priority,
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        using var sbStr = Utf8Span.Format(format, args);
        SDL_LogMessageV((int)category, priority, sbStr, null);
    }

    public static void SetPriorities(SDL_LogPriority priority)
    {
        SDL_SetLogPriorities(priority);
    }

    public static void SetPriority(SDL_LogCategory category, SDL_LogPriority priority)
    {
        SDL_SetLogPriority((int)category, priority);
    }

    public static void SetPriorityPrefix(SDL_LogPriority priority, ReadOnlySpan<char> prefix)
    {
        using var prefixStr = new Utf8Span(prefix);
        SDL_SetLogPriorityPrefix(priority, prefixStr);
    }

    public static void ResetPriorities()
    {
        SDL_ResetLogPriorities();
    }

    [Conditional("DEBUG")]
    [StringFormatMethod(nameof(format))]
    public static void Trace(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_TRACE, format, args);
    }

    [Conditional("DEBUG")]
    public static void Trace(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_TRACE, exception);
    }

    [StringFormatMethod(nameof(format))]
    public static void Verbose(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_VERBOSE, format, args);
    }

    public static void Verbose(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_VERBOSE, exception);
    }

    [StringFormatMethod(nameof(format))]
    public static void Warn(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_WARN, format, args);
    }

    public static void Warn(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_WARN, exception);
    }
}