using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.OpenApi.Validations;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers;

// Comment this out

//rgb(6,22,28);
//rgb(14,89,164);
public class AccountController(UserManager<AppUser> userManager, ITokenService tokenservice, IMapper mapper) : BaseApiController
{
    #region Register

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> RegisterUser(RegisterDto registerDto)
    {

        if (await CheckDuplicateUsername(registerDto.Username))
        {
            return BadRequest("Username is taken.");
        }


        AppUser newUser = mapper.Map<AppUser>(registerDto);
        newUser.UserName = registerDto.Username.ToLower();
        /*
        newUser.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        newUser.PasswordSalt = hmac.Key;*/

        var result = await userManager.CreateAsync(newUser, registerDto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return new UserDto
        {
            Username = newUser.UserName,
            Token = await tokenservice.CreateTokenAsync(newUser),
            Gender = newUser.Gender,
            KnownAs = newUser.KnownAs,
        };

        #region other return statements
        //return Ok(newUser);
        //return await Task.FromResult(newUser);
        #endregion
    }

    #endregion

    #region Login

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        //byte[] hashedPassword = GetPasswordHash(loginDto.Password);

        var fetchedUser = await userManager.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(
                u => u.UserName!.ToLower() == loginDto.Username.ToLower()
            );

        if (fetchedUser == null || fetchedUser.UserName == null)
            return Unauthorized("Invalid username");

        /*using var hmac = new HMACSHA512(fetchedUser.PasswordSalt);
        byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != fetchedUser.PasswordHash[i])
                return Unauthorized("Invalid Password");
        }
*/

        return new UserDto
        {
            Username = fetchedUser.UserName,
            Token = await tokenservice.CreateTokenAsync(fetchedUser),
            PhotoUrl = fetchedUser.Photos.FirstOrDefault(p => p.IsMain)?.Url,
            Gender = fetchedUser.Gender,
            KnownAs = fetchedUser.KnownAs,
        };
    }

    #endregion

    /*

    The float property was introduced to allow web developers to implement layouts involving an image floating inside a column of text,
    with the text wrapping around the left or right of it. The kind of thing you might get in a newspaper layout.

    But web developers quickly realized that you can float anything, not just images, so the use of float broadened, 
    for example, to fun layout effects such as drop-caps.

    Floats have commonly been used to create entire website layouts featuring multiple columns of information floated 
    so they sit alongside one another (the default behavior would be for the columns to sit below one another 
    in the same order as they appear in the source). There are newer, better layout techniques available. 
    Using floats in this way should be regarded as a legacy technique.

    In this article we'll just concentrate on the proper uses of floats.



     */



    //static byte[] GetPasswordHash(string password)
    //{
    //    using var hmac = new HMACSHA512();
    //    return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    //}
    async Task<bool> CheckDuplicateUsername(string username)
    {
        return await userManager.Users.AnyAsync(u => u.NormalizedUserName == username.ToUpper());
    }



}

public struct Nectar
{

}
