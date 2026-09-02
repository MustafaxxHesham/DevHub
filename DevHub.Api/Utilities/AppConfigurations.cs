using DevHub.Options;

namespace DevHub.Utilities;

public static class AppConfigurations
{
    extension(IServiceCollection _services)
    {
        public IServiceCollection AddAppConfigurations(IConfiguration config)
        {
            _services.Configure<JwtOptions>(config.GetSection("Jwt"));
            _services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
            return _services;
        }
    }

}
