using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
