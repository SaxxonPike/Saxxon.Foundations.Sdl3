using System.Threading.Channels;

namespace Saxxon.Foundations.Sdl3.Game;

public abstract partial class Game
{
    /// <summary>
    /// SDL_EventType that is used within SDL to indicate game events.
    /// </summary>
    private SDL_EventType _messageEventType;

    /// <summary>
    /// Queue used for pending game events.
    /// </summary>
    private readonly Channel<(Guid Id, object Content)> _pendingMessages =
        Channel.CreateUnbounded<(Guid, object)>();

    /// <summary>
    /// Enqueues a game message. A unique <see cref="Guid"/> is generated
    /// and will be provided when the game message is later processed.
    /// </summary>
    /// <param name="content">
    /// Content of the game message.
    /// </param>
    /// <returns>
    /// The ID generated for the game message.
    /// </returns>
    public Guid SendMessage(object content)
    {
        //
        // There are two components to a message:
        // - the message content
        // - the SDL user event
        //
        // When a new message is created, the object is added to the message channel
        // and a new SDL user event is added to the SDL event queue. For each
        // SDL user event of this type that is later dequeued, the message content
        // is read from the message channel. This ensures that the messages are
        // processed in the order that they were enqueued, and that messages are
        // only processed as part of the SDL lifecycle.
        //

        var id = Guid.NewGuid();
        _pendingMessages.Writer.TryWrite((Id: id, Content: content));

        var ev = new SDL_Event
        {
            user =
            {
                type = (uint)_messageEventType,
                timestamp = Time.GetNowNanoseconds(),
                windowID = Window.GetId()
            }
        };

        ev.Push();

        return id;
    }
}