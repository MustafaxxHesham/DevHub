using DevHub.API.AuthorizationRequirements;
using DevHub.EFCore;
using DevHub.Middlewares;
using DevHub.Services;
using DevHub.Services.PostsService;
using DevHub.Utilities;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

void ConfigApiBehavior(ApiBehaviorOptions options)
{
    options.InvalidModelStateResponseFactory = x => new BadRequestObjectResult(x.ModelState);
}

builder.Services.AddControllers().ConfigureApiBehaviorOptions(ConfigApiBehavior);

builder.Services.AddCors(setup => setup.AddPolicy("MyOwnPolicy", policyConfig => policyConfig.AddClientSidePolicy()));

builder.Services.AddEntityFrameworkCoreConfigurations(builder.Configuration.GetConnectionString("DefaultConnStr")!);

builder.Services.AddScoped<IAuthorizationHandler, CommentPolicyHandler>();

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("Comment", config => config.AddRequirements(new CommentPolicyRequirement()));
});

//builder.Services.AddHangfire(c => c.UseSqlServerStorage("connStr"));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDataProtection();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, new ConfigJwt(builder.Configuration).Configure);

builder.Services.AddAppConfigurations(builder.Configuration);

builder.Services.AddAppRepositories();

builder.Services.AddAppServices();

builder.Services.AddDataStore();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionsMiddleware>();

app.UseMiddleware<TokenUnusedClearMiddleware>();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.HeadContent = @"
        <script>
            window.addEventListener('DOMContentLoaded', () => {
                const checkUi = setInterval(() => {
                    if (window.ui) {
                        clearInterval(checkUi);

                        // 1. Sort API methods by custom verb order
                        window.ui.getConfigs().operationsSorter = (a, b) => {
                            const order = { get: 1, post: 2, put: 3, patch: 4, delete: 5 };
                            return (order[a.get('method')] || 99) - (order[b.get('method')] || 99);
                        };

                        // 2. Calculate and render total API count
                        const infoSection = document.querySelector('.swagger-ui .info');
                        if (infoSection) {
                            const totalOperations = document.querySelectorAll('.opblock').length;
                            const countBadge = document.createElement('div');
                            countBadge.style.cssText = 'margin-top: 10px; font-weight: bold; font-size: 16px; color: #4990e2;';
                            countBadge.innerHTML = `Total Endpoints: <span>${totalOperations}</span>`;
                            infoSection.appendChild(countBadge);
                        }
                    }
                }, 100);
            });
        </script>";
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("MyOwnPolicy");

//app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();