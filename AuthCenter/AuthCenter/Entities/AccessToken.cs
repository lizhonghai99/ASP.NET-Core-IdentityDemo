using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthCenter.Entities
{
    [Table(nameof(AccessToken))]
    public class AccessToken
    {
        [Key]
        [MaxLength(450)]
        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string ApplicationName { get; set; }

        [Required]
        [MaxLength(256)]
        public string TokenName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Token { get; set; }

        [Required]
        [MaxLength(256)]
        public string Secret { get; set; }

        public bool InActive { get; set; }

        [Required]
        [MaxLength(450)]
        public string RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public IdentityRole Role { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        [MaxLength(450)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        [MaxLength(450)]
        public string? UpdatedBy { get; set; }

    }
}
