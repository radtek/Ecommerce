using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalEducacional.Models
{
    public class CaixaMovimento
    {
        public int CaixaMovimentoID { get; set; }

        public int CaixaFechamentoID { get; set; }
        public virtual CaixaFechamento CaixaFechamento { get; set; }

        public int? PedidoID { get; set; }
        public virtual Pedido Pedido { get; set; }

        public int? SangriaCaixaID { get; set; }
        public virtual SangriaCaixa SangriaCaixa { get; set; }

        public virtual string AspNetUsersID { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual double Valor { get; set; }

        [Display(Name = "Data de Cadastro")]
        public string DataCadastro { get; set; }

        [NotMapped]
        public IList<Pedido> ListaPedidos { get; set; }
    }
}
