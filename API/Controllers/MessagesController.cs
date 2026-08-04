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
public class MessagesController(IMessageRepository msgRepo, IUserRepository userRepo, IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        string username = User.GetUsernameClaim();

        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot message youself");

        var fetchedSender = await userRepo.GetUserByUsernameAsync(username);
        var fetchedRecipient = await userRepo.GetUserByUsernameAsync(createMessageDto.RecipientUsername.ToLower());

        if (fetchedSender == null || fetchedRecipient is null)
            return BadRequest("Cannot find Sender or recipient");

        Message newMessage = new()
        {
            Content = createMessageDto.Content,
            SenderUsername = fetchedSender.Username,
            RecipientUsername = fetchedRecipient.Username,
            SentAt = DateTime.UtcNow,
            RecipientId = fetchedRecipient.Id,
            SenderId = fetchedSender.Id,
            //Recipient = fetchedRecipient,
            //Sender = fetchedSender
        };

        msgRepo.AddMessage(newMessage);

        if (await msgRepo.SaveAllAsync())
            return Ok(mapper.Map<MessageDto>(newMessage));

        return BadRequest("Failded to save message");
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetUserMessages([FromQuery] MessageParams @params)
    {
        @params.Username = User.GetUsernameClaim();
        var messages = await msgRepo.GetMessagesForUserAsync(@params);

        Response.AddPaginationHeader(messages);

        return messages;
        //return Ok();
    }

    [HttpGet("thread/{usernameToChatWith}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string usernameToChatWith)
    {
        var currentUsername = User.GetUsernameClaim();

        return Ok(await msgRepo.GetMessageThreadAsync(currentUsername, usernameToChatWith));

    }

    [HttpDelete("{messageId:int}")]
    public async Task<ActionResult> DeleteMessage(int messageId)
    {
        string username = User.GetUsernameClaim();

        Message? fetchedMessage = await msgRepo.getMessageAsync(messageId);

        if (fetchedMessage is null)
            return BadRequest("Cannot delete this message");

        if (username != fetchedMessage.SenderUsername && username != fetchedMessage.RecipientUsername)
            return Forbid();

        if(fetchedMessage.SenderUsername == username)
            fetchedMessage.SenderDeleted = true;

        if(fetchedMessage.RecipientUsername == username)
            fetchedMessage.RecipientDeleted = true;

        if(fetchedMessage is { RecipientDeleted:true , SenderDeleted:true })
            msgRepo.DeleteMessage(fetchedMessage);  


        if (await msgRepo.SaveAllAsync())
            return Ok();

        return BadRequest("Problem deleting the message");

    }


}
