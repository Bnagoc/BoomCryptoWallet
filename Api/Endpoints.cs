using Api.Authentication.Endpoints;
using Api.Clients.Endpoints;
using Api.Common.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Api;
public static class Endpoints
{
    private static readonly OpenApiSecurityScheme securityScheme = new()
    {
        Type = SecuritySchemeType.Http,
        Name = JwtBearerDefaults.AuthenticationScheme,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Reference = new()
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };

    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("")
            .AddEndpointFilter<RequestLoggingFilter>()
            .WithOpenApi();

        endpoints.MapAuthenticationEndpoints();
        endpoints.MapUserEndpoints();
    }

    private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/auth")
            .WithTags("Authentication");
            
        endpoints.MapPublicGroup()
            .MapEndpoint<Signup>()
            .MapEndpoint<Login>()
            .MapEndpoint<GenerateRefreshToken>();
    }

    private static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/users")
            .WithTags("Users");

        endpoints.MapPublicGroup()
            .MapEndpoint<CreateUser>();

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<CreateDeposit>()
            .MapEndpoint<CreateWithdraw>()
            .MapEndpoint<GetBalance>();
    }

    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }

    private static RouteGroupBuilder MapAuthorizedGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .RequireAuthorization()
            .WithOpenApi(x => new(x)
            {
                Security = [new() { [securityScheme] = [] }],
            });
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}