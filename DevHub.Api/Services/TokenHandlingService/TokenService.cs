using DevHub.Domain.Result;
using Microsoft.AspNetCore.DataProtection;
using System.Collections.Concurrent;
namespace DevHub.Services.TokenHandlingService;

public class TokenService : ITokenService
{
    private const string _PURPOSE = "DAS;LXJASXMKL2EJ'1";
    private readonly IDataProtector _protector;
    public TokenService(IDataProtectionProvider _provider) => _protector = _provider.CreateProtector(_PURPOSE); 
    private readonly ConcurrentDictionary<string, string> _tokens = new ConcurrentDictionary<string, string>();
    private DateTime _lastAccess = DateTime.Now;
    public DateTime GetLastAccess() => _lastAccess;
    public void ClearNotUsedTokens() => _tokens.Clear();
    public Result<string> EnsureTokenForgotPassword(string userEmail, string userComingToken, bool isFirstAccess)
    {
        _lastAccess = DateTime.Now;
        // Ensure Existence, Ensure Equality.
        if (_tokens.ContainsKey(userEmail))
        {
            var appToken = _tokens[userEmail];
            if (appToken.Equals(userComingToken))
            {
                var time = DateTime.Parse(appToken.Split(":")[1]);
                if (time.AddMinutes(1) >= DateTime.Now && isFirstAccess)
                {
                    string newlyGeneratedToken = GetToken(userEmail);
                    _tokens[userEmail] = newlyGeneratedToken;
                    return Result<string>.Success(newlyGeneratedToken);
                }
                else if(time.AddMinutes(4) >= DateTime.Now && !isFirstAccess)
                {
                    if(_tokens.TryRemove(userEmail, out string val))
                    {
                        return Result<string>.Success(string.Empty);
                    }
                    return Result<string>.Failure(TokenMessages.Error_In_Object_Only.ToString());
                }
                return Result<string>.Failure(TokenMessages.Token_Time_Excceded.ToString());
            }
            return Result<string>.Failure(TokenMessages.Tokens_Not_Equals.ToString());
        }
        return Result<string>.Failure(TokenMessages.Token_Not_Exist.ToString());
    }
    public Result<string> EnsureTokenVerifiedEmail(string userEmail, string userComingToken)
    {
        var originalToken = _protector.Unprotect(userComingToken);
        Console.WriteLine(originalToken);
        _lastAccess = DateTime.Now;
        if (_tokens.ContainsKey(userEmail))
        {
            var appToken = _tokens[userEmail];
            if (appToken.Equals(originalToken))
            {
                var time = DateTime.Parse(appToken.Split("|")[1]);
                if (time.AddMinutes(8) >= DateTime.Now)
                {
                    if (_tokens.Remove(userEmail, out string val))

                    return Result<string>.Success(string.Empty);
                }
                return Result<string>.Failure(TokenMessages.Token_Time_Excceded.ToString());
            }
            return Result<string>.Failure(TokenMessages.Tokens_Not_Equals.ToString());
        }
        return Result<string>.Failure(TokenMessages.Token_Not_Exist.ToString());
    }
    public string GetToken(string userEmail)
    {
        _lastAccess = DateTime.Now;

        string token = Guid.NewGuid().ToString() + "|" + DateTime.Now.ToString();
        _tokens.AddOrUpdate(userEmail, token, (key,val) =>
        {
            _tokens[key] = val;
            return val;
        });
        token = _protector.Protect(token);  
        return token;
    }
}
