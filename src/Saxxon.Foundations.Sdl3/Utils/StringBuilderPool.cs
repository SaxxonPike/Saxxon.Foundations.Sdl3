using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;

namespace Saxxon.Foundations.Sdl3.Utils;

/// <summary>
/// A pool of <see cref="StringBuilder"/> objects.
/// </summary>
[PublicAPI]
public static class StringBuilderPool
{
    private static readonly ObjectPool<StringBuilder> Pool =
        new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());

    /// <summary>
    /// Rents a <see cref="StringBuilder"/> from the pool.
    /// </summary>
    public static StringBuilder Rent() => Pool.Get();

    /// <summary>
    /// Returns a <see cref="StringBuilder"/> to the pool.
    /// </summary>
    public static void Return(StringBuilder sb) => Pool.Return(sb);
}