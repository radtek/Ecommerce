using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models.LojaEA
{
    public class LojaPedido:BaseModel
    {
        public int LojaPedidoID { get; set; }
        public int ProdutoID { get; set; }
        public int Quantidade { get; set; }
        public decimal Desconto { get; set; }
        public decimal Valor { get; set; }
    }
}
