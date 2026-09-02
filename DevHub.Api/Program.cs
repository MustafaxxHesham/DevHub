using DevHub.Middlewares;
using DevHub.Utilities;
using DevHub.EFCore;
using DevHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDataProtection();

builder.Services.AddAuthentication()
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
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("MyOwnPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();