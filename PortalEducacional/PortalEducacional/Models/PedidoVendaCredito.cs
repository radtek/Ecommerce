namespace PortalEducacional.Models
{
    public class PedidoVendaCredito
    {
        public virtual int PedidoVendaCreditoID { get; set; }

        public virtual string ApplicationUserID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual string Numero { get; set; }

        public virtual string Data { get; set; }

        public virtual decimal Valor { get; set; }

        public virtual int FaturamentoCartaoID { get; set; }
        public virtual FaturamentoCartao FaturamentoCartao { get; set; }

        public virtual int AlunoID { get; set; }
        public virtual Aluno Aluno { get; set; }
    }
}
