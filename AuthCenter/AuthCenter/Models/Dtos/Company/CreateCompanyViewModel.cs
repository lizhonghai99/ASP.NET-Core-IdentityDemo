using System.ComponentModel.DataAnnotations;

namespace AuthCenter.Models.Dtos.Company
{
    public class CreateCompanyViewModel
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
    }
}
