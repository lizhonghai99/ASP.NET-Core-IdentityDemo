using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AuthCenter.Services
{
    public class DynamicAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public DynamicAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                var permissionRequirment = new PermissionRequirement(policyName);
                policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(permissionRequirment)
                    .Build();
            }

            return policy;
        }
    }
}
