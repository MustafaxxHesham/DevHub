using DevHub.Domain.Result;
namespace DevHub.Services.TokenHandlingService;
public interface ITokenService
{
    DateTime GetLastAccess();
    string GetToken(string userEmail);
    void ClearNotUsedTokens();
    Result<string> EnsureTokenForgotPassword(string userEmail, string userComingToken, bool isFirstAccess);
    Result<string> EnsureTokenVerifiedEmail(string userEmail, string userComingToken);
}