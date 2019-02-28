
namespace PortalEducacional.Models
{
    public class SetupEscola
    {
        public int SetupEscolaID { get; set; }

        public virtual int? PonteiroPedido { get; set; }

        public virtual int EscolaID { get; set; }

        public Escola Escola { get; set; }
    }
}
