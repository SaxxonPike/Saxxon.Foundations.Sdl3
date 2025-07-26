using JetBrains.Annotations;

namespace Saxxon.Foundations.Sdl3.Models;

/// <summary>
/// Represents an audio channel.
///
/// This is not an actual SDL_mixer type. It wraps "int" for the purpose of
/// providing the fluent interface.
/// </summary>
[PublicAPI]
// ReSharper disable InconsistentNaming
public enum Mix_ChannelId
{
    Default = -1
}