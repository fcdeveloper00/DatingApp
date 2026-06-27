using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.OpenApi.Validations;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenservice) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> RegisterUser(RegisterDto registerDto)
    {
        if (await CheckDuplicateUsername(registerDto.Username))
        {
            return BadRequest("Username is taken.");
        }

        using var hmac = new HMACSHA512();
        var newUser = new AppUser
        {
            Username = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        await context.Users.AddAsync(newUser);

        await context.SaveChangesAsync();

        return new UserDto
        {
            Username = newUser.Username,
            Token = tokenservice.CreateToken(newUser)
        };

        #region other return statements
        //return Ok(newUser);
        //return await Task.FromResult(newUser);
        #endregion
    }

    #region Login

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        //byte[] hashedPassword = GetPasswordHash(loginDto.Password);

        var fetchedUser = await context.Users
            .FirstOrDefaultAsync(
                u => u.Username == loginDto.Username.ToLower()
            );

        if (fetchedUser == null)
            return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(fetchedUser.PasswordSalt);
        byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != fetchedUser.PasswordHash[i])
                return Unauthorized("Invalid Password");
        }

        return new UserDto
        {
            Username = fetchedUser.Username,
            Token = tokenservice.CreateToken(fetchedUser)
        };
    }



    #endregion








    //static byte[] GetPasswordHash(string password)
    //{
    //    using var hmac = new HMACSHA512();
    //    return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    //}
    async Task<bool> CheckDuplicateUsername(string username)
    {
        return await context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }



}
