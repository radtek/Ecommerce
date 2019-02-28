using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class DadoNutricional
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DadoNutricionalID { get; set; }
        public int TipoNutricionalID { get; set; }
        public int ProdutoID { get; set; }
        public decimal Porcao { get; set; }
        public decimal ValorDiario { get; set; }
        public virtual TipoNutricional TipoNutricional { get; set; }
        public virtual Produto Produto { get; set; }
        [NotMapped]
        public bool Deletado { get; set; }
    }
}
