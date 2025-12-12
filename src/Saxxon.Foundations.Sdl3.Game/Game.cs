using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;

namespace Saxxon.Foundations.Sdl3.Game;

/// <summary>
/// Base class for game, window and render operations.
/// </summary>
[PublicAPI]
public abstract partial class Game : IDisposable
{
    /// <summary>
    /// Function that will be used to log SDL messages.
    /// </summary>
    private LogOutputFunction? _logOutputFunction;

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _closeToken.Dispose();
        _logOutputFunction?.Dispose();
        OnDispose();
    }
}