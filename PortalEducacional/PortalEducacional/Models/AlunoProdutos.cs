namespace PortalEducacional.Models
{
    public class AlunoProdutos
    {
        public virtual int AlunoProdutosID { get; set; }

        public virtual int AlunoID { get; set; }
        public virtual Aluno Aluno { get; set; }

        public virtual int ProdutoID { get; set; }
        public virtual Produto Produto { get; set; }
    }
}
