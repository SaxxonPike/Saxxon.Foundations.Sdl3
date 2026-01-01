using Saxxon.Foundations.Sdl3.Interop;

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

        Assert.That(UserDataStore.TryGet<object>(new IntPtr(-1), out _), Is.False);
        Assert.That(UserDataStore.TryGet<string>(new IntPtr(-1), out _), Is.False);

        // Adding the same object multiple times returns different user data
        // each time.

        var userData0 = UserDataStore.Add(obj0);
        var userData1 = UserDataStore.Add(obj1);
        var userData2 = UserDataStore.Add(obj2);
        var userData3 = UserDataStore.Add(obj3);

        Assert.That(userData0, Is.Not.Zero);
        Assert.That(userData1, Is.Not.Zero);
        Assert.That(userData2, Is.Not.AnyOf(IntPtr.Zero, userData1));
        Assert.That(userData3, Is.Not.AnyOf(IntPtr.Zero, userData1, userData2));

        // Retrieving each object should work.

        Assert.That(UserDataStore.TryGet<object>(userData0, out var result0), Is.True);
        Assert.That(UserDataStore.TryGet<object>(userData1, out var result1), Is.True);
        Assert.That(UserDataStore.TryGet<object>(userData2, out var result2), Is.True);
        Assert.That(UserDataStore.TryGet<object>(userData3, out var result3), Is.True);

        Assert.That(result0, Is.SameAs(obj0));
        Assert.That(result1, Is.SameAs(obj1));
        Assert.That(result2, Is.SameAs(obj2));
        Assert.That(result3, Is.SameAs(obj3));

        Assert.That(UserDataStore.Get<object>(userData0), Is.SameAs(result0));
        Assert.That(UserDataStore.Get<object>(userData1), Is.SameAs(result1));
        Assert.That(UserDataStore.Get<object>(userData2), Is.SameAs(result2));
        Assert.That(UserDataStore.Get<object>(userData3), Is.SameAs(result3));

        // Removing an object should make it no longer available.

        UserDataStore.Remove(userData0);
        Assert.That(UserDataStore.TryGet<object>(userData0, out _), Is.False);

        // Removing an object should not affect other objects.

        UserDataStore.Remove(userData1);
        Assert.That(UserDataStore.TryGet<string>(userData1, out _), Is.False);
        Assert.That(UserDataStore.Get<string>(userData2), Is.SameAs(obj2));
        Assert.That(UserDataStore.Get<string>(userData3), Is.SameAs(obj3));

        // Removing all objects of a given type should make them unavailable.

        UserDataStore.RemoveAll<object>();
        UserDataStore.RemoveAll<string>();

        Assert.That(UserDataStore.TryGet<object>(userData0, out result0), Is.False);
        Assert.That(UserDataStore.TryGet(userData1, out result1), Is.False);
        Assert.That(UserDataStore.TryGet(userData2, out result2), Is.False);
        Assert.That(UserDataStore.TryGet(userData3, out result3), Is.False);
    }
}