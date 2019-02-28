using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class EscolaFornecedor
    {
        public int EscolaID { get; set; }
        public int FornecedorID { get; set; }

        public virtual Escola Escola { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
    }
}
