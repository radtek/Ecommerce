using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class Biometria
    {
        public Guid BiometriaID { get; set; }
        public int AlunoID { get; set; }
        public int FuncionarioID { get; set; }
        public int? ResponsavelID { get; set; }
        public int EscolaID { get; set; }
        public int Dedo { get; set; }
        public string HashDedo { get; set; }
    }
}
