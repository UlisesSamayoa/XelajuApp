using Microsoft.AspNetCore.Authorization;

namespace TransferApp.Security
{
    public class PermissionAttribute : AuthorizeAttribute
    {
        public PermissionAttribute(string permission)
        {
            Policy = permission;
        }
    }
}