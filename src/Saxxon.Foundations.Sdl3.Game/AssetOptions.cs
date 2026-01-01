namespace Saxxon.Foundations.Sdl3.Game;

/// <summary>
/// Options that affect asset loading.
/// </summary>
public struct AssetOptions
{
    /// <summary>
    /// If true, any asset that is requested is not cached.
    /// </summary>
    public bool DoNotCache { get; set; }

    /// <summary>
    /// If true, any asset that is requested will persist through
    /// garbage collection, even if it is no longer referenced.
    /// </summary>
    public bool DoNotExpire { get; set; }

    /// <summary>
    /// If true, the cache will not be checked for the asset. This
    /// guarantees that the asset is requested.
    /// </summary>
    public bool Force { get; set; }
}