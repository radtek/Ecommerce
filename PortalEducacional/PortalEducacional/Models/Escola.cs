using PortalEducacional.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalEducacional.Models
{
    public class Escola
    {
        public int EscolaID { get; set; }

        [StringLength(100, ErrorMessage = "Nome pode conter até 100 caracteres")]
        [Required(ErrorMessage = "Nome é requerido")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [StringLength(19, ErrorMessage = "CNPJ pode conter até 14 caracteres")]
        [Required(ErrorMessage = "CNPJ é requerido")]
        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }

        [Display(Name = "Data de Cadastro")]
        public string DataCadastro { get; set; }

        [StringLength(15, ErrorMessage = "O Telefone pode conter até 15 caracteres")]
        [Required(ErrorMessage = "Telefone é requerido")]
        public string Telefone { get; set; }

        [StringLength(100, ErrorMessage = "O E-mail pode conter até 100 caracteres")]
        public string Email { get; set; }

        public bool Ativo { get; set; }

        public virtual int EnderecoId { get; set; }

        public virtual Endereco Endereco { get; set; }

        public virtual IList<EscolaFornecedor> EscolaFornecedor { get; set; }

        public virtual IList<Funcionario> Funcionarios { get; set; }

        public virtual IList<Produto> Produtos { get; set; }
    }
}
