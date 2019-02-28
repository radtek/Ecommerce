using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models.ViewModel
{
    public class CantinaViewModel
    {
        public Produto Produto { get; set; }

        public Categoria Categoria { get; set; }

        public Pedido Pedido { get; set; }

        public PedidoItem PedidoItem { get; set; }

        public CaixaFechamento CaixaFechamento { get; set; }

        public CaixaMovimento CaixaMovimento { get; set; }

        public SangriaCaixa SangriaCaixa { get; set; }

        public Escola Escola { get; set; }

        public SetupEscola SetupEscola { get; set; }

        public Aluno Aluno { get; set; }

        public Funcionario Funcionario { get; set; }

        public virtual string Imagem { get; set; }

        [NotMapped]
        public IList<Categoria> Categorias { get; set; }

        [NotMapped]
        public IList<Produto> Produtos { get; set; }

        [NotMapped]
        public virtual IList<PedidoItem> PedidoItens { get; set; }

        [NotMapped]
        public virtual IList<CaixaMovimento> ListaCaixaMovimento { get; set; }

        [NotMapped]
        public virtual string ValorTotalBruto { get; set; }

        [NotMapped]
        public virtual string ValorTotalLiquido { get; set; }

        [NotMapped]
        public virtual string ValorTotalDesconto { get; set; }

        [NotMapped]
        public virtual string SaldoCaixaAnterior { get; set; }

        [NotMapped]
        public virtual int QtdePedidos { get; set; }

        [NotMapped]
        public virtual int QtdeSangrias { get; set; }

        [NotMapped]
        public virtual string ValorTotalSangria { get; set; }

        [NotMapped]
        public virtual string TipoCliente { get; set; }

        [NotMapped]
        public virtual string ClienteID { get; set; }
    }
}
