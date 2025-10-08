
using Chirp.Razor.DTOs;

namespace Chirp.Razor.Interfaces;

public interface IMessageRepository
{
    public Task CreateMessage(MessageDTO message);
    public Task<List<MessageDTO>> ReadMessage(string userName);
    public Task UpdateMessage(MessageDTO message);

}