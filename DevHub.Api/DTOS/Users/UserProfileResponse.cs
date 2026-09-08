namespace DevHub.DTOS.Users;
public record class UserProfileResponse(
        string FullName,
        string JobTitle,
        string Email,
        string Bio,
        string? ProfileImageUrl,
        int FollowersCount,
        int FollowingCount,
        int PostsViews
);