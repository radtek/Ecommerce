using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models.ViewModel
{
    public class AlunoResponsavelViewModel
    {
        public Aluno Aluno { get; set; }

        public Responsavel ResponsavelLegal { get; set; }

        public Responsavel ResponsavelFinanceiro { get; set; }

        public virtual string Imagem { get; set; }
    }
}
