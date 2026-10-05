using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskApi.Authorization;
using TaskApi.Data;
using TaskApi.Domain;
using TaskApi.Services;
using TaskApi.Services.Auth;
using TaskApi.Services.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddControllers();

builder
    .Services
    .AddOpenApi();

builder
    .Services
    .AddDbContext<AppDbContext>(
        options =>
        {
            options.UseNpgsql(
                builder
                    .Configuration
                    .GetConnectionString(
                        "TaskApiDatabase"));
        });

builder
    .Services
    .Configure<JwtOptions>(
        builder.Configuration.GetSection(
            JwtOptions.SectionName));

var jwtOptions =
    builder.Configuration
        .GetSection(JwtOptions.SectionName)
        .Get<JwtOptions>()
    ?? throw new InvalidOperationException(
        "JWT configuration is missing.");

if (string.IsNullOrWhiteSpace(jwtOptions.Key))
{
    throw new InvalidOperationException(
        "JWT key is missing.");
}

builder
    .Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                jwtOptions.Key)),

                    ClockSkew = TimeSpan.Zero
                };
        });

builder
    .Services
    .AddHttpContextAccessor();

builder
    .Services
    .AddScoped<
        ICurrentUserService,
        CurrentUserService>();

builder
    .Services
    .AddAuthorization(options =>
    {
        options.AddPolicy(
            Policies.AdminOnly,
            policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(Roles.Admin);
            });
    });

builder
    .Services
    .AddScoped<ITaskService, TaskService>();

builder
    .Services
    .AddScoped<
        ITaskQueryBuilder,
        TaskQueryBuilder>();

builder
    .Services
    .AddScoped<
        IAuthService,
        AuthService>();

builder
    .Services
    .AddScoped<
        IJwtTokenService,
        JwtTokenService>();

builder
    .Services
    .AddScoped<
        IPasswordHasher<User>,
        PasswordHasher<User>>();

builder
    .Services
    .AddProblemDetails(
        options =>
        {
            options.CustomizeProblemDetails =
                context =>
                {
                    context
                        .ProblemDetails
                        .Extensions["traceId"] =
                        context
                            .HttpContext
                            .TraceIdentifier;
                };
        });

builder
    .Services
    .AddExceptionHandler<
        TaskApi.Errors.GlobalExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }

