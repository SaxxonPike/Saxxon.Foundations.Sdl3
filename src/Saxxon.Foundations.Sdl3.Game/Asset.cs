namespace Saxxon.Foundations.Sdl3.Game;

/// <summary>
/// Typed asset tracker.
/// </summary>
/// <param name="factory">
/// Function that will be used to retrieve asset data when it is needed.
/// </param>
/// <typeparam name="T">
/// Asset type.
/// </typeparam>
internal sealed class Asset<T>(Func<T> factory, bool noExpire) : Asset
    where T : class
{
    /// <summary>
    /// Tracked asset data. This will be either <see cref="WeakReference{T}"/> or T
    /// depending on the load options.
    /// </summary>
    private object? _obj;

    /// <summary>
    /// If true, the next fetch will force a data request.
    /// </summary>
    private bool _invalidated;

    /// <inheritdoc />
    public override object? Fetch(AssetOptions options)
    {
        switch (_obj)
        {
            case WeakReference<T> weakRef:
            {
                if (!_invalidated && weakRef.TryGetTarget(out var target))
                    return target;

                target = factory();

                if (options.DoNotCache)
                    return target;

                weakRef.SetTarget(target);
                _invalidated = false;
                return target;
            }
            case T obj:
            {
                if (!_invalidated)
                    return obj;

                _obj = factory();
                _invalidated = false;
                return obj;
            }
            default:
            {
                _invalidated = false;
                _obj = factory();
                return _obj;
            }
        }
    }

    /// <inheritdoc />
    public override void Invalidate()
    {
        if (!noExpire)
            _invalidated = true;
    }
}

/// <summary>
/// Asset tracker.
/// </summary>
internal abstract class Asset
{
    /// <summary>
    /// Path of the asset.
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    /// Fetches the asset.
    /// </summary>
    /// <param name="options">
    /// Options that affect fetch and cache operations.
    /// </param>
    /// <returns>
    /// The fetched asset, whether from cache or external.
    /// </returns>
    public abstract object? Fetch(AssetOptions options);

    /// <summary>
    /// Explicitly marks the asset for collection.
    /// </summary>
    public abstract void Invalidate();
}