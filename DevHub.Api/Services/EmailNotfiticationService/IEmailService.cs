namespace DevHub.Services.EmailNotfiticationService;
public interface IEmailService
{
    Task SendEmailVerificationMailAsync(UserVerificationToken vToken);
    Task SendEmailForgotPasswordCodeAsync(UserVerificationToken vToken);
}
