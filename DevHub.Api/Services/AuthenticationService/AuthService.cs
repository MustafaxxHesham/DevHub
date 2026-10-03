using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Auth;
using DevHub.Options;
using DevHub.Responses;
using DevHub.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DevHub.Services.AuthenticationService;

public class AuthService(IOptions<JwtOptions> option, IDataStore _dataStore,
                         ILogger<AuthService> _logger, ProtectionHandler _handler,
                         IWebHostEnvironment _env) : IAuthService
{
    private readonly JwtOptions _jwt = option.Value;

    public async Task<Result<LoginUserResponse>> AuthenticateUserAsync(LoginUserRequest request)
    {
        //var password = _protectorPassword.Protect(request.Password);

        var user = await _dataStore.Users.GetByCriteriaFirstAsync(u => u.Email.Equals(request.Email));

//        string pass = _protectorPassword.Unprotect(user.PasswordHashed);

        //if (user is null || !pass.Equals("dadas"))
        //    return Result<LoginUserResponse>.Failure("Email or password is invalid!");

        if (!user.IsActive)
            return Result<LoginUserResponse>.Failure("User can't sign in due to not actived.");

        var refreshToken = user.RefreshTokens?.SingleOrDefault(u => u.IsActive);

        if (refreshToken is null) 
        {
            refreshToken = new RefreshToken
            {
                UserId = user.Id,
                CreatedAt = DateTime.Now.ToLocalTime(),
                ExpiredOn = DateTime.Now.ToLocalTime().AddMinutes(2),
                IsActive = true,
                Token = GenerateRefreshToken()
            };
        }
        await _dataStore.RefreshTokens.AddAsync(refreshToken);

        await _dataStore.CompleteAsync();

        var userlogin = new LoginUserResponse
        {
            RefreshToken = refreshToken.Token,
            FullName = user.Email,
            ProfileImageUrl = user.ProfileImageUrl ?? "",
            AccessToken = GenerateAccessToken(user),  //Mocking
        };

        return Result<LoginUserResponse>.Success(userlogin);
    }
    public async Task<Result<AddUserResponse>> CreateUserAsync(AddUserRequest request)
    {
        if (await _dataStore.Users.IsEmailExistAsync(request.Email!))
        {
            return Result<AddUserResponse>.Failure("Email already exists!");
        }

        var user = await ConvertRequestToUserAsync(request);

        await _dataStore.Users.AddAsync(user);

        await _dataStore.CompleteAsync();
    
        var response = new AddUserResponse(101, "", "");   // Requires Mocking

        return Result<AddUserResponse>.Success(response);
    }
    public async Task<Result<RefreshTokenLoginResponse>> AuthenticateByRefreshTokenAsync(string refreshToken)
    {
        var refreshTokenFromDb = await _dataStore.RefreshTokens
                                .GetByCriteriaFirstAsync(rt => rt.Token == refreshToken && rt.IsActive, ["User"]);

        if (refreshTokenFromDb is null)
        {
            return Result<RefreshTokenLoginResponse>.Failure("Refresh Token doesn't exist."); // UnAuthenticated
        }

        refreshTokenFromDb.IsActive = false;
        refreshTokenFromDb.RevokeOn = DateTime.Now.ToLocalTime();

        var userRefreshToken = new RefreshToken
        {
            CreatedAt = DateTime.UtcNow.ToLocalTime(),
            ExpiredOn = DateTime.UtcNow.ToLocalTime().AddMinutes(200),// updatedBy appsettings.json
            IsActive = true,
            Token = GenerateRefreshToken(),
            UserId = refreshTokenFromDb.UserId
        };

        _dataStore.RefreshTokens.UpdateItem(refreshTokenFromDb);

        await _dataStore.RefreshTokens.AddAsync(userRefreshToken);
        await _dataStore.CompleteAsync();

        var accessToken = GenerateAccessToken(refreshTokenFromDb.User);

        if (string.IsNullOrEmpty(accessToken))
        {
            return Result<RefreshTokenLoginResponse>.Failure("User doesn't exist"); // Bad Request
        }

        var response = new RefreshTokenLoginResponse(accessToken, userRefreshToken.Token);

        return Result<RefreshTokenLoginResponse>.Success(response);
    }
    public async Task<Result<string>> VerifyEmailAsync(string email)
    {
        var user = await _dataStore.Users.GetByEmailAsync(email);

        if (user is null)
        {
            return Result<string>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        if (user.IsEmailVerified)
        {
            return Result<string>.Failure(ResponseMessages.ACTION_ALREADY_DONE);
        }

        user.IsEmailVerified = true;

        await _dataStore.CompleteAsync();

        return Result<string>.Success(string.Empty);
    }
    public async Task<Result<string>> GenerateJwtTokenByUserId(int userId)
    {
        var user = await _dataStore.Users.GetByCriteriaFirstAsync(u => u.Id == userId);

        if (user is null)
            return Result<string>.Failure("User not Found");

        return Result<string>.Success(GenerateAccessToken(user));
    }
    public async Task<SimpleResult<bool>> IsEmailVerifiedAsync(string email)
    {
        var result = await _dataStore.Users.GetByCriteriaFirstAsync(u => u.Email == email);

        if (result is null)
            return SimpleResult<bool>.Failure("Email isn't existed.");

        if (result.IsEmailVerified)
            return SimpleResult<bool>.Failure("Email is already verified.");

        return SimpleResult<bool>.Success(true);
    }
    public async Task<SimpleResult<bool>> BlackListTokenAsync(string refreshToken)
    {
        var result = await _dataStore.RefreshTokens.GetByCriteriaFirstAsync(rt => rt.Token == refreshToken);
        
        result.IsActive = false;
        
        result.RevokeOn = DateTime.Now.ToLocalTime();
        
        _dataStore.RefreshTokens.UpdateItem(result);
        
        bool operationsCount = await _dataStore.CompleteAsync() > 0;
        
        if (operationsCount)
            return SimpleResult<bool>.Success(true);

        return SimpleResult<bool>.Failure("Error Happened!");
    }
    public async Task<SimpleResult<bool>> IsRefreshTokenValidAsync(string refreshToken)
    {
        var dbResult = await _dataStore.RefreshTokens
            .GetByCriteriaFirstAsync(rt => rt.Token.Equals(refreshToken) && !rt.IsExpired && rt.IsActive && rt.RevokeOn < DateTime.UtcNow);

        if (dbResult is null) 
            return SimpleResult<bool>.Failure("Refresh Token isn't available");

        return SimpleResult<bool>.Success(true);
    }
    public async Task<SimpleResult<bool>> IsEmailExistedAsync(string email)
    {
        var result = await _dataStore.Users.IsEmailExistAsync(email);

        if (result)
            return SimpleResult<bool>.Success(true);

        return SimpleResult<bool>.Failure("Email Not Found");
    }
    public async Task<bool> Logout(int userId)
    {
        var refreshToken = await _dataStore.RefreshTokens
                                    .GetByCriteriaFirstAsync(r => r.UserId == userId && r.IsActive);
        if (refreshToken is not null)
        {
            refreshToken.RevokeOn = DateTime.Now.ToLocalTime();
            refreshToken.IsActive = false;
            _dataStore.RefreshTokens.UpdateItem(refreshToken);
        }
        var effectedRows = await _dataStore.CompleteAsync();
        return effectedRows > 0;
    }


    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }
    private string GenerateAccessToken(User user)
    {
        var userId = _handler.GetProtectedUserId(user.Id);
        // Instaniate (tokenHandler, SSK(bytes.signingkey), SC, TDesc, 
        var tokenHandler = new JwtSecurityTokenHandler();

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey));

        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwt.Issuer,
            Expires = DateTime.Now.AddSeconds(_jwt.LifeTime),
            Audience = _jwt.Audience,
            SigningCredentials = signingCredentials,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, Roles.Reader.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName + user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(nameof(SubscriptionPlans), SubscriptionPlans.Free.ToString())   // Mocked
            })
        };
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }
    private async Task<string> UploadImageFileToServerAsync(IFormFile image)
    {
        string imageName = Guid.NewGuid().ToString() + Path.GetFileName(image.FileName);

        string imagePath = Path.Combine(_env.WebRootPath, "Images", "UsersProfileImages", imageName);

        using (var fs = new FileStream(imagePath, FileMode.Create))
            await image.CopyToAsync(fs);

        return imagePath;
    }
    private async Task<User> ConvertRequestToUserAsync(AddUserRequest request)
    {
        return new User
        {
            FirstName = request.FirstName!,
            LastName = request.LastName!,
            Email = request.Email!,
            PasswordHashed = _handler.GetProtectedPassword(request.Password!),
            CreatedAt = DateTime.UtcNow,
            Bio = request.Bio!,
            IsActive = true,
            IsEmailVerified = false,
            ProfileImageUrl = await UploadImageFileToServerAsync(request.ProfileImage!),
            RoleId = (await _dataStore.Roles.GetByCriteriaFirstAsync(r => r.RoleName.Equals(Roles.Reader.ToString()))).Id
        };
    }
}
