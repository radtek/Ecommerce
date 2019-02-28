using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models
{
    public class CaixaFechamento
    {
        public int CaixaFechamentoID { get; set; }

        public int EscolaID { get; set; }
        public virtual Escola Escola { get; set; }

        public virtual string AspNetUsersID { get; set; }

        [Display(Name = "Data de Cadastro")]
        public string DataCadastro { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual double SaldoInicial { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual double SaldoFinal { get; set; }

        public virtual bool Fechado { get; set; }
    }
}
