using System;
using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models
{
    public class Funcionario
    {
        public int FuncionarioID { get; set; }

        [Display(Name = "Nome do Funcionário")]
        [StringLength(100)]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        [Display(Name = "Data de Cadastro")]
        public string DataCadastro { get; set; }

        public virtual int CargoId { get; set; }

        public virtual int EscolaId { get; set; }

        public virtual Cargo Cargo { get; set; }

        public virtual Escola Escola { get; set; }

        public virtual bool Master { get; set; }
    }
}
