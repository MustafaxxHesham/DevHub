using DevHub.Utilities;
using System.Security.Claims;

namespace DevHub.AuthorizationRequirements;

public static class Common
{
    public static int RetrieveUserId(ClaimsPrincipal User, ProtectionHandler _protectionHandler)
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);

        int userId = 0;

        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim?.Value))
        {
            return userId;
        }

        return _protectionHandler.GetRealUserId(userIdClaim?.Value);
    }
}
