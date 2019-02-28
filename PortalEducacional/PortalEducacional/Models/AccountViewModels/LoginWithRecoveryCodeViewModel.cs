using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models.AccountViewModels
{
    public class LoginWithRecoveryCodeViewModel
    {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Código de recuperação")]
            public string RecoveryCode { get; set; }
    }
}
