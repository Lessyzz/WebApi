using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApi.Models;

namespace WebApi.ServiceExtensions;

public static class AuthConfiguration
{
    public static IServiceCollection ApplyAuthSetup(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = new JwtConfiguration
        {
            Issuer = configuration["JWT:Issuer"] ?? "",
            Audience = configuration["JWT:Audience"] ?? "",
            Key = configuration["JWT:Key"] ?? ""
        };

        ArgumentException.ThrowIfNullOrEmpty(jwtConfig.Issuer);
        ArgumentException.ThrowIfNullOrEmpty(jwtConfig.Audience);
        ArgumentException.ThrowIfNullOrEmpty(jwtConfig.Key);

        services.AddSingleton(jwtConfig);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Key)),
                    RoleClaimType = "rol"
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["jwtToken"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                     
                        var response = context.AuthenticateFailure?.GetType() == typeof(SecurityTokenExpiredException) 
                            ? new Response("token_expired", 401)
                            : new Response("invalid_token", 401);

                        var json = JsonConvert.SerializeObject(response);

                        return context.Response.WriteAsync(json);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = new Response("forbidden", 403);
                        var json = JsonConvert.SerializeObject(response);

                        return context.Response.WriteAsync(json);
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}

