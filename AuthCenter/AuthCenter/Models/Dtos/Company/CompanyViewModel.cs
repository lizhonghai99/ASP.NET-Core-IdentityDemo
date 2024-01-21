using System.ComponentModel.DataAnnotations;

namespace AuthCenter.Models.Dtos.Company
{
    public class CompanyViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
