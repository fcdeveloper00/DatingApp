using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace API.Controllers;

public class LikesController(ILikesRepository likesRepo) : BaseApiController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        int currentUserId = User.GetUserIdClaim();

        if (currentUserId == targetUserId) return BadRequest("You cannot like yourself!");

        UserLike? existingLike = await likesRepo.GetUserLike(currentUserId, targetUserId);

        if (existingLike is null)
        {
            var newLike = new UserLike()
            {
                SourceUserId = currentUserId,
                TargetUserId = targetUserId,
            };

            likesRepo.AddLike(newLike);
        }
        else
        {
            likesRepo.DeleteLike(existingLike);
        }

        if (await likesRepo.SaveChangesAsync()) return Ok();

        return BadRequest("Appling like failed.");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds() =>
       Ok(await likesRepo.GetCurrentUserLikeIds(User.GetUserIdClaim()));


    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    { 
        likesParams.UserId = User.GetUserIdClaim();
        var users = await likesRepo.GetUserLikes(likesParams);
        Response.AddPaginationHeader(users);

        return Ok(users);
    }
}
