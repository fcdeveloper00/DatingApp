using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


[Authorize]
public class UsersController(IUserRepository repo /*, IMapper mapper*/) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await repo.GetMembersAsync();

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

    //public void Visit()
    //{
    //    Console.WriteLine("Peace be uopn Mohammad and his descnedants.");
    //    Console.WriteLine("Hello There");
    //    Console.WriteLine("What are u Talkin' about dude?");
    //    Console.WriteLine();
    //}




}
