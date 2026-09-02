namespace DevHub.Services.AuthenticationService;
public static class AuthResultMessages
{
    public static string USER_NOT_FOUND  = "User doesn't exist.";
    public static string TOKEN_NOT_FOUND = "User isn't authenticated.";
    public static string USER_IS_BLOCKED = "User is blocked.";
    public static string USER_NOT_AUTHORIZED = "User is not allowed to process.";
}

public static class EntityRetrievalError
{
    public static string EntityNotFound(string entityName)
    {
        return $"{entityName} Not Found";
    }
}