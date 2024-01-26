using AuthCenter.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AuthCenter.Services
{
    public class PermissionService
    {
        private readonly IAuthorizationPolicyProvider _policyProvider;
        private readonly IServiceProvider _serviceProvider;

        public PermissionService(IAuthorizationPolicyProvider policyProvider, IServiceProvider serviceProvider)
        {
            _policyProvider = policyProvider;
            _serviceProvider = serviceProvider;
        }

        public List<string> GetPermissions()
        {
            var permissions = new List<string>();

            var controllerTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ControllerBase)));

            foreach (var controllerType in controllerTypes)
            {
                var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);

                foreach (var method in methods)
                {
                    var authorizeAttributes = method.GetCustomAttributes<AuthorizeAttribute>();
                    foreach (var authorizeAttribute in authorizeAttributes)
                    {
                        permissions.Add(authorizeAttribute.Policy);
                    }
                }
            }

            return permissions.Distinct().ToList();
        }


        public async Task InitializePermissions()
        {
           
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var systemPermissions = GetPermissions();
                var savedPermissions = await dbContext.Permission.ToListAsync();
                var permissionsToAdd = systemPermissions.Except(savedPermissions.Select(p => p.Name)).ToList();
                var permissionsToRemove = savedPermissions.Where(p => !systemPermissions.Contains(p.Name)).ToList();


                foreach (var permission in permissionsToAdd)
                {
                    await dbContext.Permission.AddAsync(new Entities.Permission { Name = permission });
                }

                foreach (var permission in permissionsToRemove)
                {
                    dbContext.Permission.Remove(permission);
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
