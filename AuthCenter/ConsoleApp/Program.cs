using AuthCenter.Entities;
using ConsoleApp.Data;
using ConsoleApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Console.Out.WriteLineAsync("Hello, World!");
            await InitialRoleClaims();
            await Console.Out.WriteLineAsync("Ok");
            await Console.In.ReadLineAsync();
        }

        static async Task InitiaRolePermission()
        {
            using var dbContext = new ApplicationDbContext();
            var permissions = await dbContext.AspNetPermissions.ToListAsync();
            var roles = await dbContext.AspNetRoles.ToListAsync();

            try
            {
                foreach (var role in roles)
                {
                    var rolePermissions = new List<AspNetRolePermission>();
                    foreach (var permission in permissions)
                    {
                        rolePermissions.Add(new AspNetRolePermission() { RoleId = role.Id, PermissionId = permission.Id });
                    }
                    await dbContext.AspNetRolePermissions.AddRangeAsync(rolePermissions);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }

        }

        static async Task InitialUserClaims()
        {
            using var dbContext = new ApplicationDbContext();

            var users = await dbContext.AspNetUsers.Include(t => t.Roles).ToListAsync();
            foreach (var user in users)
            {
                var claim = await dbContext.AspNetUserClaims.Where(t => t.UserId == user.Id && t.ClaimType == "Permissions").FirstOrDefaultAsync();
                if (claim == null)
                {
                    var roleIds = user.Roles.Select(t => t.Id).ToList();
                    var rolePermissionIds = await dbContext.AspNetRolePermissions.Where(t => roleIds.Contains(t.RoleId)).Select(t => t.PermissionId).ToListAsync();
                    var permissions = await dbContext.AspNetPermissions.Where(t => rolePermissionIds.Contains(t.Id)).Select(t => t.Name).ToListAsync();
                    permissions = permissions.Distinct().ToList();
                    var json = System.Text.Json.JsonSerializer.Serialize<List<string>>(permissions);
                    await dbContext.AspNetUserClaims.AddAsync(new Entities.AspNetUserClaim { UserId = user.Id, ClaimType = "Permissions", ClaimValue = json });
                }
            }
            await dbContext.SaveChangesAsync();
        }

        static async Task InitialRoleClaims()
        {
            using var dbContext = new ApplicationDbContext();
            var roles = await dbContext.AspNetRoles.ToListAsync();
            foreach (var role in roles)
            {
                var claim = await dbContext.AspNetRoleClaims.Where(t => t.RoleId == role.Id && t.ClaimType == "Permissions").FirstOrDefaultAsync();
                if (claim == null)
                {

                    var rolePermissionIds = await dbContext.AspNetRolePermissions.Where(t => t.RoleId == role.Id).Select(t => t.PermissionId).ToListAsync();
                    var permissions = await dbContext.AspNetPermissions.Where(t => rolePermissionIds.Contains(t.Id)).Select(t => t.Name).ToListAsync();
                    permissions = permissions.Distinct().ToList();
                    var json = System.Text.Json.JsonSerializer.Serialize<List<string>>(permissions);
                    await dbContext.AspNetRoleClaims.AddAsync(new AspNetRoleClaim { RoleId = role.Id, ClaimType = "Permissions", ClaimValue = json });
                }
            }
            dbContext.SaveChanges();

        }
    }
}
