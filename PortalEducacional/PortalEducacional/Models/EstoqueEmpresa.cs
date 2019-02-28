using System;
using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models
{
    public class EstoqueEmpresa
    {
        public int EstoqueEmpresaID { get; set; }

        public virtual int EscolaID { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual Escola Escola { get; set; }

        public virtual int ProdutoID { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual Produto Produto { get; set; }

        public virtual double Saldo { get; set; }

        [Display(Name = "Qtde. Máxima")]
        public virtual double? QuantidadeMaxima { get; set; }

        [Display(Name = "Qtde. Mínima")]
        public virtual double? QuantidadeMinima { get; set; }

        [Display(Name = "Data Vencimento")]
        public virtual string Validade { get; set; }
    }
}
