using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models
{
    public class Serie
    {
        public int SerieID { get; set; }

        [Display(Name = "Série")]
        public string Descricao { get; set; }

        public virtual IList<Aluno> Alunos { get; set; }
    }
}

