
using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveChangesAsync();
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    Task<MemberDto?> GetMemberByUsernameAsync(string username);
    Task<MemberDto?> GetMemberByIdAsync(int id);
    Task<IEnumerable<MemberDto>> GetMembersAsync();


}
