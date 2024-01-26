using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthCenter.Entities
{
    [Table("AspNetRolePermissions")]
    public class RolePermission
    {
        [Key]
        [MaxLength(450)]
        public string RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public IdentityRole Role { get; set; }

        [Key]
        [MaxLength(450)]
        public string PermissionId { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public Permission Permission { get; set; }
    }
}
