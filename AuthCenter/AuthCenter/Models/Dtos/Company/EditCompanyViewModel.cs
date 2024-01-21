using System.ComponentModel.DataAnnotations;

namespace AuthCenter.Models.Dtos.Company
{
    public class EditCompanyViewModel
    {

        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
    }
}
