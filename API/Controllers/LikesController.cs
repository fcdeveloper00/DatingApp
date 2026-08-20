using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace API.Controllers;

public class LikesController(IUnitOfWork<ILikesRepository> uow) : BaseApiController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        int currentUserId = User.GetUserIdClaim();

        if (currentUserId == targetUserId) return BadRequest("You cannot like yourself!");

        UserLike? existingLike = await uow.Repository.GetUserLike(currentUserId, targetUserId);

        if (existingLike is null)
        {
            var newLike = new UserLike()
            {
                SourceUserId = currentUserId,
                TargetUserId = targetUserId,
            };

            uow.Repository.AddLike(newLike);
        }
        else
        {
            uow.Repository.DeleteLike(existingLike);
        }

        if (await uow.CompleteAsync()) return Ok();

        return BadRequest("Appling like failed.");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds() =>
       Ok(await uow.Repository.GetCurrentUserLikeIds(User.GetUserIdClaim()));


    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    { 
        likesParams.UserId = User.GetUserIdClaim();
        var users = await uow.Repository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(users);

        return Ok(users);
    }
}
