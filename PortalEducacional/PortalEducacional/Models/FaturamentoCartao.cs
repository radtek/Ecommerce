using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalEducacional.Models
{
    public class FaturamentoCartao
    {
        public virtual int FaturamentoCartaoID { get; set; }

        public virtual string ApplicationUserID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual string NumeroCartao { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual string DataVencimento { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual string TitularCartao { get; set; }

        public virtual string Bandeira { get; set; }

        public virtual string DataTransacao { get; set; }

        public virtual string PaymentID { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual decimal Valor { get; set; }

        [NotMapped]
        public virtual string Mes { get; set; }

        [NotMapped]
        public virtual string Ano { get; set; }

        [NotMapped]
        public virtual string CVV { get; set; }

        [NotMapped]
        public virtual string Status { get; set; }
    }
}
