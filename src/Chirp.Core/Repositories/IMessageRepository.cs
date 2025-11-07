using Chirp.Core.DTO;

namespace Chirp.Core.Repositories;

public interface IMessageRepository
{
    public Task CreateMessage(MessageDto message);
    public Task<List<MessageDto>> ReadMessage(string userName);
    public Task UpdateMessage(MessageDto message);

}