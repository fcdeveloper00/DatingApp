using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController(IUnitOfWork<IMessageRepository> msgUow,IUnitOfWork<IUserRepository> userUow, IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        string username = User.GetUsernameClaim();

        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot message youself");

        var fetchedSender = await userUow.Repository.GetUserByUsernameAsync(username);
        var fetchedRecipient = await userUow.Repository.GetUserByUsernameAsync(createMessageDto.RecipientUsername.ToLower());

        if (fetchedSender == null || fetchedRecipient is null || fetchedSender.UserName == null || fetchedRecipient.UserName == null)
            return BadRequest("Cannot find Sender or recipient");

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

        msgUow.Repository.AddMessage(newMessage);

        if (await msgUow.CompleteAsync())
            return Ok(mapper.Map<MessageDto>(newMessage));

        return BadRequest("Failded to save message");
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetUserMessages([FromQuery] MessageParams @params)
    {
        @params.Username = User.GetUsernameClaim();
        var messages = await msgUow.Repository.GetMessagesForUserAsync(@params);

        Response.AddPaginationHeader(messages);

        return messages;
        //return Ok();
    }

    [HttpGet("thread/{usernameToChatWith}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string usernameToChatWith)
    {
        var currentUsername = User.GetUsernameClaim();

        return Ok(await msgUow.Repository.GetMessageThreadAsync(currentUsername, usernameToChatWith));

    }

    [HttpDelete("{messageId:int}")]
    public async Task<ActionResult> DeleteMessage(int messageId)
    {
        string username = User.GetUsernameClaim();

        Message? fetchedMessage = await msgUow.Repository.getMessageAsync(messageId);

        if (fetchedMessage is null)
            return BadRequest("Cannot delete this message");

        if (username != fetchedMessage.SenderUsername && username != fetchedMessage.RecipientUsername)
            return Forbid();

        if(fetchedMessage.SenderUsername == username)
            fetchedMessage.SenderDeleted = true;

        if(fetchedMessage.RecipientUsername == username)
            fetchedMessage.RecipientDeleted = true;

        if(fetchedMessage is { RecipientDeleted:true , SenderDeleted:true })
            msgUow.Repository.DeleteMessage(fetchedMessage);  


        if (await msgUow.CompleteAsync())
            return Ok();

        return BadRequest("Problem deleting the message");

    }


}
