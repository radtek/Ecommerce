using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalEducacional.Models
{
    public class Produto : BaseModel
    {
        public int ProdutoID { get; set; }

        [Display(Name = "Código do Produto")]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Codigo { get; set; }

        [Display(Name = "Descrição do Produto")]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Descricao { get; set; }

        [Display(Name = "R$ Valor")]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public virtual decimal Valor { get; set; }

        public bool Ativo { get; set; }

        public string DataCadastro { get; set; }

        public string CodigoBarras { get; set; }

        public bool PrecisaAprovacao { get; set; }

        public IList<DadoNutricional> DadosNutricionais { get; set; }

        public int CategoriaID { get; set; }

        public virtual Categoria Categoria { get; set; }

        public virtual Escola Escola { get; set; }

        public virtual int EscolaID { get; set; }

        [Display(Name = "Unidade de Medida")]
        public virtual string UnidadeMedida { get; set; }

        [Display(Name = "Benefícios")]
        public virtual string Observacao { get; set; }

        [NotMapped]
        public virtual bool Replicar { get; set; }

        [NotMapped]
        public bool Deletado { get; set; }

        [NotMapped]
        public int AlunoProdutosID { get; set; }
    }
}
