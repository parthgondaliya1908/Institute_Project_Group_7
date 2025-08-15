using System.Text;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

using Api.Common.Options;
using Api.Common.Constants;
using Api.Common.States;
using Api.Handlers;

namespace Api.Startup;

public static class AuthInitializer
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IReadOnlyList<Permission> permissions, JwtOptions jwtOptions)
    {
        services.AddJwtAuthentication(jwtOptions);
        services.AddJwtAuthorization(permissions);

        return services;
    }

    private static void AddJwtAuthentication(this IServiceCollection services, JwtOptions jwtOptions)
    {
        AuthenticationBuilder authBuilder = services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = Scheme.ValidJwt.Name;
            x.DefaultChallengeScheme = Scheme.ValidJwt.Name;
            x.DefaultForbidScheme = Scheme.ValidJwt.Name;
        });

        authBuilder.AddJwtBearer(Scheme.ValidJwt.Name, options => ConfigureJwtBearer(options, jwtOptions, validateLifetime: true));
        authBuilder.AddJwtBearer(Scheme.ExpiredJwt.Name, options => ConfigureJwtBearer(options, jwtOptions, validateLifetime: false));
    }

    private static void AddJwtAuthorization(this IServiceCollection services, IReadOnlyList<Permission> permissions)
    {
        services.AddAuthorization(options =>
        {
            options.BuildAndAddPolicy(Scheme.ValidJwt.Name, Policy.RequireValidJwt.Name);
            options.BuildAndAddPolicy(Scheme.ExpiredJwt.Name, Policy.AllowExpiredJwt.Name);
            
            foreach (Permission permission in permissions)
            {
                options.BuildAndAddPolicy(Scheme.ValidJwt.Name, permission.Name, isPermission: true);
            }

            options.DefaultPolicy = options.GetPolicy(Policy.RequireValidJwt.Name)!;
        });
    }

    private static JwtBearerOptions ConfigureJwtBearer(JwtBearerOptions bearerOptions, JwtOptions jwtOptions, bool validateLifetime)
    {
        bearerOptions.RequireHttpsMetadata = !jwtOptions.AllowInsecureHttp;
        bearerOptions.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = validateLifetime,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey)),
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = jwtOptions.Issuer
        };

        return bearerOptions;
    }

    private static AuthorizationOptions BuildAndAddPolicy(this AuthorizationOptions authOptions, string schemeName, string policyName, bool isPermission = false)
    {
        AuthorizationPolicyBuilder policyBuilder = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(schemeName)
            .RequireAuthenticatedUser();

        if (isPermission)
        {
            policyBuilder.Requirements.Add(new PermissionRequirement(policyName));
        }

        authOptions.AddPolicy(policyName, policyBuilder.Build());

        return authOptions;
    }
}
