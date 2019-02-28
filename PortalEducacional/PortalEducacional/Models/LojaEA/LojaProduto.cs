using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models.LojaEA
{
    public class LojaProduto:BaseModel
    {
        public int LojaProdutoID { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public IEnumerable<LojaImagemProduto> Imagens { get; set; }
        public int Quantidade { get; set; }
    }
}
