using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PortalEducacional.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public virtual string Nome { get; set; }

        public virtual int? PerfilID { get; set; }

        public virtual Perfil Perfil { get; set; }

        public virtual int EscolaID { get; set; }

        public virtual Escola Escola { get; set; }

        public virtual bool Ativo { get; set; }

        public virtual bool Master { get; set; }

        [NotMapped]
        public IList<UsuarioPermissoes> ListaPermissoes { get; set; }

        [NotMapped]
        [Required]
        [StringLength(100, ErrorMessage = "O {0} deve ter no mínimo {2} e no máximo {1} caracteres longos.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirme sua senha")]
        [Compare("Password", ErrorMessage = "A senha e a senha de confirmação não correspondem.")]
        public string ConfirmPassword { get; set; }
    }
}
