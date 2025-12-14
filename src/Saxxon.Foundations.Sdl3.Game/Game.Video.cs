namespace Saxxon.Foundations.Sdl3.Game;

public abstract partial class Game
{
    /// <summary>
    /// Texture that is used as a target for upscaling prior to presentation.
    /// </summary>
    private IntPtr<SDL_Texture> _scaleTexture;

    /// <summary>
    /// SDL renderer for the game window.
    /// </summary>
    public IntPtr<SDL_Renderer> Renderer { get; private set; }

    /// <summary>
    /// SDL window for the game.
    /// </summary>
    public IntPtr<SDL_Window> Window { get; private set; }

    /// <summary>
    /// Color that will be used to clear the backbuffer each frame.
    /// </summary>
    protected SDL_FColor ClearColor { get; set; } = new() { a = 1 };

    /// <summary>
    /// Backbuffer that game objects will be rendered to.
    /// </summary>
    protected IntPtr<SDL_Texture> Backbuffer { get; private set; }

    /// <summary>
    /// Returns whether the game window is currently focused.
    /// </summary>
    public bool IsFocused { get; private set; }
    
    /// <summary>
    /// Returns whether the game window is currently occluded (that is, completely hidden.)
    /// </summary>
    public bool IsOccluded { get; private set; }

    /// <summary>
    /// Gets or sets the title of the game window.
    /// </summary>
    public string Title
    {
        get;
        set
        {
            field = value;
            if (Window != IntPtr.Zero)
                Window.SetTitle(field);
        }
    } = "";

    /// <summary>
    /// Enables the letterboxed coordinate system.
    /// </summary>
    private void SetLetterboxOn()
    {
        var (w, h) = CanvasSize;
        Renderer.SetLogicalPresentation(
            w, h, SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_LETTERBOX
        );
    }

    /// <summary>
    /// Disables the letterboxed coordinate system.
    /// </summary>
    private void SetLetterboxOff() =>
        Renderer.SetLogicalPresentation(
            0, 0, SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_DISABLED
        );

    /// <summary>
    /// Gets the size, in pixels, of the virtual canvas.
    /// </summary>
    protected virtual (int Width, int Height) CanvasSize => (640, 360);
}