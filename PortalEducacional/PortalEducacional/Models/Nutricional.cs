using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models
{
    public class Nutricional
    {
        public int NutricionalID { get; set; }

        [Display(Name = "Peso do Aluno (Kg)")]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Peso { get; set; }

        [Display(Name = "Altura do Aluno (cm)")]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Altura { get; set; }

        [Display(Name = "Resultado do Calc. de IMC")]
        public string Resultado { get; set; }

        [Display(Name = "Data de Cadastro")]
        public string DataCadastro { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(200)]
        public string Descricao { get; set; }

        public int AlunoID { get; set; }

        public virtual Aluno Aluno { get; set; }

    }
}
