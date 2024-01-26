using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthCenter.Entities
{
    [Table("AspNetPermissions")]
    public class Permission
    {

        public Permission() => Id = Guid.NewGuid().ToString();

        [Key]
        [MaxLength(450)]
        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
    }
}
