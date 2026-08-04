using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context,IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like) => context.Likes.Add(like);

    public void DeleteLike(UserLike like) => context.Likes.Remove(like);

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes.Where(ul => ul.SourceUserId == currentUserId)
            .Select(ul => ul.TargetUserId)
            .ToListAsync();
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId) =>
        await context.Likes.FindAsync([sourceUserId, targetUserId]);

    public async Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams)
    {
        var likesQuery = context.Likes.AsQueryable();
        IQueryable<MemberDto> query;

        switch (likesParams.Predicate)
        {
            case "liked":
                query = likesQuery.Where(ul => ul.SourceUserId == likesParams.UserId).Select(ul => ul.TargetUser)
                   .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            case "likedBy":
                query = likesQuery.Where(ul => ul.TargetUserId == likesParams.UserId).Select(ul => ul.SourceUser)
                      .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            default:
                // Mutual likes
                var currentUserLikedIDs = await GetCurrentUserLikeIds(likesParams.UserId);
                query = likesQuery.Where(
                    ul => ul.TargetUserId == likesParams.UserId &&
                    currentUserLikedIDs.Contains(ul.SourceUserId))
                    .Select(ul => ul.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
        }

        return await PagedList<MemberDto>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<bool> SaveChangesAsync() => await context.SaveChangesAsync() > 0;
        
}
