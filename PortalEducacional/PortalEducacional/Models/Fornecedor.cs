using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class Fornecedor
    {
        public int FornecedorID { get; set; }

        [StringLength(100, ErrorMessage = "Nome pode conter até 100 caracteres")]
        [Required(ErrorMessage = "Nome é requerido")]
        [Display(Name = "Nome do Fornecedor")]
        public string Nome { get; set; }

        [StringLength(18, ErrorMessage = "CNPJ pode conter até 18 caracteres")]
        [Display(Name = "Cnpj do Fornecedor")]
        public string Cnpj { get; set; }

        public string Telefone { get; set; }

        public string Email { get; set; }

        [Display(Name = "Data de Cadastro")]
        public string DataCadastro { get; set; }

        [StringLength(200, ErrorMessage = "Descrição pode conter até 200 caracteres")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Fornecedor esta Ativo?")]
        public bool Ativo { get; set; }

        public int EnderecoID { get; set; }

        public virtual Endereco Endereco { get; set; }

        public virtual IList<EscolaFornecedor> EscolaFornecedor { get; set; }

        public virtual int EscolaFornecedorID { get; set; }
    }
}
