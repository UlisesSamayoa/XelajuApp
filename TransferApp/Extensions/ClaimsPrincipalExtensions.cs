using System.Security.Claims;

namespace TransferApp.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            return user.HasClaim("Permission", permission);
        }
        public static bool HasRole(this ClaimsPrincipal user, string role)
        {
            return user.IsInRole(role);
        }
    }
}