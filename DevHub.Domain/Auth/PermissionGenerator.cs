namespace DevHub.Domain.Auth;
public static class PermissionGenerator
{
    public static string[] GeneratePermissions(string module)
    {
        return new [] {
            $"{module}.Permission.Create", 
            $"{module}.Permission.Update",
            $"{module}.Permission.Delete",
            $"{module}.Permission.Read"
        };
    }
}