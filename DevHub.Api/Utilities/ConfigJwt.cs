using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DevHub.Utilities;
public class ConfigJwt
{
    private readonly IConfiguration _config;
    public ConfigJwt(IConfiguration config)
    {
        _config = config;
    }
    public void Configure(JwtBearerOptions options)
    {
        options.SaveToken = false;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SigningKey"]!)),
            ValidAudience = _config["Jwt:Audience"],
            ValidIssuer = _config["Jwt:Issuer"],
        };
    }
}
