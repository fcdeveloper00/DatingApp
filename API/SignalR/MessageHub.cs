using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IUnitOfWork<IMessageRepository> msgUow,IUnitOfWork<IUserRepository> userUow,
    IMapper mapper, IHubContext<PresenceHub> presenceHub) : Hub
{
    public override async Task OnConnectedAsync()
    {
        /*IClientProxy*/
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"].ToString();
        var username = Context.User?.GetUsernameClaim();
        if (username is null || string.IsNullOrEmpty(otherUser))
            throw new HubException("Cannot Join group");

        string groupName = GetGroupName(username, otherUser);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        Group group = await AddConnectionToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await msgUow.Repository.GetMessageThreadAsync(username, otherUser);

        if (msgUow.HasChanges()) await msgUow.CompleteAsync();
        
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);

    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Group group = await RemoveConnectionFromGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        /*
            var username = Context?.User?.GetUsernameClaim() ?? throw new HubException("Cannot find username");
            var recipientUsername = messageDto.RecipientUsername;

            if (username == recipientUsername) throw new HubException("You can't message yourself");

            var sender  = await userRepo.GetUserByUsernameAsync(username);

            var recipient = await userRepo.GetUserByUsernameAsync(recipientUsername);

            if (sender is null || recipient is null) throw new HubException("The user is not exist");

            var newMessage = new Message
            {
                Content = messageDto.Content,
                RecipientUsername = recipient.UserName,
                SenderUsername = sender.UserName,
                SentAt = DateTime.UtcNow,
            };
        */

        string username = Context.User?.GetUsernameClaim() ?? throw new HubException("Cannot find username");

        if (username == createMessageDto.RecipientUsername.ToLower())
            throw new HubException("You cannot message youself");

        var fetchedSender = await userUow.Repository.GetUserByUsernameAsync(username);
        var fetchedRecipient = await userUow.Repository.GetUserByUsernameAsync(createMessageDto.RecipientUsername.ToLower());

        if (fetchedSender == null || fetchedRecipient is null || fetchedSender.UserName == null || fetchedRecipient.UserName == null)
            throw new HubException("Cannot find Sender or recipient");

        Message newMessage = new()
        {
            Content = createMessageDto.Content,
            SenderUsername = fetchedSender.UserName,
            RecipientUsername = fetchedRecipient.UserName,
            SentAt = DateTime.UtcNow,
            RecipientId = fetchedRecipient.Id,
            SenderId = fetchedSender.Id,
            //Recipient = fetchedRecipient,
            //Sender = fetchedSender
        };

        var groupName = GetGroupName(fetchedSender.UserName, fetchedRecipient.UserName);
        var group = await msgUow.Repository.GetMessageGroupAsync(groupName);

        if (group is not null && group.Connections.Any(c => c.UserName == fetchedRecipient.UserName))// This means the recipient of the message is in the group chat
        {
            newMessage.ReadAt = DateTime.UtcNow;
        }
        else
        {
            var connectionsIds = await PresenceTracker.GetUserConnectionIds(fetchedRecipient.UserName);
            if (connectionsIds is not null && connectionsIds.Count != 0)
            {
                await presenceHub.Clients.Clients(connectionsIds).SendAsync(
                    "NewMessageReceived",
                    new
                    {
                        username = fetchedSender.UserName,
                        knownsAs = fetchedSender.KnownAs
                    });
            }
        }

        msgUow.Repository.AddMessage(newMessage);

        if (await msgUow.CompleteAsync())
        {
            //var groupName = GetGroupName(fetchedSender.UserName, fetchedRecipient.UserName);
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(newMessage));
        }
    }

    private async Task<Group> AddConnectionToGroup(string groupName)
    {

        var username = Context.User?.GetUsernameClaim() ?? throw new HubException("Cannot find username");

        Group? group = await msgUow.Repository.GetMessageGroupAsync(groupName);

        var connection = new Connection
        {
            ConnectionId = Context.ConnectionId,
            UserName = username
        };

        if (group == null)
        {
            group = new Group
            {
                Name = groupName,
            };

            await msgUow.Repository.AddGroupAsync(group);
        }

        group.Connections.Add(connection);

        return (await msgUow.CompleteAsync()) ? group : throw new HubException("Failed to join group");
    }

    private async Task<Group> RemoveConnectionFromGroup()
    {
        Group group = await msgUow.Repository.GetGroupForConnection(Context.ConnectionId)
            ?? throw new HubException("Cannot get group for this chat");

        var connectionToDelete = group.Connections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId)
            ?? throw new HubException("Failed to disconnect");

        msgUow.Repository.RemoveConnection(connectionToDelete);
        return (await msgUow.CompleteAsync()) ? group : throw new HubException("Failed to remove from group.");
    }

    private string GetGroupName(string caller, string other) =>
        (string.CompareOrdinal(caller, other)) switch
        {
            < 0 => $"{caller}-{other}",
            > 0 => $"{other}-{caller}",
            _ => $"{caller}-{other}"
        };

}
