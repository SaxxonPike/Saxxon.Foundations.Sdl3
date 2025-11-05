using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Delegates;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="MIX_Group"/>.
/// </summary>
[PublicAPI]
public static class Group
{
    public static unsafe void Destroy(
        this IntPtr<MIX_Group> group
    )
    {
        MIX_DestroyGroup(group);
    }

    public static unsafe IntPtr<MIX_Mixer> GetMixer(
        this IntPtr<MIX_Group> group
    )
    {
        return ((IntPtr<MIX_Mixer>)MIX_GetGroupMixer(group))
            .AssertSdlNotNull();
    }

    public static unsafe SDL_PropertiesID GetProperties(
        this IntPtr<MIX_Group> group
    )
    {
        var result = MIX_GetGroupProperties(group);
        return result == 0 ? throw new SdlException() : result;
    }

    public static unsafe void SetPostMixCallback(
        this IntPtr<MIX_Group> group,
        GroupMixCallback? callback
    )
    {
        if (callback == null)
        {
            MIX_SetGroupPostMixCallback(group, null, 0)
                .AssertSdlSuccess();
        }
        else
        {
            MIX_SetGroupPostMixCallback(
                group,
                GroupMixCallback.Callback,
                callback.UserData
            ).AssertSdlSuccess();
        }
    }
}