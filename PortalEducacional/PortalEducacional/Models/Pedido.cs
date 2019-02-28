using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalEducacional.Models
{
    public class Pedido
    {
        public int PedidoID { get; set; }

        public int EscolaID { get; set; }
        public virtual Escola Escola { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Numero { get; set; }

        [Display(Name = "Data de Cadastro")]
        public string DataCadastro { get; set; }

        public virtual string AspNetUsersID { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual double ValorTotal { get; set; }

        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual double ValorBruto { get; set; }

        public virtual double Desconto { get; set; }

        public int? AlunoID { get; set; }
        public virtual Aluno Aluno { get; set; }

        public int? FuncionarioID { get; set; }
        public virtual Funcionario Funcionario { get; set; }

        public virtual string Observacao { get; set; }

        public virtual string FormaRecebimento { get; set; }

        [NotMapped]
        public virtual IList<PedidoItem> ListaItens { get; set; }
    }
}