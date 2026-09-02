using DevHub.Domain.Result;
using DevHub.Domain.DataStoreContract;
using DevHub.DTOS.Auth;
using DevHub.Services.AuthenticationService;
using DevHub.Services.EmailNotfiticationService;
using DevHub.Services.TokenHandlingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DevHub.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController(IAuthService _authService, ILogger<AuthController> _logger, IDataStore _dataStore, IEmailService _emailService, ITokenService _tokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<Result<LoginUserResponse>>> SignIn(LoginUserRequest credentials)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.AuthenticateUserAsync(credentials);

        if (result.IsSuccess)
        {
            AddTokenToCookie(result.Value.RefreshToken);
            return Ok(result);
        }

        return BadRequest(result.Error);
    }

    [HttpPost("register")]
    public async Task<ActionResult> SignUp(AddUserRequest userDataRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.SaveUserAsync(userDataRequest);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var token = _tokenService.GetToken(userDataRequest.Email);

        var vToken = new UserVerificationToken(userDataRequest.Email, token);

        await _emailService.SendEmailVerificationMailAsync(vToken);

        return Ok();
    }

    [HttpPost("signout")]
    public async Task<ActionResult> SignOut(string email, [FromHeader(Name = "X-RefreshToken")] string refreshToken)
    {
        //error here.
        if ((await _authService.IsEmailVerifiedAsync(email)).Value != null)
        {
            var result = await _authService.BlackListTokenAsync(refreshToken);
            if (!result.IsSuccess)
                _logger.LogCritical($"User {email} | has problem with black listing the refresh token {refreshToken} ");
            return NoContent();
        }
        return NotFound();
    }

    [HttpPost("access-reset-password")]
    public async Task<ActionResult> GrantAccessToResetPasswordPage(EmailVerificationRequest request)
    {
        var isEmailExists = await _authService.IsEmailExistedAsync(request.email);

        if (!isEmailExists.IsSuccess)
            return NotFound(isEmailExists.Error);

        var result = _tokenService.EnsureTokenForgotPassword(request.email, request.token, true);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.Error);
    }

    [HttpPost("handle-forgot-password")]
    public async Task<ActionResult> HandleForgotPassword(HandleForgotPasswordRequest request)
    {
        if (!await _dataStore.Users.IsEmailExistAsync(request.Email))
            return NotFound("Email isn't existed");

        var result = _tokenService.EnsureTokenForgotPassword(request.Email, request.Token, false);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        throw new NotImplementedException();
    }

    [HttpPost("extrenal-login")]
    public async Task<ActionResult> ExternalLogin() 
    {
        throw new NotImplementedException();
    }

    [HttpGet("request-email-verification")]
    public async Task<ActionResult<string>> GetEmailVerification([EmailAddress] string email, [FromServices] IConfiguration config)
    {
        var result = await _authService.IsEmailVerifiedAsync(email);

        if (!result.IsSuccess && (result.Error.Equals("Email isn't existed.") || result.Error.Equals("Email is already verified.")))
            return BadRequest(result.Error);

        var token = _tokenService.GetToken(email);

        var host = config.GetValue<string>("ASPNETCORE_URLS")!.Split(";")[0];

        var url = Url.ActionLink(nameof(VerifyEmail), values: new { email, token });

        var userVerificationToken = new UserVerificationToken(email, url!);

        await _emailService.SendEmailVerificationMailAsync(userVerificationToken);

        return Ok(url);
    }

    [HttpGet("VerifyEmail")]
    public async Task<ActionResult<Result<string>>> VerifyEmail(EmailVerificationRequest request)
    {
        var result = _tokenService.EnsureTokenVerifiedEmail(request.email, request.token);

        if (result.IsSuccess)
        {
            var verificationResult = await _authService.VerifyEmailAsync(request.email);

            if (verificationResult.IsSuccess)
                return NoContent();

            return BadRequest(verificationResult.Error);
        }
        return BadRequest(result.Error);
    }

    [HttpGet("forgot-password")]
    public async Task<ActionResult> ForgotPassword(string email)
    {
        if (!await _dataStore.Users.IsEmailExistAsync(email))
            return NotFound("Email isn't existed");

        var token = _tokenService.GetToken(email);

        var verificationToken = new UserVerificationToken(email, token);

        await _emailService.SendEmailForgotPasswordCodeAsync(verificationToken);

        return Ok();
    }

    [HttpGet("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult> RenewRefreshToken()
    {
        var refreshToken = Request.Cookies["X-RefreshToken"];

        var result = await _authService.AuthenticateByRefreshTokenAsync(refreshToken);

        if (result.IsSuccess)
        {
            AddTokenToCookie(result.Value.RefreshToken);
            return Ok(result.Value);
        }

        if (result.Error == AuthResultMessages.USER_NOT_FOUND)
            return BadRequest(result.Error);

        return Unauthorized(result.Error);
    }




    //  Refresh Token <> Login generates(Access Token, Refresh Token, Profile Image, FullName)
/*
    [HttpPost("custom-register")]
    public async Task<ActionResult> CustomRegister([FromBody] AddUserRequest request)
    {
        if (string.IsNullOrEmpty(request.FirstName))
            return BadRequest();

        var result = await _authService.SaveUserAsync(request);

        var loginDto = new LoginUserRequest(request.Email, request.PasswordHashed);

        var authenticateUser = await _authService.AuthenticateUserAsync(loginDto);

        if (authenticateUser.IsSuccess)
        {
            AddTokenToCookie(authenticateUser.Value.RefreshToken);
            return Ok(authenticateUser.Value);
        }

        return StatusCode(501);
    }


    [HttpGet("Check")]
    [Authorize]
    public ActionResult Done() => Ok(new { result = "No Problem!" });
*/

    private void AddTokenToCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.ToLocalTime().AddHours(1)
        };
        Response.Cookies.Append("X-RefreshToken", refreshToken, cookieOptions);
    }
}
