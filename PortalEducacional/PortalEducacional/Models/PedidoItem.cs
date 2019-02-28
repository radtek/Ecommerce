using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalEducacional.Models
{
    public class PedidoItem
    {
        public int PedidoItemID { get; set; }

        public int PedidoID { get; set; }
        public virtual Pedido Pedido { get; set; }

        public int ProdutoID { get; set; }
        public virtual Produto Produto { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual double Quantidade { get; set; }

        public virtual double Desconto { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual double Valor { get; set; }

        [NotMapped]
        public bool Deletado { get; set; }
    }
}
