using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class Permissao : System.Attribute
    {
        public string Modulo { get; set; }

        public string Menu { get; set; }

        public string DescricaoTela { get; set; }

        public string Acao { get; set; }

        public string Controller { get; set; }
    }
}