using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Test;

[TestFixture]
public class UnmanagedStringTests
{
    [Test]
    public unsafe void TestWithChars()
    {
        using var str = new UnmanagedString("test123");
        Assert.That(((IntPtr<byte>)str.Ptr).GetString(), Is.EqualTo("test123"));
    }

    [Test]
    public unsafe void TestWithBytes()
    {
        using var str = new UnmanagedString("test123"u8);
        Assert.That(((IntPtr<byte>)str.Ptr).GetString(), Is.EqualTo("test123"));
    }
}