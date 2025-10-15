using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace SDL;

// ReSharper disable InconsistentNaming
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

/// <summary>
/// Represents two-dimensional size.
///
/// This is not an actual SDL type.
/// </summary>
[PublicAPI]
public struct SDL_Size
{
    public int w;
    public int h;
}