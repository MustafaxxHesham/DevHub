using Microsoft.AspNetCore.Cors.Infrastructure;

namespace DevHub.Utilities;

public static class ClientSidePolicy
{
    extension(CorsPolicyBuilder policyBuilder)
    {
        public CorsPolicyBuilder AddClientSidePolicy()
        {
            return policyBuilder.WithOrigins(["https://localhost:4200"])
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
        }

    }

}
