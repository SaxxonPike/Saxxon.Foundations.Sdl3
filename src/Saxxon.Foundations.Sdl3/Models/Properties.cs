using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_PropertiesID"/>.
/// </summary>
[PublicAPI]
public static class Properties
{
    /// <summary>
    /// Creates a new property set.
    /// </summary>
    public static SDL_PropertiesID Create()
    {
        return SDL_CreateProperties();
    }

    /// <summary>
    /// Clears a key/value from a property set.
    /// </summary>
    public static void Clear(this SDL_PropertiesID id, ReadOnlySpan<char> name)
    {
        using var nameStr = new Utf8Span(name);
        SDL_ClearProperty(id, nameStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Clears a key/value from a property set.
    /// </summary>
    public static unsafe void Clear(this SDL_PropertiesID id, ReadOnlySpan<byte> name)
    {
        fixed (byte* namePtr = name)
        {
            SDL_ClearProperty(id, namePtr)
                .AssertSdlSuccess();
        }
    }

    /// <summary>
    /// Copies key/value pairs from one property set onto another.
    /// </summary>
    public static void Copy(this SDL_PropertiesID id, SDL_PropertiesID dst)
    {
        SDL_CopyProperties(id, dst)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Frees a property set.
    /// </summary>
    /// <param name="id"></param>
    public static void Destroy(this SDL_PropertiesID id)
    {
        SDL_DestroyProperties(id);
    }

    /// <summary>
    /// Gets all keys present in a property set.
    /// </summary>
    public static unsafe HashSet<string> GetKeys(this SDL_PropertiesID id)
    {
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static void Handler(IntPtr userData, SDL_PropertiesID id, byte* name)
        {
            var set = UserDataStore.Get<HashSet<string>>(userData)!;
            if (Utf8StringMarshaller.ConvertToManaged(name) is { } key)
                set.Add(key);
        }

        var list = new HashSet<string>();
        var ud = UserDataStore.Add(list);

        try
        {
            SDL_EnumerateProperties(id, &Handler, ud);
            return list;
        }
        finally
        {
            if (ud != IntPtr.Zero)
                UserDataStore.Remove(ud);
        }
    }

    /// <summary>
    /// Get the global property set.
    /// </summary>
    public static SDL_PropertiesID GetGlobal()
    {
        return SDL_GetGlobalProperties();
    }

    /// <summary>
    /// Gets a boolean value from a property set, by key.
    /// </summary>
    public static bool GetBoolean(this SDL_PropertiesID id, ReadOnlySpan<char> name, bool defaultValue = false)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_GetBooleanProperty(id, nameStr, defaultValue);
    }

    /// <summary>
    /// Gets a boolean value from a property set, by key.
    /// </summary>
    public static bool GetBoolean(this SDL_PropertiesID id, ReadOnlySpan<byte> name, bool defaultValue = false)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_GetBooleanProperty(id, nameStr, defaultValue);
    }

    /// <summary>
    /// Gets a float value from a property set, by key.
    /// </summary>
    public static float GetFloat(this SDL_PropertiesID id, ReadOnlySpan<char> name, float defaultValue = 0)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_GetFloatProperty(id, nameStr, defaultValue);
    }

    /// <summary>
    /// Gets a float value from a property set, by key.
    /// </summary>
    public static float GetFloat(this SDL_PropertiesID id, ReadOnlySpan<byte> name, float defaultValue = 0)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_GetFloatProperty(id, nameStr, defaultValue);
    }

    /// <summary>
    /// Gets an integer value from a property set, by key.
    /// </summary>
    public static long GetNumber(this SDL_PropertiesID id, ReadOnlySpan<char> name, long defaultValue = 0)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_GetNumberProperty(id, nameStr, defaultValue);
    }

    /// <summary>
    /// Gets an integer value from a property set, by key.
    /// </summary>
    public static long GetNumber(this SDL_PropertiesID id, ReadOnlySpan<byte> name, long defaultValue = 0)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_GetNumberProperty(id, nameStr, defaultValue);
    }

    /// <summary>
    /// Gets a native pointer from a property set, by key.
    /// </summary>
    public static IntPtr GetPointer(this SDL_PropertiesID id, ReadOnlySpan<char> name, IntPtr defaultValue = 0)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_GetPointerProperty(id, nameStr, defaultValue);
    }

    /// <summary>
    /// Gets a native pointer from a property set, by key.
    /// </summary>
    public static IntPtr GetPointer(this SDL_PropertiesID id, ReadOnlySpan<byte> name, IntPtr defaultValue = 0)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_GetPointerProperty(id, nameStr, defaultValue);
    }

    /// <summary>
    /// Gets a string from a property set, by key.
    /// </summary>
    public static string? GetString(this SDL_PropertiesID id, ReadOnlySpan<char> name,
        ReadOnlySpan<char> defaultValue = default)
    {
        using var nameStr = new Utf8Span(name);
        using var defaultStr = new Utf8Span(defaultValue);
        return SDL_GetStringProperty(id, nameStr, defaultStr);
    }

    /// <summary>
    /// Gets a string from a property set, by key.
    /// </summary>
    public static string? GetString(this SDL_PropertiesID id, ReadOnlySpan<byte> name,
        ReadOnlySpan<char> defaultValue = default)
    {
        using var nameStr = new Utf8Span(name);
        using var defaultStr = new Utf8Span(defaultValue);
        return SDL_GetStringProperty(id, nameStr, defaultStr);
    }

    /// <summary>
    /// Gets the type of value stored in a property set, by key.
    /// </summary>
    public static SDL_PropertyType GetType(this SDL_PropertiesID id, ReadOnlySpan<char> name)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_GetPropertyType(id, nameStr);
    }

    /// <summary>
    /// Gets the type of value stored in a property set, by key.
    /// </summary>
    public static SDL_PropertyType GetType(this SDL_PropertiesID id, ReadOnlySpan<byte> name)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_GetPropertyType(id, nameStr);
    }

    /// <summary>
    /// Returns true if a property set contains the specified key.
    /// </summary>
    public static bool Has(this SDL_PropertiesID id, ReadOnlySpan<char> name)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_HasProperty(id, nameStr);
    }

    /// <summary>
    /// Returns true if a property set contains the specified key.
    /// </summary>
    public static bool Has(this SDL_PropertiesID id, ReadOnlySpan<byte> name)
    {
        using var nameStr = new Utf8Span(name);
        return SDL_HasProperty(id, nameStr);
    }

    /// <summary>
    /// Assigns a mutex to a property set.
    /// </summary>
    public static void Lock(this SDL_PropertiesID id)
    {
        SDL_LockProperties(id)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets a boolean value on a property set.
    /// </summary>
    public static void SetBoolean(this SDL_PropertiesID id, ReadOnlySpan<char> name, bool value)
    {
        using var nameStr = new Utf8Span(name);
        SDL_SetBooleanProperty(id, nameStr, value)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets a boolean value on a property set.
    /// </summary>
    public static void SetBoolean(this SDL_PropertiesID id, ReadOnlySpan<byte> name, bool value)
    {
        using var nameStr = new Utf8Span(name);
        SDL_SetBooleanProperty(id, nameStr, value)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets a float value on a property set.
    /// </summary>
    public static void SetFloat(this SDL_PropertiesID id, ReadOnlySpan<char> name, float value)
    {
        using var nameStr = new Utf8Span(name);
        SDL_SetFloatProperty(id, nameStr, value)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets a float value on a property set.
    /// </summary>
    public static void SetFloat(this SDL_PropertiesID id, ReadOnlySpan<byte> name, float value)
    {
        using var nameStr = new Utf8Span(name);
        SDL_SetFloatProperty(id, nameStr, value)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets an integer value on a property set.
    /// </summary>
    public static void SetNumber(this SDL_PropertiesID id, ReadOnlySpan<char> name, long value)
    {
        using var nameStr = new Utf8Span(name);
        SDL_SetNumberProperty(id, nameStr, value)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets an integer value on a property set.
    /// </summary>
    public static void SetNumber(this SDL_PropertiesID id, ReadOnlySpan<byte> name, long value)
    {
        using var nameStr = new Utf8Span(name);
        SDL_SetNumberProperty(id, nameStr, value)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets a native pointer on a property set.
    /// </summary>
    public static void SetPointer(this SDL_PropertiesID id, ReadOnlySpan<char> name, IntPtr value)
    {
        using var nameStr = new Utf8Span(name);
        SDL_SetPointerProperty(id, nameStr, value)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets a native pointer on a property set.
    /// </summary>
    public static void SetPointer(this SDL_PropertiesID id, ReadOnlySpan<byte> name, IntPtr value)
    {
        using var nameStr = new Utf8Span(name);
        SDL_SetPointerProperty(id, nameStr, value)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Sets a native pointer on a property set. When the associated object
    /// is freed, the specified cleanup method is invoked.
    /// </summary>
    private static unsafe void SetPointerWithCleanupInternal(
        this SDL_PropertiesID id,
        Utf8Span name,
        IntPtr value,
        Action<IntPtr> cleanUp
    )
    {
        SDL_SetPointerPropertyWithCleanup(id, name, value, &Handler, UserDataStore.Add(cleanUp))
            .AssertSdlSuccess();
        return;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static void Handler(IntPtr userData, IntPtr val)
        {
            UserDataStore.Get<Action<IntPtr>>(userData)!(val);
            UserDataStore.Remove(userData);
        }
    }

    /// <summary>
    /// Sets a native pointer on a property set. When the associated object
    /// is freed, the specified cleanup method is invoked.
    /// </summary>
    public static void SetPointerWithCleanup(
        this SDL_PropertiesID id,
        ReadOnlySpan<char> name,
        IntPtr value,
        Action<IntPtr> cleanUp
    )
    {
        using var nameStr = new Utf8Span(name);
        SetPointerWithCleanupInternal(id, nameStr, value, cleanUp);
    }

    /// <summary>
    /// Sets a native pointer on a property set. When the associated object
    /// is freed, the specified cleanup method is invoked.
    /// </summary>
    public static void SetPointerWithCleanup(
        this SDL_PropertiesID id,
        ReadOnlySpan<byte> name,
        IntPtr value,
        Action<IntPtr> cleanUp
    )
    {
        using var nameStr = new Utf8Span(name);
        SetPointerWithCleanupInternal(id, nameStr, value, cleanUp);
    }

    /// <summary>
    /// Sets a string value on a property set.
    /// </summary>
    public static void SetString(this SDL_PropertiesID id, ReadOnlySpan<char> name, ReadOnlySpan<char> value)
    {
        using var nameStr = new Utf8Span(name);
        using var valueStr = new Utf8Span(value);

        SDL_SetStringProperty(id, nameStr, valueStr)
            .AssertSdlSuccess();
    }

    /// <summary>
    /// Frees a mutex from a property set.
    /// </summary>
    /// <param name="id"></param>
    public static void Unlock(this SDL_PropertiesID id)
    {
        SDL_UnlockProperties(id);
    }
}