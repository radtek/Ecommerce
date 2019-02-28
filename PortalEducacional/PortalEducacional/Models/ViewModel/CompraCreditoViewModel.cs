using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models.ViewModel
{
    public class CompraCreditoViewModel
    {
        public Aluno Aluno { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public FaturamentoCartao FaturamentoCartao { get; set; }

        public AlunoProdutos AlunoProdutos { get; set; }

        public PedidoVendaCredito PedidoVendaCredito { get; set; }

        public virtual string Imagem { get; set; }

        [NotMapped]
        public IList<Produto> ListaProdutosAluno { get; set; }

        [NotMapped]
        public IList<Categoria> ListaCategorias { get; set; }

        [NotMapped]
        public virtual string BandeiraID { get; set; }

        [NotMapped]
        public virtual decimal? SaldoDisponivel { get; set; }

        [NotMapped]
        public virtual decimal? TotalCredito { get; set; }

        [NotMapped]
        public virtual bool MaisAlunos { get; set; }
    }
}
