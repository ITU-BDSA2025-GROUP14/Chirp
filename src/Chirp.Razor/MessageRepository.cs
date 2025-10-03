using Chirp.Razor.DTOs;
using Chirp.Razor.Interfaces;

namespace Chirp.Razor;

public class MessageRepository : IMessageRepository
{
    public Task CreateMessage(MessageDTO message)
    {
        throw new NotImplementedException();
    }

    public Task<List<MessageDTO>> ReadMessage(string userName)
    {
        throw new NotImplementedException();
    }

    public Task UpdateMessage(MessageDTO message)
    {
        throw new NotImplementedException();
    }
}