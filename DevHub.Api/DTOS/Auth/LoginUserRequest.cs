using System.ComponentModel.DataAnnotations;

namespace DevHub.DTOS.Auth;
public record LoginUserRequest([EmailAddress] string Email, string Password) { }
/*    public record LoginUserRequest([EmailAddress]string Email,
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$\r\n")]
    [MinLength(10)]string Password)*/