using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;

namespace Saxxon.Foundations.Sdl3.Utils;

[PublicAPI]
public static class StringBuilderPool
{
    private static readonly ObjectPool<StringBuilder> Pool =
        new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());

    public static StringBuilder Rent() => Pool.Get();
    public static void Return(StringBuilder sb) => Pool.Return(sb);
}