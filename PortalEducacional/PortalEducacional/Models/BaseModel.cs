using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class BaseModel
    {
        public string AtualizadoPor { get; set; }

        public DateTime UltimaAtualizacao { get; set; }
    }
}
