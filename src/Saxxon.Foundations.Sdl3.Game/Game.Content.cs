using System.Collections.Concurrent;

namespace Saxxon.Foundations.Sdl3.Game;

public abstract partial class Game
{
    /// <summary>
    /// While true, the asset cache will be queried when <see cref="LoadAsset{T}"/> is called.
    /// </summary>
    protected virtual bool ShouldEnableAssetCache => true;

    /// <summary>
    /// Cache of loaded assets.
    /// </summary>
    private ConcurrentDictionary<(Type Type, string Path), Asset> _assetCache = [];

    /// <summary>
    /// Loads a game asset of type T.
    /// </summary>
    /// <param name="path">
    /// Path to the game asset.
    /// </param>
    /// <param name="options">
    /// Options that affect the request and cache behavior during load.
    /// </param>
    /// <typeparam name="T">
    /// Type of asset.
    /// </typeparam>
    /// <returns>
    /// The loaded asset.
    /// </returns>
    public T? LoadAsset<T>(string path, AssetOptions options = default)
        where T : class
    {
        if (!options.Force && ShouldEnableAssetCache && _assetCache.TryGetValue((typeof(T), path), out var cached))
            return (T)cached.Fetch(options)!;

        if (!OnAssetRequest(path, out T? result))
        {
            if (_assetCache.TryRemove((typeof(T), path), out cached))
                cached.Invalidate();
            return null;
        }

        if (ShouldEnableAssetCache && !options.DoNotCache)
            SetAsset(path, result, options.DoNotExpire);

        return result;
    }

    /// <summary>
    /// Explicitly sets asset data for a given path.
    /// </summary>
    /// <param name="path">
    /// Asset path.
    /// </param>
    /// <param name="value">
    /// Asset data to set.
    /// </param>
    /// <param name="doNotExpire">
    /// If true, the asset data is loaded once and not eligible for collection.
    /// </param>
    /// <typeparam name="T">
    /// Type of asset.
    /// </typeparam>
    public void SetAsset<T>(string path, T? value, bool doNotExpire = false)
        where T : class
    {
        if (value != null)
            _assetCache[(typeof(T), path)] = new Asset<T>(() => value, doNotExpire);
        else
            _assetCache.Remove((typeof(T), path), out _);
    }

    /// <summary>
    /// Explicitly sets deferred asset data for a given path.
    /// </summary>
    /// <param name="path">
    /// Asset path.
    /// </param>
    /// <param name="valueFunc">
    /// A function that will be used to obtain asset data.
    /// </param>
    /// <param name="doNotExpire">
    /// If true, the asset data is loaded once and not eligible for collection.
    /// </param>
    /// <typeparam name="T">
    /// Type of asset.
    /// </typeparam>
    public void SetAsset<T>(string path, Func<T>? valueFunc, bool doNotExpire = false)
        where T : class
    {
        if (valueFunc != null)
            _assetCache[(typeof(T), path)] = new Asset<T>(valueFunc, doNotExpire);
        else
            _assetCache.Remove((typeof(T), path), out _);
    }

    /// <summary>
    /// Called whenever <see cref="LoadAsset{T}"/> is called and the asset needs
    /// to be fetched (either due to cache expiry or not being present in the cache
    /// at all.)
    /// </summary>
    /// <param name="path">
    /// Asset path that is requested.
    /// </param>
    /// <param name="asset">
    /// When the request is fulfilled, the asset must be stored here.
    /// </param>
    /// <typeparam name="T">
    /// Type of asset being loaded.
    /// </typeparam>
    /// <returns>
    /// True if the asset was loaded (and "asset" is populated.) Otherwise,
    /// false.
    /// </returns>
    /// <remarks>
    /// The default implementation always returns false. In order to use the
    /// content cache feature, this must be overridden.
    /// </remarks>
    protected virtual bool OnAssetRequest<T>(string path, out T? asset)
        where T : class
    {
        asset = null;
        return false;
    }

    /// <summary>
    /// Clears the asset cache completely.
    /// </summary>
    public void ClearAssetCache()
    {
        _assetCache.Clear();
    }
}