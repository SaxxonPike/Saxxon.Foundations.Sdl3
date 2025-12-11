using System.Numerics;

namespace Saxxon.Foundations.Sdl3.Game;

public abstract partial class Game
{
    /// <summary>
    /// Processes an event from SDL.
    /// </summary>
    private static SDL_AppResult MainEvent(Game game, ref SDL_Event e)
    {
        game._updateMutex.WaitOne();

        //
        // Handle the raw event.
        //

        game.SetLetterboxOff();
        game.OnPreEvent(ref e);
        game.SetLetterboxOn();

        //
        // Convert coordinates to the renderer's coordinate system.
        //

        game.Renderer.ConvertEventCoordinates(ref e);

        //
        // Handle the event.
        //

        var result = SDL_AppResult.SDL_APP_CONTINUE;

        switch (e.Type)
        {
            //
            // User events
            //

            case >= SDL_EventType.SDL_EVENT_USER and <= SDL_EventType.SDL_EVENT_LAST:
            {
                //
                // Game messages
                //

                if (e.Type == game._messageEventType &&
                    game._pendingMessages.Reader.TryRead(out var message))
                    game.OnMessageReceived(Time.GetFromNanoseconds(e.user.timestamp), message.Id, message.Content);

                break;
            }

            //
            // Keyboard events
            //

            case SDL_EventType.SDL_EVENT_KEYBOARD_ADDED:
            {
                game._keyboards.Add(e.kdevice.which);
                game.OnKeyboardConnected(e.kdevice.which);
                break;
            }
            case SDL_EventType.SDL_EVENT_KEYBOARD_REMOVED:
            {
                game._keyboards.Remove(e.kdevice.which);
                game.OnKeyboardDisconnected(e.kdevice.which);
                break;
            }
            case SDL_EventType.SDL_EVENT_KEY_DOWN:
            {
                game.OnKeyPressed(e.key.which, e.key.key, e.key.scancode, e.key.mod);
                break;
            }
            case SDL_EventType.SDL_EVENT_KEY_UP:
            {
                game.OnKeyReleased(e.key.which, e.key.key, e.key.scancode, e.key.mod);
                break;
            }

            //
            // Mouse events
            //

            case SDL_EventType.SDL_EVENT_MOUSE_ADDED:
            {
                game._mice.Add(e.mdevice.which);
                game.OnMouseConnected(e.mdevice.which);
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_REMOVED:
            {
                game._mice.Remove(e.mdevice.which);
                game.OnMouseDisconnected(e.mdevice.which);
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
            {
                game.OnMouseButtonPressed(e.button.which, (SDLButton)e.button.button);
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
            {
                game.OnMouseButtonReleased(e.button.which, (SDLButton)e.button.button);
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
            {
                game.OnMouseMoved(
                    e.motion.which,
                    new Vector2(e.motion.x, e.motion.y),
                    new Vector2(e.motion.xrel, e.motion.yrel)
                );
                break;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_WHEEL:
            {
                game.OnMouseWheelMoved(e.wheel.which, new Vector2(e.wheel.x, e.wheel.y), e.wheel.direction);
                break;
            }

            //
            // Gamepad events
            //

            case SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
            {
                var gamepad = e.gdevice.which.OpenGamepad();
                game._gamepads.Add(gamepad);
                game._gamepadMap.Add(e.gdevice.which, gamepad);
                game.OnGamepadConnected(gamepad);
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
            {
                game._gamepadMap.Remove(e.gdevice.which, out var gamepad);
                game._gamepads.Remove(gamepad);
                gamepad.Close();
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
            {
                game.OnGamepadAxisChanged(e.gaxis.which.GetGamepad(), e.gaxis.Axis, e.gaxis.value / 32767f);
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN:
            {
                game.OnGamepadButtonPressed(e.gaxis.which.GetGamepad(), e.gbutton.Button);
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP:
            {
                game.OnGamepadButtonReleased(e.gaxis.which.GetGamepad(), e.gbutton.Button);
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_MOTION:
            {
                game.OnTouchpadMoved(
                    e.gtouchpad.which.GetGamepad(),
                    e.gtouchpad.touchpad,
                    e.gtouchpad.finger,
                    new Vector2(e.gtouchpad.x, e.gtouchpad.y),
                    e.gtouchpad.pressure
                );
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_DOWN:
            {
                game.OnTouchpadPressed(
                    e.gtouchpad.which.GetGamepad(),
                    e.gtouchpad.touchpad,
                    e.gtouchpad.finger,
                    new Vector2(e.gtouchpad.x, e.gtouchpad.y),
                    e.gtouchpad.pressure
                );
                break;
            }
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_UP:
            {
                game.OnTouchpadReleased(
                    e.gtouchpad.which.GetGamepad(),
                    e.gtouchpad.touchpad,
                    e.gtouchpad.finger,
                    new Vector2(e.gtouchpad.x, e.gtouchpad.y),
                    e.gtouchpad.pressure
                );
                break;
            }

            //
            // Touch events
            //

            case SDL_EventType.SDL_EVENT_FINGER_DOWN:
            {
                game.OnFingerPressed(
                    e.tfinger.touchID,
                    e.tfinger.fingerID,
                    new Vector2(e.tfinger.x, e.tfinger.y),
                    new Vector2(e.tfinger.dx, e.tfinger.dy),
                    e.tfinger.pressure
                );
                break;
            }
            case SDL_EventType.SDL_EVENT_FINGER_UP:
            {
                game.OnFingerReleased(
                    e.tfinger.touchID,
                    e.tfinger.fingerID,
                    new Vector2(e.tfinger.x, e.tfinger.y),
                    new Vector2(e.tfinger.dx, e.tfinger.dy),
                    e.tfinger.pressure
                );
                break;
            }
            case SDL_EventType.SDL_EVENT_FINGER_MOTION:
            {
                game.OnFingerMoved(
                    e.tfinger.touchID,
                    e.tfinger.fingerID,
                    new Vector2(e.tfinger.x, e.tfinger.y),
                    new Vector2(e.tfinger.dx, e.tfinger.dy),
                    e.tfinger.pressure
                );
                break;
            }
            case SDL_EventType.SDL_EVENT_FINGER_CANCELED:
            {
                game.OnFingerCanceled(
                    e.tfinger.touchID,
                    e.tfinger.fingerID,
                    new Vector2(e.tfinger.x, e.tfinger.y),
                    new Vector2(e.tfinger.dx, e.tfinger.dy),
                    e.tfinger.pressure
                );
                break;
            }
            case SDL_EventType.SDL_EVENT_PINCH_BEGIN:
            {
                game.OnPinchStarted(e.pinch.scale);
                break;
            }
            case SDL_EventType.SDL_EVENT_PINCH_UPDATE:
            {
                game.OnPinchUpdated(e.pinch.scale);
                break;
            }
            case SDL_EventType.SDL_EVENT_PINCH_END:
            {
                game.OnPinchEnded(e.pinch.scale);
                break;
            }

            //
            // Window events
            //

            case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_GAINED:
            {
                game.IsFocused = true;
                game.OnFocusGained();
                break;
            }
            case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_LOST:
            {
                game.IsFocused = false;
                game.OnFocusLost();
                break;
            }
            case SDL_EventType.SDL_EVENT_QUIT:
            {
                if (!game._quitting)
                {
                    game._quitting = true;
                    game.OnQuitting();
                }

                game._closeToken.Cancel();
                game.OnQuit();

                result = game._crashing
                    ? SDL_AppResult.SDL_APP_FAILURE
                    : SDL_AppResult.SDL_APP_SUCCESS;
                break;
            }
        }

        game.OnEvent(e);
        game._updateMutex.ReleaseMutex();
        return result;
    }

    /// <summary>
    /// Handler for when SDL is logging a message.
    /// </summary>
    public delegate void LoggingDelegate(int category, SDL_LogPriority priority, ReadOnlySpan<char> message);

    /// <summary>
    /// Raised when SDL is logging a message.
    /// </summary>
    public event LoggingDelegate? Logging;

    /// <inheritdoc cref="LoggingDelegate"/>
    protected virtual void OnLogging(int category, SDL_LogPriority priority, ReadOnlySpan<char> message)
    {
        Logging?.Invoke(category, priority, message);
    }

    /// <summary>
    /// Handler for when the game has first started.
    /// </summary>
    public delegate void StartingDelegate();

    /// <summary>
    /// Raised when the game first starts.
    /// </summary>
    public event Action? Starting;

    /// <inheritdoc cref="StartingDelegate"/>
    protected virtual void OnStarting()
    {
        Starting?.Invoke();
    }

    /// <summary>
    /// Handles updating game state.
    /// </summary>
    /// <param name="delta">
    /// Amount of time that has passed since the last time game state was
    /// updated.
    /// </param>
    public delegate void UpdatingDelegate(TimeSpan delta);

    /// <summary>
    /// Raised when the game logic should update.
    /// </summary>
    public event UpdatingDelegate? Updating;

    /// <inheritdoc cref="UpdatingDelegate"/>
    protected virtual void OnUpdating(TimeSpan delta)
    {
        Updating?.Invoke(delta);
    }

    /// <summary>
    /// Handles rendering game objects.
    /// </summary>
    /// <param name="delta">
    /// Amount of time that has passed since the last time game objects were
    /// rendered.
    /// </param>
    public delegate void DrawingDelegate(TimeSpan delta);

    /// <summary>
    /// Raised when game objects should be rendered.
    /// </summary>
    public event DrawingDelegate? Drawing;

    /// <inheritdoc cref="DrawingDelegate"/>
    protected virtual void OnDrawing(TimeSpan delta)
    {
        Drawing?.Invoke(delta);
    }

    /// <summary>
    /// Handles raw events from SDL.
    /// </summary>
    /// <param name="e">
    /// Event data.
    /// </param>
    /// <remarks>
    /// Modifications to event data will be reflected when the event is
    /// actually broadcast.
    /// </remarks>
    public delegate void PreEventDelegate(ref SDL_Event e);

    /// <summary>
    /// Raised when an SDL event is about to be handled.
    /// </summary>
    /// <remarks>
    /// Modifications to event data will be reflected when the event is
    /// actually broadcast.
    /// </remarks>
    public event PreEventDelegate? PreEvent;

    /// <inheritdoc cref="PreEventDelegate"/>
    protected virtual void OnPreEvent(ref SDL_Event e)
    {
        PreEvent?.Invoke(ref e);
    }

    /// <summary>
    /// Handles incoming events from SDL.
    /// </summary>
    /// <param name="e">
    /// Event data.
    /// </param>
    /// <remarks>
    /// For event types that contain coordinate data, the coordinates will
    /// have been converted to the renderer's coordinate system prior to the
    /// call.
    /// </remarks>
    public delegate void EventDelegate(SDL_Event e);

    /// <summary>
    /// Raised when an SDL event was received and has been pre-processed.
    /// </summary>
    public event EventDelegate? Event;

    /// <inheritdoc cref="EventDelegate"/>
    protected virtual void OnEvent(SDL_Event e)
    {
        Event?.Invoke(e);
    }

    /// <summary>
    /// Handler for when a request to quit the game is made.
    /// </summary>
    public delegate void QuittingDelegate();

    /// <summary>
    /// Raised when the game is about to quit.
    /// </summary>
    public event QuittingDelegate? Quitting;

    /// <inheritdoc cref="QuittingDelegate"/>
    protected virtual void OnQuitting()
    {
        Quitting?.Invoke();
    }

    /// <summary>
    /// Handler for when the game has quit.
    /// </summary>
    public delegate void QuitDelegate();

    /// <summary>
    /// Raised when the game has quit.
    /// </summary>
    public event QuitDelegate? Quit;

    /// <inheritdoc cref="QuitDelegate"/>
    protected virtual void OnQuit()
    {
        Quit?.Invoke();
    }

    /// <summary>
    /// Handler for when a gamepad has been connected.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that was connected.
    /// </param>
    public delegate void GamepadConnectedDelegate(IntPtr<SDL_Gamepad> gamepad);

    /// <summary>
    /// Raised when a gamepad has been connected.
    /// </summary>
    public event GamepadConnectedDelegate? GamepadConnected;

    /// <inheritdoc cref="GamepadConnectedDelegate"/>
    protected virtual void OnGamepadConnected(IntPtr<SDL_Gamepad> gamepad)
    {
        GamepadConnected?.Invoke(gamepad);
    }

    /// <summary>
    /// Handler for when a gamepad has been disconnected.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that was disconnected.
    /// </param>
    public delegate void GamepadDisconnectedDelegate(IntPtr<SDL_Gamepad> gamepad);

    /// <summary>
    /// Raised when a gamepad has been disconnected.
    /// </summary>
    public event GamepadDisconnectedDelegate? GamepadDisconnected;

    /// <inheritdoc cref="GamepadDisconnectedDelegate"/>
    protected virtual void OnGamepadDisconnected(IntPtr<SDL_Gamepad> gamepad)
    {
        GamepadDisconnected?.Invoke(gamepad);
    }

    /// <summary>
    /// Handler for when a button on a gamepad has been pressed.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the button was pressed on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was pressed.
    /// </param>
    public delegate void GamepadButtonPressedDelegate(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadButton button);

    /// <summary>
    /// Raised when a button on a gamepad has been pressed.
    /// </summary>
    public event GamepadButtonPressedDelegate? GamepadButtonPressed;

    /// <inheritdoc cref="GamepadButtonPressedDelegate"/>
    protected virtual void OnGamepadButtonPressed(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadButton button)
    {
        GamepadButtonPressed?.Invoke(gamepad, button);
    }

    /// <summary>
    /// Handler for when a button on a gamepad has been released.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the button was released on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was released.
    /// </param>
    public delegate void GamepadButtonReleasedDelegate(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadButton button);

    /// <summary>
    /// Raised when a button on a gamepad has been released.
    /// </summary>
    public event GamepadButtonReleasedDelegate? GamepadButtonReleased;

    /// <inheritdoc cref="GamepadButtonReleasedDelegate"/>
    protected virtual void OnGamepadButtonReleased(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadButton button)
    {
        GamepadButtonReleased?.Invoke(gamepad, button);
    }

    /// <summary>
    /// Handler for when a gamepad axis has changed.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the axis was changed on.
    /// </param>
    /// <param name="axis">
    /// ID of the axis that was changed.
    /// </param>
    /// <param name="value">
    /// The new value of the axis.
    /// </param>
    public delegate void GamepadAxisChangedDelegate(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadAxis axis, float value);

    /// <summary>
    /// Raised when a gamepad axis has changed.
    /// </summary>
    public event GamepadAxisChangedDelegate? GamepadAxisChanged;

    /// <inheritdoc cref="GamepadAxisChangedDelegate"/>
    protected virtual void OnGamepadAxisChanged(IntPtr<SDL_Gamepad> gamepad, SDL_GamepadAxis axis, float value)
    {
        GamepadAxisChanged?.Invoke(gamepad, axis, value);
    }

    /// <summary>
    /// Handler for when a gamepad touchpad has been pressed.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the touchpad was pressed on.
    /// </param>
    /// <param name="padIndex">
    /// Touchpad that was pressed.
    /// </param>
    /// <param name="fingerIndex">
    /// Finger that was pressed.
    /// </param>
    /// <param name="position">
    /// Position of the touch.
    /// </param>
    /// <param name="pressure">
    /// Pressure of the touch.
    /// </param>
    public delegate void TouchpadPressedDelegate(
        IntPtr<SDL_Gamepad> gamepad,
        int padIndex,
        int fingerIndex,
        Vector2 position,
        float pressure);

    /// <summary>
    /// Raised when a gamepad touchpad has been pressed.
    /// </summary>
    public event TouchpadPressedDelegate? TouchpadPressed;

    /// <inheritdoc cref="TouchpadPressedDelegate"/>
    protected virtual void OnTouchpadPressed(
        IntPtr<SDL_Gamepad> gamepad,
        int padIndex,
        int fingerIndex,
        Vector2 position,
        float pressure)
    {
        TouchpadPressed?.Invoke(gamepad, padIndex, fingerIndex, position, pressure);
    }

    /// <summary>
    /// Handler for when a gamepad touchpad has been moved.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the touchpad was moved on.
    /// </param>
    /// <param name="padIndex">
    /// Touchpad that was moved.
    /// </param>
    /// <param name="fingerIndex">
    /// Finger that was moved.
    /// </param>
    /// <param name="position">
    /// Position of the touch.
    /// </param>
    /// <param name="pressure">
    /// Pressure of the touch.
    /// </param>
    public delegate void TouchpadMovedDelegate(
        IntPtr<SDL_Gamepad> gamepad,
        int padIndex,
        int fingerIndex,
        Vector2 position,
        float pressure);

    /// <summary>
    /// Raised when a gamepad touchpad has been moved.
    /// </summary>
    public event TouchpadMovedDelegate? TouchpadMoved;

    /// <inheritdoc cref="TouchpadMovedDelegate"/>
    protected virtual void OnTouchpadMoved(
        IntPtr<SDL_Gamepad> gamepad,
        int padIndex,
        int fingerIndex,
        Vector2 position,
        float pressure)
    {
        TouchpadMoved?.Invoke(gamepad, padIndex, fingerIndex, position, pressure);
    }

    /// <summary>
    /// Handler for when a gamepad touchpad has been released.
    /// </summary>
    /// <param name="gamepad">
    /// Gamepad device that the touchpad was released on.
    /// </param>
    /// <param name="padIndex">
    /// Touchpad that was released.
    /// </param>
    /// <param name="fingerIndex">
    /// Finger that was released.
    /// </param>
    /// <param name="position">
    /// Position of the touch.
    /// </param>
    /// <param name="pressure">
    /// Pressure of the touch.
    /// </param>
    public delegate void TouchpadReleasedDelegate(
        IntPtr<SDL_Gamepad> gamepad,
        int padIndex,
        int fingerIndex,
        Vector2 position,
        float pressure);

    /// <summary>
    /// Raised when a gamepad touchpad has been released.
    /// </summary>
    public event TouchpadReleasedDelegate? TouchpadReleased;

    /// <inheritdoc cref="TouchpadReleasedDelegate"/>
    protected virtual void OnTouchpadReleased(
        IntPtr<SDL_Gamepad> gamepad,
        int padIndex,
        int fingerIndex,
        Vector2 position,
        float pressure)
    {
        TouchpadReleased?.Invoke(gamepad, padIndex, fingerIndex, position, pressure);
    }

    /// <summary>
    /// Handler for when a mouse has been connected.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that was connected.
    /// </param>
    public delegate void MouseConnectedDelegate(SDL_MouseID mouse);

    /// <summary>
    /// Raised when a mouse has been connected.
    /// </summary>
    public event MouseConnectedDelegate? MouseConnected;

    /// <inheritdoc cref="MouseConnectedDelegate"/>
    protected virtual void OnMouseConnected(SDL_MouseID mouse)
    {
        MouseConnected?.Invoke(mouse);
    }

    /// <summary>
    /// Handler for when a mouse has been disconnected.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that was disconnected.
    /// </param>
    public delegate void MouseDisconnectedDelegate(SDL_MouseID mouse);

    /// <summary>
    /// Raised when a mouse has been disconnected.
    /// </summary>
    public event MouseDisconnectedDelegate? MouseDisconnected;

    /// <inheritdoc cref="MouseDisconnectedDelegate"/>
    protected virtual void OnMouseDisconnected(SDL_MouseID mouse)
    {
        MouseDisconnected?.Invoke(mouse);
    }

    /// <summary>
    /// Handler for when a mouse button has been pressed.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that the button was pressed on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was pressed.
    /// </param>
    public delegate void MouseButtonPressedDelegate(SDL_MouseID mouse, SDLButton button);

    /// <summary>
    /// Raised when a mouse button has been pressed.
    /// </summary>
    public event MouseButtonPressedDelegate? MouseButtonPressed;

    /// <inheritdoc cref="MouseButtonPressedDelegate"/>
    protected virtual void OnMouseButtonPressed(SDL_MouseID mouse, SDLButton button)
    {
        MouseButtonPressed?.Invoke(mouse, button);
    }

    /// <summary>
    /// Handler for when a mouse button has been released.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that the button was released on.
    /// </param>
    /// <param name="button">
    /// ID of the button that was released.
    /// </param>
    public delegate void MouseButtonReleasedDelegate(SDL_MouseID mouse, SDLButton button);

    /// <summary>
    /// Raised when a mouse button has been released.
    /// </summary>
    public event MouseButtonReleasedDelegate? MouseButtonReleased;

    /// <inheritdoc cref="MouseButtonReleasedDelegate"/>
    protected virtual void OnMouseButtonReleased(SDL_MouseID mouse, SDLButton button)
    {
        MouseButtonReleased?.Invoke(mouse, button);
    }

    /// <summary>
    /// Handler for when a mouse has moved.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that moved.
    /// </param>
    /// <param name="position">
    /// New coordinates of the mouse.
    /// </param>
    /// <param name="delta">
    /// Delta coordinates of the movement.
    /// </param>
    public delegate void MouseMovedDelegate(SDL_MouseID mouse, Vector2 position, Vector2 delta);

    /// <summary>
    /// Raised when a mouse has moved.
    /// </summary>
    public event MouseMovedDelegate? MouseMoved;

    /// <inheritdoc cref="MouseMovedDelegate"/>
    protected virtual void OnMouseMoved(SDL_MouseID mouse, Vector2 position, Vector2 delta)
    {
        MouseMoved?.Invoke(mouse, position, delta);
    }

    /// <summary>
    /// Handler for when a mouse wheel has moved.
    /// </summary>
    /// <param name="mouse">
    /// ID of the mouse that the wheel was moved on.
    /// </param>
    /// <param name="amount">
    /// Scroll amount.
    /// </param>
    /// <param name="direction">
    /// Direction of the scroll.
    /// </param>
    public delegate void MouseWheelMovedDelegate(SDL_MouseID mouse, Vector2 amount, SDL_MouseWheelDirection direction);

    /// <summary>
    /// Raised when a mouse wheel has moved.
    /// </summary>
    public event MouseWheelMovedDelegate? MouseWheelMoved;

    /// <inheritdoc cref="MouseWheelMovedDelegate"/>
    protected virtual void OnMouseWheelMoved(SDL_MouseID mouse, Vector2 amount, SDL_MouseWheelDirection direction)
    {
        MouseWheelMoved?.Invoke(mouse, amount, direction);
    }

    /// <summary>
    /// Handler for when a key has been pressed.
    /// </summary>
    /// <param name="keyboard">
    /// ID of the keyboard that the key was pressed on.
    /// </param>
    /// <param name="code">
    /// Key code for the key that was pressed.
    /// </param>
    /// <param name="scan">
    /// Scan code for the key that was pressed.
    /// </param>
    /// <param name="mod">
    /// Modifier keys.
    /// </param>
    public delegate void KeyPressedDelegate(SDL_KeyboardID keyboard, SDL_Keycode code, SDL_Scancode scan,
        SDL_Keymod mod);

    /// <summary>
    /// Raised when a keyboard key has been pressed.
    /// </summary>
    public event KeyPressedDelegate? KeyPressed;

    /// <inheritdoc cref="KeyPressedDelegate"/>
    protected virtual void OnKeyPressed(SDL_KeyboardID keyboard, SDL_Keycode code, SDL_Scancode scan, SDL_Keymod mod)
    {
        KeyPressed?.Invoke(keyboard, code, scan, mod);
    }

    /// <summary>
    /// Handler for when a key has been released.
    /// </summary>
    /// <param name="keyboard">
    /// ID of the keyboard that the key was released on.
    /// </param>
    /// <param name="code">
    /// Key code for the key that was released.
    /// </param>
    /// <param name="scan">
    /// Scan code for the key that was released.
    /// </param>
    /// <param name="mod">
    /// Modifier keys.
    /// </param>
    public delegate void KeyReleasedDelegate(SDL_KeyboardID keyboard, SDL_Keycode code, SDL_Scancode scan,
        SDL_Keymod mod);

    /// <summary>
    /// Raised when a keyboard key has been released.
    /// </summary>
    public event KeyReleasedDelegate? KeyReleased;

    /// <inheritdoc cref="KeyReleasedDelegate"/>
    protected virtual void OnKeyReleased(SDL_KeyboardID keyboard, SDL_Keycode code, SDL_Scancode scan, SDL_Keymod mod)
    {
        KeyReleased?.Invoke(keyboard, code, scan, mod);
    }

    /// <summary>
    /// Handler for when a keyboard has been connected.
    /// </summary>
    /// <param name="keyboard">
    /// ID of the keyboard that was connected.
    /// </param>
    public delegate void KeyboardConnectedDelegate(SDL_KeyboardID keyboard);

    /// <summary>
    /// Raised when a keyboard has been connected.
    /// </summary>
    public event KeyboardConnectedDelegate? KeyboardConnected;

    /// <inheritdoc cref="KeyboardConnectedDelegate"/>
    protected virtual void OnKeyboardConnected(SDL_KeyboardID keyboard)
    {
        KeyboardConnected?.Invoke(keyboard);
    }

    /// <summary>
    /// Handler for when a keyboard has been disconnected.
    /// </summary>
    /// <param name="keyboard">
    /// ID of the keyboard that was disconnected.
    /// </param>
    public delegate void KeyboardDisconnectedDelegate(SDL_KeyboardID keyboard);

    /// <summary>
    /// Raised when a keyboard has been disconnected.
    /// </summary>
    public event KeyboardDisconnectedDelegate? KeyboardDisconnected;

    /// <inheritdoc cref="KeyboardDisconnectedDelegate"/>
    protected virtual void OnKeyboardDisconnected(SDL_KeyboardID keyboard)
    {
        KeyboardDisconnected?.Invoke(keyboard);
    }

    /// <summary>
    /// Handler for when the game window gains focus.
    /// </summary>
    public delegate void FocusGainedDelegate();

    /// <summary>
    /// Raised when the game window gains focus.
    /// </summary>
    public event FocusGainedDelegate? FocusGained;

    /// <inheritdoc cref="FocusGainedDelegate"/>
    protected virtual void OnFocusGained()
    {
        FocusGained?.Invoke();
    }

    /// <summary>
    /// Handler for when the game window loses focus.
    /// </summary>
    public delegate void FocusLostDelegate();

    /// <summary>
    /// Raised when the game window loses focus.
    /// </summary>
    public event FocusLostDelegate? FocusLost;

    /// <inheritdoc cref="FocusLostDelegate"/>
    protected virtual void OnFocusLost()
    {
        FocusLost?.Invoke();
    }

    /// <summary>
    /// Handler for when <see cref="Dispose"/> is called.
    /// </summary>
    public delegate void DisposedDelegate();

    /// <summary>
    /// Raised when <see cref="Dispose"/> is called.
    /// </summary>
    public event DisposedDelegate? Disposed;

    /// <inheritdoc cref="DisposedDelegate"/>
    protected virtual void OnDispose()
    {
        Disposed?.Invoke();
    }

    /// <summary>
    /// Handler for when the game scene is about to be presented to the window.
    /// </summary>
    public delegate void PresentingDelegate(TimeSpan delta);

    /// <summary>
    /// Raised when the game scene is about to be presented to the window.
    /// </summary>
    public event PresentingDelegate? Presenting;

    /// <inheritdoc cref="PresentingDelegate"/>
    protected virtual void OnPresenting(TimeSpan delta)
    {
        Presenting?.Invoke(delta);
    }

    /// <summary>
    /// Handler for when a game message will be processed.
    /// </summary>
    public delegate void MessageReceivedDelegate(TimeSpan delta, Guid id, object data);

    /// <summary>
    /// Raised when a game message will be processed.
    /// </summary>
    public event MessageReceivedDelegate? MessageReceived;

    /// <inheritdoc cref="MessageReceivedDelegate"/>
    protected virtual void OnMessageReceived(TimeSpan delta, Guid id, object data)
    {
        MessageReceived?.Invoke(delta, id, data);
    }

    /// <summary>
    /// Handler for when a finger is pressed down.
    /// </summary>
    public delegate void FingerPressedDelegate(
        SDL_TouchID touchId,
        SDL_FingerID fingerId,
        Vector2 location,
        Vector2 delta,
        float pressure
    );

    /// <summary>
    /// Raised when a finger is pressed down.
    /// </summary>
    public event FingerPressedDelegate? FingerPressed;

    /// <inheritdoc cref="FingerPressedDelegate"/>
    protected virtual void OnFingerPressed(
        SDL_TouchID touchId,
        SDL_FingerID fingerId,
        Vector2 location,
        Vector2 delta,
        float pressure
    )
    {
        FingerPressed?.Invoke(touchId, fingerId, location, delta, pressure);
    }

    /// <summary>
    /// Handler for when a finger is released.
    /// </summary>
    public delegate void FingerReleasedDelegate(
        SDL_TouchID touchId,
        SDL_FingerID fingerId,
        Vector2 location,
        Vector2 delta,
        float pressure
    );

    /// <summary>
    /// Raised when a finger is released.
    /// </summary>
    public event FingerReleasedDelegate? FingerReleased;

    /// <inheritdoc cref="FingerReleasedDelegate"/>
    protected virtual void OnFingerReleased(
        SDL_TouchID touch,
        SDL_FingerID finger,
        Vector2 location,
        Vector2 delta,
        float pressure
    )
    {
        FingerReleased?.Invoke(touch, finger, location, delta, pressure);
    }

    /// <summary>
    /// Handler for when a finger is moved.
    /// </summary>
    public delegate void FingerMovedDelegate(
        SDL_TouchID touch,
        SDL_FingerID finger,
        Vector2 location,
        Vector2 delta,
        float pressure
    );

    /// <summary>
    /// Raised when a finger is moved.
    /// </summary>
    public event FingerMovedDelegate? FingerMoved;

    /// <inheritdoc cref="FingerMovedDelegate"/>
    protected virtual void OnFingerMoved(
        SDL_TouchID touch,
        SDL_FingerID finger,
        Vector2 location,
        Vector2 delta,
        float pressure
    )
    {
        FingerMoved?.Invoke(touch, finger, location, delta, pressure);
    }

    /// <summary>
    /// Handler for when a finger motion is canceled.
    /// </summary>
    public delegate void FingerCanceledDelegate(
        SDL_TouchID touch,
        SDL_FingerID finger,
        Vector2 location,
        Vector2 delta,
        float pressure
    );

    /// <summary>
    /// Raised when a finger motion is canceled.
    /// </summary>
    public event FingerCanceledDelegate? FingerCanceled;

    /// <inheritdoc cref="FingerCanceledDelegate"/>
    protected virtual void OnFingerCanceled(
        SDL_TouchID touch,
        SDL_FingerID finger,
        Vector2 location,
        Vector2 delta,
        float pressure
    )
    {
        FingerCanceled?.Invoke(touch, finger, location, delta, pressure);
    }

    /// <summary>
    /// Handler for when a finger pinch motion begins.
    /// </summary>
    public delegate void PinchStartedDelegate(float scale);

    /// <summary>
    /// Raised when a finger pinch motion has begun.
    /// </summary>
    public event PinchStartedDelegate? PinchStarted;

    /// <inheritdoc cref="PinchStartedDelegate"/>
    protected virtual void OnPinchStarted(float scale)
    {
        PinchStarted?.Invoke(scale);
    }

    /// <summary>
    /// Handler for when a finger pinch motion updates.
    /// </summary>
    public delegate void PinchUpdatedDelegate(float scale);

    /// <summary>
    /// Raised when a finger pinch motion has been updated.
    /// </summary>
    public event PinchUpdatedDelegate? PinchUpdated;

    /// <inheritdoc cref="PinchUpdatedDelegate"/>
    protected virtual void OnPinchUpdated(float scale)
    {
        PinchUpdated?.Invoke(scale);
    }

    /// <summary>
    /// Handler for when a finger pinch motion ends.
    /// </summary>
    public delegate void PinchEndedDelegate(float scale);

    /// <summary>
    /// Raised when a finger pinch motion has ended.
    /// </summary>
    public event PinchEndedDelegate? PinchEnded;

    /// <inheritdoc cref="PinchEndedDelegate"/>
    protected virtual void OnPinchEnded(float scale)
    {
        PinchEnded?.Invoke(scale);
    }
}