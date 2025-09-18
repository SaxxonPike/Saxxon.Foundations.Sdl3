using Saxxon.Foundations.Sdl3.Interop;
using Shouldly;

namespace Saxxon.Foundations.Sdl3.Test;

[TestFixture]
public class UserDataStoreTests
{
    [Test]
    public void TestAll()
    {
        var obj0 = new object();
        const string obj1 = "string 1";
        const string obj2 = "string 2";
        const string obj3 = "string 2";

        // Nonexistent entries return false.

        UserDataStore.TryGet<object>(new IntPtr(-1), out _).ShouldBeFalse();
        UserDataStore.TryGet<string>(new IntPtr(-1), out _).ShouldBeFalse();

        // Adding the same object multiple times returns different user data
        // each time.

        var userData0 = UserDataStore.Add(obj0);
        var userData1 = UserDataStore.Add(obj1);
        var userData2 = UserDataStore.Add(obj2);
        var userData3 = UserDataStore.Add(obj3);

        userData0.ShouldNotBe(IntPtr.Zero);
        userData1.ShouldNotBe(IntPtr.Zero);
        userData2.ShouldNotBeOneOf(IntPtr.Zero, userData1);
        userData3.ShouldNotBeOneOf(IntPtr.Zero, userData1, userData2);

        // Retrieving each object should work.

        UserDataStore.TryGet<object>(userData0, out var result0).ShouldBeTrue();
        UserDataStore.TryGet<string>(userData1, out var result1).ShouldBeTrue();
        UserDataStore.TryGet<string>(userData2, out var result2).ShouldBeTrue();
        UserDataStore.TryGet<string>(userData3, out var result3).ShouldBeTrue();

        result0.ShouldBe(obj0);
        result1.ShouldBe(obj1);
        result2.ShouldBe(obj2);
        result3.ShouldBe(obj3);

        UserDataStore.Get<object>(userData0).ShouldBe(result0);
        UserDataStore.Get<string>(userData1).ShouldBe(result1);
        UserDataStore.Get<string>(userData2).ShouldBe(result2);
        UserDataStore.Get<string>(userData3).ShouldBe(result3);

        // Removing an object should make it no longer available.

        UserDataStore.Remove(userData0);
        UserDataStore.TryGet<object>(userData0, out _).ShouldBeFalse();

        // Removing an object should not affect other objects.

        UserDataStore.Remove(userData1);
        UserDataStore.TryGet<string>(userData1, out _).ShouldBeFalse();
        UserDataStore.Get<string>(userData2).ShouldBe(obj2);
        UserDataStore.Get<string>(userData3).ShouldBe(obj3);

        // Removing all objects of a given type should make them unavailable.

        UserDataStore.RemoveAll<object>();
        UserDataStore.RemoveAll<string>();

        UserDataStore.TryGet<object>(userData0, out result0).ShouldBeFalse();
        UserDataStore.TryGet(userData1, out result1).ShouldBeFalse();
        UserDataStore.TryGet(userData2, out result2).ShouldBeFalse();
        UserDataStore.TryGet(userData3, out result3).ShouldBeFalse();
    }
}