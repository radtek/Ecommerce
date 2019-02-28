using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalEducacional.Models
{
    public class UsuarioPermissoes
    {
        public virtual int UsuarioPermissoesID { get; set; }

        public string Permissao { get; set; }

        public string Controller { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual string ApplicationUserID { get; set; }

        [NotMapped]
        public virtual string Menu { get; set; }

        [NotMapped]
        public virtual string DescricaoTela { get; set; }

        [NotMapped]
        public virtual string Status { get; set; }
    }
}
