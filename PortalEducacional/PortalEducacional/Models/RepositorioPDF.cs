using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class RepositorioPDF
    {
        public int ID { get; set; }
        public string EmailLegal { get; set; }
        public string EmailMae { get; set; }
        public string EmailPai { get; set; }
        public string EmailFinanceiro { get; set; }
        public string Aluno { get; set; }
        public string Escola { get; set; }
        public byte[] PDF { get; set; }
        public string RA { get; set; }


    }
}
