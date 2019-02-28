using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class Cidade
    {
        public int CidadeId { get; set; }

        public int CodIbge { get; set; }

        public virtual Estado Estado { get; set; }

        public virtual int EstadoId { get; set; }

        [DisplayName("Cidade")]
        public string Nome { get; set; }
    }
}
