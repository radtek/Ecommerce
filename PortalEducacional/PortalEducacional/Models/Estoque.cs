using System;
using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models
{
    public class Estoque
    {
        public int EstoqueID { get; set; }

        public virtual int EscolaID { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual Escola Escola { get; set; }

        public virtual int ProdutoID { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual Produto Produto { get; set; }

        public virtual string AspNetUsersID { get; set; }

        public virtual string DataCadastro { get; set; }

        public virtual bool Saida { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual double Quantidade { get; set; }

        [Display(Name = "Histórico da Ação")]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual string Historico { get; set; }

        [Display(Name = "Valor Compra")]
        public virtual double? ValorCompra { get; set; }
    }
}
