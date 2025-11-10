using Chirp.Core.DTO;
using Chirp.Core.Repositories;

namespace Chirp.Infrastructure.Chirp.Repositories;

public class MessageRepository : IMessageRepository
{
    public Task CreateMessage(MessageDto message)
    {
        throw new NotImplementedException();
    }

    public Task<List<MessageDto>> ReadMessage(string userName)
    {
        throw new NotImplementedException();
    }

    public Task UpdateMessage(MessageDto message)
    {
        throw new NotImplementedException();
    }
}