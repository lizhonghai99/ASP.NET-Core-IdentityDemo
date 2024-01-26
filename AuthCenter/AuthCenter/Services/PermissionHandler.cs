using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthCenter.Services
{
    public class PermissionHandler : IAuthorizationHandler
    {
        private readonly ILogger<PermissionHandler> _logger;


        public PermissionHandler(ILogger<PermissionHandler> logger)
        {
            _logger = logger;
        }


        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var pendingRequirements = context.PendingRequirements.ToList();
            foreach (var requirement in pendingRequirements)
            {
                // Check if the requirement is of type PermissionRequirement
                if (requirement is PermissionRequirement permissionRequirement)
                {
                    // Check if the user has the required permission for the policy
                    if (HasPermission(context.User, permissionRequirement.PermissionName))
                    {
                        context.Succeed(requirement); // User has the required permission
                    }
                    else
                    {
                        _logger.LogWarning($"User does not have the required permission: {permissionRequirement.PermissionName}");
                        context.Fail();
                    }
                }
            }

            return Task.CompletedTask;
        }

        private bool HasPermission(ClaimsPrincipal user, string permissionName)
        {
            var permissionClaim = user.Claims.FirstOrDefault(c => c.Type == "Permissions");
            if (permissionClaim == null)
            {
                return false;
            }

            try
            {
                var permissions = System.Text.Json.JsonSerializer.Deserialize<List<string>>(permissionClaim.Value);

                return permissions != null && permissions.Contains(permissionName);
            }
            catch
            {
                return false;
            }
        }
    }
}
