using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace API.Controllers;


[Authorize]
public class UsersController(IUserRepository repo, IMapper mapper/*IPhotoService photoService*/) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams @params)
    {
        PagedList<MemberDto> users = await repo.GetMembersAsync(@params);

        Console.WriteLine(@params.Gender);

        @params.CurrentUsername = User.GetUsernameClaim();

        Response.AddPaginationHeader(users);

        return Ok(users);
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<MemberDto>> GetUser(int id)
    {
        var user = await repo.GetMemberByIdAsync(id);
        if (user is null)
            return NotFound();

        return user;
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await repo.GetMemberByUsernameAsync(username);
        if (user is null)
            return NotFound();


        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        string? username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username == null)
            return BadRequest("Username was not supplied."); // No username found in token

        AppUser? fetchedUser = await repo.GetUserByUsernameAsync(username);

        if (fetchedUser is null)
            return BadRequest("No User was Found with the given username."); // Could not find user

        mapper.Map(memberUpdateDto, fetchedUser);

        //repo.Update(fetchedUser);

        if (await repo.SaveAllAsync())
            return NoContent();

        return BadRequest("Faild to update the user.");
    }


    //public void Visit()
    //{
    //    Console.WriteLine("Peace be uopn Mohammad and his descnedants.");
    //    Console.WriteLine("Hello There");
    //    Console.WriteLine("What are u Talkin' about dude?");
    //    Console.WriteLine();
    //}

    #region Add Photo
    /*
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            AppUser? fetchedUser = await repo.GetUserByUsernameAsync(User.GetUsernameClaim());

            if (fetchedUser is null)
                return BadRequest("Cannot update user.");

            ImageUploadResult result = await photoService.AddPhotoAsync(file);

            if (result.Error != null)
                return BadRequest($"Error: {result.Error.Message}");

            Photo photo = new()
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                //IsMain = false,
            };

            if(!fetchedUser.Photos.Any()) // Or .Count()
                photo.IsMain = true;

            fetchedUser.Photos.Add(photo);

            if (await repo.SaveChangesAsync())
                //return CreatedAtAction(nameof(GetUser),new { username = fetchedUser.Username },mapper.Map<PhotoDto>(photo));

                return mapper.Map<PhotoDto>(photo);

            return BadRequest("Oops! There's a Problem with adding new photo");

        } 
    */
    #endregion

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var fetchedUser = await repo.GetUserByUsernameAsync(User.GetUsernameClaim());

        if (fetchedUser is null)
            return BadRequest("Could not find user");

        var photo = fetchedUser.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo is null) return BadRequest("Invalid photo ID.");

        if (photo.IsMain) return BadRequest("The photo is already the MAIN photo.");

        var currentMainPhoto = fetchedUser.Photos.FirstOrDefault(p => p.IsMain);

        if (currentMainPhoto is not null)
        {
            currentMainPhoto.IsMain = false;
        }
        photo.IsMain = true;

        if (await repo.SaveAllAsync()) return NoContent();

        return BadRequest("Problem setting main photo");

    }


    [HttpDelete("delete/photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhotoAsync(int photoId)
    {
        var fetchedUser = await repo.GetUserByUsernameAsync(User.GetUsernameClaim());

        if (fetchedUser is null)
            return BadRequest("Cannot find user.");

        Photo? photo = fetchedUser.Photos.Find(p => p.Id == photoId);

        if (photo is null || photo.IsMain) return NotFound("Such a photo does NOT exist or cannot be deleted.");

        //if(photo.PublicId is not null)
        //{
        //    var result = await photoService.DeletePhotoAsync(photo.PublicId);
        //    if (result.Error is not null) return BadRequest(result.Error.Message);
        //}

        fetchedUser.Photos.Remove(photo);

        if (await repo.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting photo");
    }

}
                                  