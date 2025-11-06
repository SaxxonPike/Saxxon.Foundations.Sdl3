using Saxxon.Foundations.Sdl3.Helpers;
using Shouldly;

namespace Saxxon.Foundations.Sdl3.Test;

[TestFixture]
public class AllocatorTests
{
    [Test]
    public void TestMalloc()
    {
        var ptr = Allocator.Malloc(32);
        Allocator.Total.ShouldBeGreaterThanOrEqualTo(8);
        ptr.ShouldNotBe(IntPtr.Zero);
        Allocator.Free(ptr);
        Allocator.Total.ShouldBe(0);
    }

    [Test]
    public void TestCalloc()
    {
        var ptr = Allocator.Calloc(8, 8);
        Allocator.Total.ShouldBeGreaterThanOrEqualTo(16);
        ptr.ShouldNotBe(IntPtr.Zero);
        Allocator.Free(ptr);
        Allocator.Total.ShouldBe(0);
    }
}