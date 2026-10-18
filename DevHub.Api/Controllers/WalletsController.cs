using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/wallets")]
public class WalletsController
{
    [HttpGet("{walletId}")]
    public async Task GetWallet(string walletId)
    {
        throw new NotImplementedException();
    }

    public record class PayForCourseRequest(string FromWalletId, string ToWalletId, string CourseId);

    [HttpPost("/pay")]
    public async Task PayFor(PayForCourseRequest request)
    {
        throw new NotImplementedException();
    }
}