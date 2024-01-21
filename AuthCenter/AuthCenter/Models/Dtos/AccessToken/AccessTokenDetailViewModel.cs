using System.ComponentModel.DataAnnotations;

namespace AuthCenter.Models.Dtos.AccessToken
{
    public class AccessTokenDetailViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Application")]
        public string ApplicationName { get; set; }

        [Display(Name = "Token Name")]
        public string TokenName { get; set; }

        public string Token { get; set; }

        public string Secret { get; set; }

        public bool InActive { get; set; }

        public string Role { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
