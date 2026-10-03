namespace DevHub.DTOS.Auth;
public record class UserExternalLogin(string FirstName, string LastName, string PhotoUrl, string Provider);