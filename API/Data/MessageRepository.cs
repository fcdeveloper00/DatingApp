using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace API.Data;

public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
{
    public async void AddMessage(Message message)
    {
        await context.Messages.AddAsync(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }


    public async Task<Message?> getMessageAsync(int messageId)
    {
        return await context.Messages.FindAsync(messageId);
    }



    public async Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams @params)
    {
        IQueryable<Message> query = context.Messages
            .OrderByDescending(m => m.SentAt)
            .AsQueryable();

        query = @params.Container switch
        {
            "Inbox" => query.Where(m => m.RecipientUsername == @params.Username && m.RecipientDeleted == false),
            "Outbox" => query.Where(m => m.SenderUsername == @params.Username && m.SenderDeleted == false),
            _ => query.Where(m => m.RecipientUsername == @params.Username && m.ReadAt == null && m.RecipientDeleted == false), // Unread Messages
        };

        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.CreateAsync(messages, @params.PageNumber, @params.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUsername, string recipientUsername)
    {
        var messages = await context.Messages
            //.Include(m => m.Sender).ThenInclude(u => u.Photos)
            //.Include(m => m.Recipient).ThenInclude(u => u.Photos)
            .Where(m => m.SenderUsername == currentUsername && m.SenderDeleted == false && m.RecipientUsername == recipientUsername ||
                m.RecipientUsername == currentUsername && m.RecipientDeleted == false && m.SenderUsername == recipientUsername)
            .OrderBy(m => m.SentAt)
            .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        var unreadMessages = messages.Where(
            m => m.ReadAt == null && m.RecipientUsername == currentUsername).ToList();

        if (unreadMessages.Count > 0)
        {
            //unreadMessages.ForEach(m =>
            //{
            //    m.ReadAt = DateTime.UtcNow;
            //    Console.WriteLine(m.Content);
            //});

            await context.Messages.ForEachAsync(m =>
            {
                if(m.ReadAt == null && m.RecipientUsername == currentUsername)
                    m.ReadAt = DateTime.UtcNow;
            });
            await context.SaveChangesAsync();
        }

        return messages;
    }


    public async Task<bool> SaveAllAsync() => await context.SaveChangesAsync() > 0;


    #region Group-Connection

    public async Task AddGroupAsync(Group group)
    {
        await context.Groups.AddAsync(group);
    }
    public void RemoveConnection(Connection connection)
    {
        context.Connections.Remove(connection);
    }
    public async Task<Group?> GetMessageGroupAsync(string groupName)
    {
        return await context.Groups
            .Include(g => g.Connections)
            .FirstOrDefaultAsync(g => g.Name == groupName);
    }

    public async Task<Connection?> GetConnectionAsync(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }

    public async Task<Group?> GetGroupForConnection(string connectionId)
    {
        return await context.Groups
            .Include(g => g.Connections)
            .Where(g => g.Connections.Any(c => c.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    #endregion
}

