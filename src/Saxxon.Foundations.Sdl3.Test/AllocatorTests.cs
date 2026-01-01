using Saxxon.Foundations.Sdl3.Helpers;

namespace Saxxon.Foundations.Sdl3.Test;

[TestFixture]
public class AllocatorTests
{
    [Test]
    public void TestMalloc()
    {
        var ptr = Allocator.Malloc(32);
        Assert.That(Allocator.Total, Is.GreaterThanOrEqualTo(8));
        Assert.That(ptr, Is.Not.Zero);
        Allocator.Free(ptr);
        Assert.That(Allocator.Total, Is.Zero);
    }

    [Test]
    public void TestCalloc()
    {
        var ptr = Allocator.Calloc(8, 8);
        Assert.That(Allocator.Total, Is.GreaterThanOrEqualTo(16));
        Assert.That(ptr, Is.Not.Zero);
        Allocator.Free(ptr);
        Assert.That(Allocator.Total, Is.Zero);
    }
}