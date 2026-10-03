namespace DevHub.DTOS.Auth;

public record AddUserResponse(int UserId, string Token, string UserName,
    CancellationToken CancellationToken = default);