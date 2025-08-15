using Microsoft.AspNetCore.Authorization;

using Api.Common.States;
using Api.Common.Constants;

namespace Api.Handlers;

public record PermissionRequirement(string Permission) : IAuthorizationRequirement;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        IEnumerable<string> permissions = context.User.Claims
            .Where(c => c.Type == Claim.Permission.Name)
            .Select(c => c.Value);

        if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase) ||
            permissions.Contains(Permission.All.Name, StringComparer.OrdinalIgnoreCase)
        )
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
