using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Manter conectado?")]
        public bool RememberMe { get; set; }
    }
}
