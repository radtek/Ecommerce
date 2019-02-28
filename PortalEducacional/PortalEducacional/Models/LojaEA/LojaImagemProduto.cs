using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models.LojaEA
{
    public class LojaImagemProduto:BaseModel
    {
        public int LojaImagemProdutoID { get; set; }
        public byte[] Imagem { get; set; }
        public string Descricao { get; set; }

        public int ProdutoID { get; set; }
        public virtual Produto Produto { get; set; }
    }
}
