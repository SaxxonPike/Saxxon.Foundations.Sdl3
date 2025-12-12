namespace Saxxon.Foundations.Sdl3.Game;

public abstract partial class Game
{
    /// <summary>
    /// Maps joystick IDs to opened gamepads.
    /// </summary>
    private Dictionary<SDL_JoystickID, IntPtr<SDL_Gamepad>> _gamepadMap = [];

    /// <summary>
    /// Tracks opened gamepads.
    /// </summary>
    private HashSet<IntPtr<SDL_Gamepad>> _gamepads = [];

    /// <summary>
    /// Tracks connected keyboards.
    /// </summary>
    private HashSet<SDL_KeyboardID> _keyboards = [];

    /// <summary>
    /// Tracks connected mice.
    /// </summary>
    private HashSet<SDL_MouseID> _mice = [];

    /// <summary>
    /// Gets the IDs of all currently connected keyboards.
    /// </summary>
    protected IReadOnlySet<SDL_KeyboardID> Keyboards => _keyboards;

    /// <summary>
    /// Gets the IDs of all currently connected mice.
    /// </summary>
    protected IReadOnlySet<SDL_MouseID> Mice => _mice;

    /// <summary>
    /// Gets the IDs of all currently connected gamepads.
    /// </summary>
    protected IReadOnlySet<IntPtr<SDL_Gamepad>> Gamepads => _gamepads;
}