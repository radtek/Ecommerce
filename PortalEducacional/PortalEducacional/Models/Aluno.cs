using PortalEducacional.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalEducacional.Models
{
    public class Aluno
    {
        public int AlunoID { get; set; }

        [Display(Name = "Nome do Aluno")]
        [StringLength(100)]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Nome { get; set; }

        [Display(Name = "RA")]
        [StringLength(20)]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Ra { get; set; }

        [Display(Name = "Data de Cadastro")]
        public string DataCadastro { get; set; }

        [EnumDataType(typeof(Genero))]
        public Genero TipoGenero { get; set; }

        public bool Ativo { get; set; }

        [Display(Name = "Data de Nascimento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataNascimento { get; set; }

        public int EscolaID { get; set; }

        public virtual Escola Escola { get; set; }

        public int? ResposavelLegalID { get; set; }

        public int? ResposavelFinanceiroID { get; set; }

        [NotMapped]
        [ForeignKey("ResposavelLegalID")]
        public virtual Responsavel ResposavelLegal { get; set; }

        [NotMapped]
        [ForeignKey("ResposavelFinanceiroID")]
        public virtual Responsavel ResposavelFinanceiro { get; set; }

        public virtual IList<Nutricional> Nutricionais { get; set; }

        public int SerieID { get; set; }

        public virtual Serie Serie { get; set; }

        [Display(Name = "Foto do Aluno")]
        public virtual byte[] Foto { get; set; }

        public virtual decimal? SaldoDisponivel { get; set; }

        [NotMapped]
        public virtual string responsavelCpf { get; set; }

        [NotMapped]
        public virtual string SerieTurma { get; set; }

        [NotMapped]
        public virtual string EscolaCnpj { get; set; }
    }

    public enum Genero
    {
        [Description("Feminino")]
        Feminino = 0,
        [Description("Masculino")]
        Masculino = 1
    }

}
