using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class Perfil
    {
        public int PerfilID { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public int EscolaID { get; set; }

        public virtual Escola Escola { get; set; }

        [NotMapped]
        public IList<PerfilPermissoes> ListaTelas { get; set; }
    }
}
