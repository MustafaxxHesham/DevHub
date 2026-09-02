namespace DevHub.DTOS.Auth;
public record EmailVerificationRequest(string email, string token);