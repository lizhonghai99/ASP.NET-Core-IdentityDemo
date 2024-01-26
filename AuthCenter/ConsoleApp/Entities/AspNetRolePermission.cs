using ConsoleApp.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthCenter.Entities
{
    public partial class AspNetRolePermission
    {
        public string RoleId { get; set; }
        public AspNetRole Role { get; set; }

        public string PermissionId { get; set; }

        public AspNetPermission Permission { get; set; }

    }
}
