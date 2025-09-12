using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for the SDL logger.
/// </summary>
[PublicAPI]
public static class Log
{
    /// <summary>
    /// Holds a reference to the log output function set by <see cref="SetFunction"/>.
    /// This is used to prevent the delegate from being garbage collected.
    /// </summary>
    private static LogOutputFunction? _logOutputFunction;

    /// <summary>
    /// Default category to use for application logging.
    /// </summary>
    public const SDL_LogCategory AppCategory = SDL_LogCategory.SDL_LOG_CATEGORY_APPLICATION;

    /// <summary>
    /// Get the priority of a particular log category.
    /// </summary>
    /// <param name="category">
    /// Category to query.
    /// </param>
    /// <returns>
    /// <see cref="SDL_LogPriority"/> for the requested category.
    /// </returns>
    public static SDL_LogPriority GetPriority(SDL_LogCategory category)
    {
        return SDL_GetLogPriority(category);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_CRITICAL"/>.
    /// </summary>
    /// <param name="format">
    /// String to use for formatting args.
    /// </param>
    /// <param name="args">
    /// Additional arguments for the message.
    /// </param>
    [StringFormatMethod(nameof(format))]
    public static void Critical(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_CRITICAL, format, args);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_CRITICAL"/>.
    /// </summary>
    /// <param name="exception">
    /// Exception to log.
    /// </param>
    public static void Critical(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_CRITICAL, exception);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_DEBUG"/>.
    /// </summary>
    /// <param name="format">
    /// String to use for formatting args.
    /// </param>
    /// <param name="args">
    /// Additional arguments for the message.
    /// </param>
    [Conditional("DEBUG")]
    [StringFormatMethod(nameof(format))]
    public static void Debug(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_DEBUG, format, args);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_DEBUG"/>.
    /// </summary>
    /// <param name="exception">
    /// Exception to log.
    /// </param>
    [Conditional("DEBUG")]
    public static void Debug(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_DEBUG, exception);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_ERROR"/>.
    /// </summary>
    /// <param name="format">
    /// String to use for formatting args.
    /// </param>
    /// <param name="args">
    /// Additional arguments for the message.
    /// </param>
    [StringFormatMethod(nameof(format))]
    public static void Error(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_ERROR, format, args);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_ERROR"/>.
    /// </summary>
    /// <param name="exception">
    /// Exception to log.
    /// </param>
    public static void Error(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_ERROR, exception);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_INFO"/>.
    /// </summary>
    /// <param name="format">
    /// String to use for formatting args.
    /// </param>
    /// <param name="args">
    /// Additional arguments for the message.
    /// </param>
    [StringFormatMethod(nameof(format))]
    public static void Info(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_INFO, format, args);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_INFO"/>.
    /// </summary>
    /// <param name="exception">
    /// Exception to log.
    /// </param>
    public static void Info(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_INFO, exception);
    }

    /// <summary>
    /// Logs an exception.
    /// </summary>
    /// <param name="category">
    /// Category under which to log the exception.
    /// </param>
    /// <param name="priority">
    /// Logging priority.
    /// </param>
    /// <param name="exception">
    /// Exception to log.
    /// </param>
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

    /// <summary>
    /// Logs a message.
    /// </summary>
    /// <param name="category">
    /// Category under which to log the exception.
    /// </param>
    /// <param name="priority">
    /// Logging priority.
    /// </param>
    /// <param name="format">
    /// String to use for formatting args.
    /// </param>
    /// <param name="args">
    /// Additional arguments for the message.
    /// </param>
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

    /// <summary>
    /// Set the priority of all log categories.
    /// </summary>
    /// <param name="priority">
    /// <see cref="SDL_LogPriority"/> to assign.
    /// </param>
    public static void SetPriorities(SDL_LogPriority priority)
    {
        SDL_SetLogPriorities(priority);
    }

    /// <summary>
    /// Set the priority of a particular log category.
    /// </summary>
    /// <param name="category">Category to assign a priority to.</param>
    /// <param name="priority">
    /// <see cref="SDL_LogPriority"/> to assign.
    /// </param>
    public static void SetPriority(SDL_LogCategory category, SDL_LogPriority priority)
    {
        SDL_SetLogPriority((int)category, priority);
    }

    /// <summary>
    /// Set the text prepended to log messages of a given priority.
    /// </summary>
    /// <param name="priority">
    /// <see cref="SDL_LogPriority"/> to modify.
    /// </param>
    /// <param name="prefix">
    /// Prefix to use for the log priority, or null to use no prefix.
    /// </param>
    public static void SetPriorityPrefix(SDL_LogPriority priority, ReadOnlySpan<char> prefix)
    {
        using var prefixStr = new Utf8Span(prefix);
        SDL_SetLogPriorityPrefix(priority, prefixStr);
    }

    /// <summary>
    /// Reset all priorities to default.
    /// </summary>
    public static void ResetPriorities()
    {
        SDL_ResetLogPriorities();
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_TRACE"/>.
    /// </summary>
    /// <param name="format">
    /// String to use for formatting args.
    /// </param>
    /// <param name="args">
    /// Additional arguments for the message.
    /// </param>
    [Conditional("DEBUG")]
    [StringFormatMethod(nameof(format))]
    public static void Trace(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_TRACE, format, args);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_TRACE"/>.
    /// </summary>
    /// <param name="exception">
    /// Exception to log.
    /// </param>
    [Conditional("DEBUG")]
    public static void Trace(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_TRACE, exception);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_VERBOSE"/>.
    /// </summary>
    /// <param name="format">
    /// String to use for formatting args.
    /// </param>
    /// <param name="args">
    /// Additional arguments for the message.
    /// </param>
    [StringFormatMethod(nameof(format))]
    public static void Verbose(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_VERBOSE, format, args);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_VERBOSE"/>.
    /// </summary>
    /// <param name="exception">
    /// Exception to log.
    /// </param>
    public static void Verbose(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_VERBOSE, exception);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_WARN"/>.
    /// </summary>
    /// <param name="format">
    /// String to use for formatting args.
    /// </param>
    /// <param name="args">
    /// Additional arguments for the message.
    /// </param>
    [StringFormatMethod(nameof(format))]
    public static void Warn(
        string format,
        params ReadOnlySpan<object?> args
    )
    {
        Message(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_WARN, format, args);
    }

    /// <summary>
    /// Log a message with <see cref="SDL_LogPriority.SDL_LOG_PRIORITY_WARN"/>.
    /// </summary>
    /// <param name="exception">
    /// Exception to log.
    /// </param>
    public static void Warn(
        Exception exception
    )
    {
        Exception(AppCategory, SDL_LogPriority.SDL_LOG_PRIORITY_WARN, exception);
    }

    /// <summary>
    /// Gets the log output function set by <see cref="SetFunction"/>. Returns null
    /// when no function is set.
    /// </summary>
    public static LogOutputFunction? GetFunction()
    {
        return _logOutputFunction;
    }

    /// <summary>
    /// Sets the log output function.
    /// </summary>
    /// <param name="func">
    /// Function to use for logging. If null, the default SDL log function is used.
    /// </param>
    public static unsafe void SetFunction(LogOutputFunction? func)
    {
        _logOutputFunction = func;

        if (func == null)
        {
            SDL_SetLogOutputFunction(SDL_GetDefaultLogOutputFunction(), 0);
            return;
        }

        var funcPtr = (delegate* unmanaged[Cdecl]<IntPtr, int, SDL_LogPriority, byte*, void>)
            Marshal.GetFunctionPointerForDelegate(func);

        SDL_SetLogOutputFunction(funcPtr, 0);
    }
}