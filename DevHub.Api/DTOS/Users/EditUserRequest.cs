using System.ComponentModel.DataAnnotations;

namespace DevHub.DTOS.Users;
public class EditUserRequest
{
    public string UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Bio { get; set; }
    [FileExtensions(Extensions = "jpg,jpeg,png")]
    public IFormFile? ProfileImage { get; set; }
}