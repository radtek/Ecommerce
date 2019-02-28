using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class TipoNutricional
    {
        public int TipoNutricionalID { get; set; }
        public string Nome { get; set; }

        [Display(Name = "Porção")]
        public string UnidadeMedida { get; set; }
    }
}
