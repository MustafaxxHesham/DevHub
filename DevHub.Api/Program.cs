using DevHub.AuthorizationPolicies;
using DevHub.EFCore;
using DevHub.Middlewares;
using DevHub.Services;
using DevHub.Utilities;
using FluentValidation;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//void ConfigApiBehavior(ApiBehaviorOptions options)
//{
//    options.InvalidModelStateResponseFactory = x => new BadRequestObjectResult(x.ModelState);
//}

//.ConfigureApiBehaviorOptions(ConfigApiBehavior);
builder.Services.AddControllers();

builder.Services.AddCors(setup => setup.AddPolicy(AuthPolicies.CORSPolicy, policyConfig => policyConfig.AddClientSidePolicy()));

builder.Services.AddEntityFrameworkCoreConfigurations(builder.Configuration.GetConnectionString("DefaultConnStr")!);

builder.Services.AddAuthorization(x => x.AddAuthorizationPolicies());

builder.Services.AddHangfire(c => 
    c.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
     .UseSimpleAssemblyNameTypeSerializer()
     .UseRecommendedSerializerSettings()
     .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnStr"),
        new SqlServerStorageOptions
        {
            PrepareSchemaIfNecessary = true
        })
);

builder.Services.AddStackExchangeRedisCache(setup =>
{
    setup.Configuration = builder.Configuration["Redis"];
});

builder.Services.AddSignalR();

builder.Services.AddHangfireServer();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddSwaggerGen();

builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("/var/DevHub/data-protection-keys"))
                .SetApplicationName("DevHub");

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
        c.HeadContent = @"<script>
        window.addEventListener('DOMContentLoaded', () => {
            const checkSpec = setInterval(() => {
                if (window.ui && window.ui.specSelectors && window.ui.specSelectors.specJson) {
                    const spec = window.ui.specSelectors.specJson();
                    const paths = spec.get ? spec.get('paths') : spec?.paths;

                    // Ensure the spec paths have actually finished downloading/parsing
                    if (paths && (paths.size > 0 || Object.keys(paths).length > 0)) {
                        clearInterval(checkSpec);

                        // 1. Sort API methods by custom verb order
                        window.ui.getConfigs().operationsSorter = (a, b) => {
                            const order = { get: 1, post: 2, put: 3, patch: 4, delete: 5 };
                            return (order[a.get('method')] || 99) - (order[b.get('method')] || 99);
                        };

                        // 2. Calculate total endpoints directly from the spec data
                        let totalOperations = 0;
                        const pathsObj = paths.toJS ? paths.toJS() : paths;
                        const httpMethods = ['get', 'post', 'put', 'patch', 'delete', 'options', 'head', 'trace'];

                        for (const path in pathsObj) {
                            const methods = pathsObj[path];
                            if (methods) {
                                for (const method in methods) {
                                    if (httpMethods.includes(method.toLowerCase())) {
                                        totalOperations++;
                                    }
                                }
                            }
                        }

                        // 3. Render total API count badge once the info section appears
                        const checkInfo = setInterval(() => {
                            const infoSection = document.querySelector('.swagger-ui .info');
                            if (infoSection && !document.getElementById('total-endpoints-badge')) {
                                clearInterval(checkInfo);
                                const countBadge = document.createElement('div');
                                countBadge.id = 'total-endpoints-badge';
                                countBadge.style.cssText = 'margin-top: 10px; font-weight: bold; font-size: 16px; color: #4990e2;';
                                countBadge.innerHTML = `Total Endpoints: <span>${totalOperations}</span>`;
                                infoSection.appendChild(countBadge);
                            }
                        }, 100);
                    }
                }
            }, 100);
        });
    </script>";
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(AuthPolicies.CORSPolicy);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();