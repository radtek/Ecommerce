using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "O {0} deve ter no mínimo {2} e no máximo {1} caracteres longos.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme nova senha")]
        [Compare("NewPassword", ErrorMessage = "A nova senha e a senha de confirmação não coincidem.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }
}
