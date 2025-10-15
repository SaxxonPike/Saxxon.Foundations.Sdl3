using Shouldly;

namespace Saxxon.Foundations.Sdl3.Test;

[TestFixture]
public class MemTests
{
    [Test]
    public void TestMalloc()
    {
        var ptr = Mem.MallocInternal(32);
        Mem.GetTotalAllocated().ShouldBeGreaterThanOrEqualTo(8);
        ptr.ShouldNotBe(IntPtr.Zero);
        Mem.FreeInternal(ptr);
        Mem.GetTotalAllocated().ShouldBe(0);
    }

    [Test]
    public void TestCalloc()
    {
        var ptr = Mem.CallocInternal(8, 8);
        Mem.GetTotalAllocated().ShouldBeGreaterThanOrEqualTo(16);
        ptr.ShouldNotBe(IntPtr.Zero);
        Mem.FreeInternal(ptr);
        Mem.GetTotalAllocated().ShouldBe(0);
    }
}