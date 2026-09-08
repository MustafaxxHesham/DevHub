using Microsoft.AspNetCore.Cors.Infrastructure;

namespace DevHub.Utilities;

public static class ClientSidePolicy
{
    extension(CorsPolicyBuilder policyBuilder)
    {
        public CorsPolicyBuilder AddClientSidePolicy()
        {
            return policyBuilder.WithOrigins(["http://localhost:4200"])
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
        }

    }

}
