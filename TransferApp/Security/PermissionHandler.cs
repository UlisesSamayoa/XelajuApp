using Microsoft.AspNetCore.Authorization;

namespace TransferApp.Security
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            bool hasPermission = context.User.HasClaim(
                "Permission",
                requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}