using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models.AccountViewModels
{
    public class LoginWith2faViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "O {0} deve ser pelo menos {2} e no máximo {1} caracteres longos.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Código de Autenticador")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Lembre-se desta máquina")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
