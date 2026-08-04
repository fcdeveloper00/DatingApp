namespace API.Entities;

public class UserLike
{
    //The user that doing the like action.
    public AppUser SourceUser { get; set; } = null!;

    public int SourceUserId { get; set; }

    // The user that recieving the like.
    public AppUser TargetUser { get; set; } = null!;

    public int TargetUserId { get; set; }

}
