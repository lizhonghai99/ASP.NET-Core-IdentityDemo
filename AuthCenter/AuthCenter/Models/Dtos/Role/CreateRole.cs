using System.ComponentModel.DataAnnotations;

namespace AuthCenter.Models.Dtos.Role
{
    public class CreateRole
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string NormalizedName { get; set; }
    }
}
