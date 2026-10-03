using DevHub.Domain.Result;
using DevHub.DTOS.Auth;

namespace DevHub.Services.AuthenticationService;
public interface IAuthService
{
    Task<Result<LoginUserResponse>> AuthenticateUserAsync(LoginUserRequest request);
    Task<Result<AddUserResponse>> CreateUserAsync(AddUserRequest request);
    Task<Result<RefreshTokenLoginResponse>> AuthenticateByRefreshTokenAsync(string refreshToken);
    Task<SimpleResult<bool>> BlackListTokenAsync(string refreshToken);
    Task<Result<string>> GenerateJwtTokenByUserId(int userId);
    Task<SimpleResult<bool>> IsEmailVerifiedAsync(string email);
    Task<SimpleResult<bool>> IsEmailExistedAsync(string email);
    Task<SimpleResult<bool>> IsRefreshTokenValidAsync(string refreshToken);
    Task<Result<string>> VerifyEmailAsync(string email);
    Task<bool> Logout(int userId);
}