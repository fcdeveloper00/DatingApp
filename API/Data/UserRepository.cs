using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
    readonly DataContext _context;
    readonly IMapper _mapper;
    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MemberDto?> GetMemberByIdAsync(int id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<MemberDto?> GetMemberByUsernameAsync(string username)
    {
        return await _context.Users
            .Where(u => u.Username == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<MemberDto>> GetMembersAsync()
    {
        return await _context.Users
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        var fetchedUser = await _context.Users.FindAsync(id);
        return fetchedUser;
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        var fetchedUser = await _context.Users
            .Include(u => u.Photos)
            .SingleOrDefaultAsync(u => u.Username == username);
        return fetchedUser;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        var fetchedUsers = await _context.Users
            .Include(u => u.Photos)
            .ToListAsync();
        return fetchedUsers;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }
}
