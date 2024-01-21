using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthCenter.Entities
{
    [Table(nameof(Company))]
    public class Company
    {
        public Company() => Id = Guid.NewGuid().ToString();

        [Key]
        [MaxLength(450)]
        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        [MaxLength(450)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        [MaxLength(450)]
        public string? UpdatedBy { get; set; }
    }
}
