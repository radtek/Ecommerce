using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models
{
    public class SangriaCaixa
    {
        public int SangriaCaixaID { get; set; }

        //public int CaixaMovimentoID { get; set; }
        //public virtual CaixaMovimento CaixaMovimento { get; set; }

        public virtual string AspNetUsersID { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual double Valor { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual string Motivo { get; set; }

        [Display(Name = "Data de Cadastro")]
        public string DataCadastro { get; set; }
    }
}
