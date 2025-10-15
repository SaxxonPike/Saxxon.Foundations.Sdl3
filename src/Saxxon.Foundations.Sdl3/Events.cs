using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Event"/>.
/// </summary>
[PublicAPI]
public static class Events
{
    /// <summary>
    /// Generate a human-readable description of an event.
    /// </summary>
    /// <param name="ev">
    /// An event to describe.
    /// </param>
    /// <returns>
    /// The description itself.
    /// </returns>
    /// <remarks>
    /// The exact format of the string is not guaranteed; it is intended for logging purposes, to be read by a human,
    /// and not parsed by a computer.
    ///
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe string GetDescription(this ref SDL_Event ev) =>
        GetDescription((IntPtr<SDL_Event>)Unsafe.AsPointer(ref ev));

    /// <summary>
    /// Generate a human-readable description of an event.
    /// </summary>
    /// <param name="ev">
    /// An event to describe.
    /// </param>
    /// <returns>
    /// The description itself.
    /// </returns>
    /// <remarks>
    /// The exact format of the string is not guaranteed; it is intended for logging purposes, to be read by a human,
    /// and not parsed by a computer.
    ///
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe string GetDescription(this IntPtr<SDL_Event> ev)
    {
        Span<byte> str = stackalloc byte[512];
        fixed (byte* ptr = str)
        {
            _ = SDL_GetEventDescription(ev, ptr, str.Length);
            return Ptr.ToUtf8String(ptr)!;
        }
    }

    /// <summary>
    /// Gets the window associated with an event.
    /// </summary>
    /// <param name="ev">
    /// An event containing a window ID.
    /// </param>
    /// <returns>
    /// The associated window on success, or null if there is none.
    /// </returns>
    /// <remarks>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe IntPtr<SDL_Window> GetWindow(this ref SDL_Event ev)
    {
        return GetWindow((IntPtr<SDL_Event>)Unsafe.AsPointer(ref ev));
    }

    /// <summary>
    /// Gets the window associated with an event.
    /// </summary>
    /// <param name="ev">
    /// An event containing a window ID.
    /// </param>
    /// <returns>
    /// The associated window on success, or null if there is none.
    /// </returns>
    /// <remarks>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe IntPtr<SDL_Window> GetWindow(this IntPtr<SDL_Event> ev) =>
        SDL_GetWindowFromEvent(ev);

    /// <summary>
    /// Query the current event filter.
    /// </summary>
    /// <returns>
    /// The current event filter callback function.
    /// </returns>
    /// <remarks>
    /// This function can be used to "chain" filters, by saving the existing filter before replacing it with a function
    /// that will call that saved filter.
    ///
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe EventFilterFunction? GetFilter()
    {
        delegate* unmanaged[Cdecl]<IntPtr, SDL_Event*, SDLBool> filter;
        IntPtr userData;
        return SDL_GetEventFilter(&filter, &userData)
            ? UserDataStore.Get<EventFilterFunction>(userData)
            : null;
    }

    /// <summary>
    /// Adds an event to the event queue.
    /// </summary>
    /// <param name="ev">
    /// The SDL_Event to be added to the queue.
    /// </param>
    /// <exception cref="SdlException">
    /// Thrown if an error occurred. A common reason for error is the event queue being full.
    /// </exception>
    /// <remarks>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe void Push(this ref SDL_Event ev) =>
        Push((IntPtr<SDL_Event>)Unsafe.AsPointer(ref ev));

    /// <summary>
    /// Adds an event to the event queue.
    /// </summary>
    /// <param name="ev">
    /// The <see cref="SDL_Event"/> to be added to the queue.
    /// </param>
    /// <exception cref="SdlException">
    /// Thrown if an error occurred. A common reason for error is the event queue being full.
    /// </exception>
    /// <remarks>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe void Push(this IntPtr<SDL_Event> ev)
    {
        var result = SDL_PushEvent(ev);
        if (result)
            return;

        var err = Unsafe_SDL_GetError();
        if (err != null)
            throw new SdlException(Ptr.ToUtf8String(err)!);
    }

    /// <summary>
    /// Wait until the specified timeout (in milliseconds) for the next available event.
    /// </summary>
    /// <param name="timeout">
    /// The maximum number of milliseconds to wait for the next available event.
    /// </param>
    /// <returns>
    /// The next event within the timeout, or null if the timeout elapsed without any events available.
    /// </returns>
    /// <remarks>
    /// As this function may implicitly call <see cref="Pump"/>, you can only call this function in the thread that
    /// initialized the video subsystem.
    ///
    /// This function should only be called on the main thread.
    /// </remarks>
    public static unsafe SDL_Event? WaitTimeout(TimeSpan timeout)
    {
        SDL_Event ev;
        if (SDL_WaitEventTimeout(&ev, timeout.ToMilliseconds()))
            return ev;
        return null;
    }

    /// <summary>
    /// Wait until the specified timeout (in milliseconds) for the next available event.
    /// </summary>
    /// <param name="timeout">
    /// The maximum number of milliseconds to wait for the next available event.
    /// </param>
    /// <returns>
    /// True if an event arrived within the timeout, false otherwise.
    /// </returns>
    /// <remarks>
    /// This version of <see cref="WaitTimeout"/> leaves the event at the front of the queue.
    /// 
    /// As this function may implicitly call <see cref="Pump"/>, you can only call this function in the thread that
    /// initialized the video subsystem.
    ///
    /// This function should only be called on the main thread.
    /// </remarks>
    public static unsafe bool WaitTimeoutNoReturn(TimeSpan timeout) =>
        SDL_WaitEventTimeout(null, timeout.ToMilliseconds());

    /// <summary>
    /// Wait indefinitely for the next available event.
    /// </summary>
    /// <remarks>
    /// This function should only be called on the main thread.
    /// </remarks>
    public static unsafe SDL_Event Wait()
    {
        SDL_Event ev;
        SDL_WaitEvent(&ev)
            .AssertSdlSuccess();
        return ev;
    }

    /// <summary>
    /// Wait indefinitely for the next available event.
    /// </summary>
    /// <remarks>
    /// This version of <see cref="Wait"/> leaves the event at the front of the queue.
    ///
    /// This function should only be called on the main thread.
    /// </remarks>
    public static unsafe void WaitNoReturn() =>
        SDL_WaitEvent(null).AssertSdlSuccess();

    /// <summary>
    /// Poll for currently pending events.
    /// </summary>
    /// <returns>
    /// The next event from the queue, or null if there are none available.
    /// </returns>
    /// <remarks>
    /// This function should only be called on the main thread.
    /// </remarks>
    public static unsafe SDL_Event? Poll()
    {
        SDL_Event ev;
        if (SDL_PollEvent(&ev))
            return ev;
        return null;
    }

    /// <summary>
    /// Pump the event loop, gathering events from the input devices.
    /// </summary>
    /// <remarks>
    /// This function updates the event queue and internal input device state. This function gathers all the pending
    /// input information from devices and places it in the event queue. Without calls to this function, no events
    /// would ever be placed on the queue. Often the need for calls to this function is hidden from the user since
    /// <see cref="Poll"/> and <see cref="Wait(ref SDL_Event)"/> implicitly call this function. However, if you are not
    /// polling or waiting for events (e.g. you are filtering them), then you must call this function to force an event
    /// queue update.
    ///
    /// This function should only be called on the main thread.
    /// </remarks>
    public static void Pump() =>
        SDL_PumpEvents();

    /// <summary>
    /// Check the event queue for messages and returns them.
    /// </summary>
    /// <param name="events">
    /// Buffer that will store the returned events.
    /// </param>
    /// <param name="action">
    /// Action to take.
    /// </param>
    /// <param name="minType">
    /// Minimum value of the event type to be considered; <see cref="SDL_EventType.SDL_EVENT_FIRST"/> is a safe choice.
    /// </param>
    /// <param name="maxType">
    /// Maximum value of the event type to be considered; <see cref="SDL_EventType.SDL_EVENT_LAST"/> is a safe choice.
    /// </param>
    /// <returns>
    /// A span over the retrieved events.
    /// </returns>
    /// <exception cref="SdlException">
    /// Thrown if SDL encounters an error.
    /// </exception>
    /// <remarks>
    /// <see cref="Pump"/> may need to be called before calling this function. Otherwise, the events may not be ready
    /// to be filtered when this function is called.
    ///
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe Span<SDL_Event> Peep(
        Span<SDL_Event> events,
        SDL_EventAction action,
        SDL_EventType minType,
        SDL_EventType maxType
    )
    {
        fixed (SDL_Event* evPtr = events)
        {
            var count = SDL_PeepEvents(evPtr, events.Length, action, unchecked((uint)minType),
                unchecked((uint)maxType));
            return count < 0 ? throw new SdlException() : events[..count];
        }
    }

    /// <summary>
    /// Allocates a set of user-defined events.
    /// </summary>
    /// <param name="numEvents">
    /// The number of events to be allocated.
    /// </param>
    /// <returns>
    /// The beginning event number, or 0 if numEvents is invalid or if there are not enough user-defined events left.
    /// </returns>
    /// <remarks>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static SDL_EventType Register(int numEvents) =>
        unchecked((SDL_EventType)SDL_RegisterEvents(numEvents));

    /// <summary>
    /// Clears events of a specific type from the event queue.
    /// </summary>
    /// <param name="type">
    /// Type of event to clear.
    /// </param>
    /// <remarks>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static void Flush(SDL_EventType type) =>
        SDL_FlushEvent(type);

    /// <summary>
    /// Clears events of a range of types from the event queue.
    /// </summary>
    /// <param name="minType">
    /// The low end of event type to be cleared, inclusive.
    /// </param>
    /// <param name="maxType">
    /// The high end of event type to be cleared, inclusive.
    /// </param>
    /// <remarks>
    /// This will unconditionally remove any events from the queue that are in the range of minType to maxType,
    /// inclusive. If you need to remove a single event type, use <see cref="Flush(SDL_EventType)"/> instead.
    ///
    /// It's also normal to just ignore events you don't care about in your event loop without calling this function.
    ///
    /// This function only affects currently queued events. If you want to make sure that all pending OS events are
    /// flushed, you can call <see cref="Pump"/> on the main thread immediately before the flush call.
    ///
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static void Flush(SDL_EventType minType, SDL_EventType maxType) =>
        SDL_FlushEvents(minType, maxType);

    /// <summary>
    /// Queries the state of processing events by type.
    /// </summary>
    /// <param name="type">
    /// The type of event.
    /// </param>
    /// <returns>
    /// True if the event is being processed, false otherwise.
    /// </returns>
    /// <remarks>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static bool Enabled(SDL_EventType type) =>
        SDL_EventEnabled(type);

    /// <summary>
    /// Checks for the existence of a certain event type in the event queue.
    /// </summary>
    /// <param name="type">
    /// The type of event to be queried.
    /// </param>
    /// <returns>
    /// True if events matching type are present, or false if events matching type are not present.
    /// </returns>
    public static bool Has(SDL_EventType type) =>
        SDL_HasEvent(type);

    /// <summary>
    /// Checks for the existence of a certain event type in the event queue.
    /// </summary>
    /// <param name="minType">
    /// The low end of event type to be queried, inclusive.
    /// </param>
    /// <param name="maxType">
    /// The high end of event type to be queried, inclusive.
    /// </param>
    /// <returns>
    /// True if events matching type are present, or false if events matching type are not present.
    /// </returns>
    public static bool Has(SDL_EventType minType, SDL_EventType maxType) =>
        SDL_HasEvents(unchecked((uint)minType), unchecked((uint)maxType));

    /// <summary>
    /// Adds a callback to be triggered when an event is added to the event queue.
    /// </summary>
    /// <param name="func">
    /// Function to call when an event happens.
    /// </param>
    /// <remarks>
    /// The filter function  will be called when an event happens, and its return value is ignored.
    ///
    /// Be very careful of what you do in the event filter function, as it may run in a different thread!
    ///
    /// If the quit event is generated by a signal (e.g. SIGINT), it will bypass the internal queue and be delivered to
    /// the watch callback immediately, and arrive at the next event poll.
    ///
    /// Note: The callback is called for events posted by the user through <see cref="Push(ref SDL_Event)"/>, but not
    /// for disabled events, nor for events by a filter callback set with SDL_SetEventFilter(), nor for events
    /// posted by the user through <see cref="Peep"/>.
    /// 
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe void AddWatch(EventFilterFunction func)
    {
        SDL_AddEventWatch(EventFilterFunction.Callback, func.UserData)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Removes an event watch callback added with <see cref="AddWatch"/>.
    /// </summary>
    /// <param name="func">
    /// The function originally passed to <see cref="AddWatch"/>.
    /// </param>
    /// <remarks>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe void RemoveWatch(EventFilterFunction func)
    {
        SDL_RemoveEventWatch(EventFilterFunction.Callback, func.UserData);
    }

    /// <summary>
    /// Set up a filter to process all events before they are added to the internal event queue.
    /// </summary>
    /// <param name="func">
    /// The function to call when an event happens.
    /// </param>
    /// <remarks>
    /// If you just want to see events without modifying them or preventing them from being queued, you should use
    /// <see cref="AddWatch"/> instead.
    ///
    /// If the filter function returns true when called, then the event will be added to the internal queue. If it
    /// returns false, then the event will be dropped from the queue, but the internal state will still be updated.
    /// This allows selective filtering of dynamically arriving events.
    ///
    /// Be very careful of what you do in the event filter function, as it may run in a different thread! The
    /// exception is handling of <see cref="SDL_EventType.SDL_EVENT_WINDOW_EXPOSED"/>, which is guaranteed to be sent
    /// from the OS on the main thread, and you are expected to redraw your window in response to this event.
    ///
    /// On platforms that support it, if the quit event is generated by an interrupt signal (e.g. pressing Ctrl-C), it
    /// will be delivered to the application at the next event poll.
    ///
    /// Disabled events never make it to the event filter function.
    ///
    /// Events pushed onto the queue with <see cref="Push(ref SDL_Event)"/> get passed through the event filter, but
    /// events pushed onto the queue with <see cref="Peep"/> do not.
    ///
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe void SetFilter(EventFilterFunction func) => 
        SDL_SetEventFilter(EventFilterFunction.Callback, func.UserData);

    /// <summary>
    /// Sets the state of processing events by type.
    /// </summary>
    /// <param name="type">
    /// The type of event to enable or disable.
    /// </param>
    /// <param name="enabled">
    /// Whether to process the event or not.
    /// </param>
    /// <remarks>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static void SetEnabled(SDL_EventType type, bool enabled) =>
        SDL_SetEventEnabled(type, enabled);

    /// <summary>
    /// Run a specific filter function on the current event queue, removing any events for which the filter
    /// returns false.
    /// </summary>
    /// <param name="func">
    /// The filter function to call when an event happens.
    /// </param>
    /// <remarks>
    /// Unlike <see cref="SetFilter"/>, this function does not change the filter permanently, it only uses the supplied
    /// filter until this function returns.
    ///
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static unsafe void Filter(EventFilterFunction func)
    {
        SDL_FilterEvents(EventFilterFunction.Callback, func.UserData);
    }
}