using AuthCenter.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthCenter.Data;
using Microsoft.AspNetCore.Identity;

namespace AuthCenter.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
        }

        public DbSet<AccessToken> AccessToken { get; set; } = default!;

        public DbSet<Company> Company { get; set; } = default!;

        public DbSet<Permission> Permission { get; set; }

        public DbSet<RolePermission> RolePermission { get; set; }
    }
}
