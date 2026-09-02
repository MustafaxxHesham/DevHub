using System.Text.Json.Serialization;
namespace DevHub.DTOS.Auth;
public record LoginUserResponse
{
    /// Required Response Data.
    /// (Id, AccessToken, RefreshToken, UserProfileImageUrl, FullName[First+ +Last])
    public int Id { get; set; }
    public string FullName { get; set; }
    public string AccessToken { get; set; }
    public string ProfileImageUrl { set; get; }
    [JsonIgnore]
    public string RefreshToken { set; get; }
    public List<string> Roles { set; get; }
}
