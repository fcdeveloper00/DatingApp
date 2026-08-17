using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);

    Task<Message?> getMessageAsync(int messageId);
    Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams @params);
    Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUsername,string recipientUsername);
    Task<bool> SaveAllAsync();

    Task AddGroupAsync(Group group);
    void RemoveConnection (Connection connection);
    Task<Connection?> GetConnectionAsync(string connectionId);
    Task<Group?> GetMessageGroupAsync(string groupName);
    Task<Group?> GetGroupForConnection(string connectionId);
}
