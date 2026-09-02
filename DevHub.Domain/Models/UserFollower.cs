namespace DevHub.Domain.Models;
public class UserFollower
{
    public int FollowedUserId { get; set; }
    public User FollowedUser { get; set; }
    public int FollowerUserId { get; set; }
    public User FollowerUser { get; set; }
    public DateTime DateSince { get; set; }
}
