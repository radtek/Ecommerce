using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models
{
    public class Cargo
    {
        public int CargoID { get; set; }

        [Display(Name = "Descrição do Cargo")]
        [StringLength(100)]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Nome { get; set; }

        [Display(Name = "Cargo Ativo")]
        public bool Ativo { get; set; }

        public virtual IList<Funcionario> Funcionarios { get; set; }
    }
}
