namespace Saxxon.Foundations.Sdl3.Game;

public abstract partial class Game
{
    /// <summary>
    /// Audio mixer.
    /// </summary>
    public IntPtr<MIX_Mixer> Mixer { get; private set; }
}