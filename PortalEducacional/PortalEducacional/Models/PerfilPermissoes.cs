using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class PerfilPermissoes
    {
        public int PerfilPermissoesID { get; set; }

        public string Permissao { get; set; }

        public string Controller { get; set; }

        public virtual Perfil Perfil { get; set; }

        public int PerfilID { get; set; }

        [NotMapped]
        public virtual string Menu { get; set; }

        [NotMapped]
        public virtual string DescricaoTela { get; set; }

        [NotMapped]
        public virtual string Status { get; set; }
    }
}
