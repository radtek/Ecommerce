using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalEducacional.Models
{
    public class ResponsavelFinanceiro
    {
        public int ResponsavelFinanceiroID { get; set; }

        [Display(Name = "Nome do Responsável")]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(15)]
        public string Telefone { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(14, ErrorMessage = "CPF pode conter até 14 caracteres")]
        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [Display(Name = "Data de Cadastro")]
        public string DataCadastro { get; set; }

        public int ResponsavelID { get; set; }
        public Responsavel Responsavel { get; set; }

        [NotMapped]
        public bool Deletado { get; set; }
    }
}
