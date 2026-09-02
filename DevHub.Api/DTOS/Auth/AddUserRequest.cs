namespace DevHub.DTOS.Auth;
public record AddUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PasswordHashed { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
    public IFormFile? ProfileImage { get; set; }
}