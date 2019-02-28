using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models
{
    public class Endereco
    {
        public int EnderecoId { get; set; }

        [Display(Name = "Número")]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Numero { get; set; }

        [Display(Name = "CEP")]
        [StringLength(10)]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string CEP { get; set; }

        [Display(Name = "Logradouro")]
        [StringLength(100)]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Logradouro { get; set; }

        public virtual int CidadeId { get; set; }

        public virtual Cidade Cidade { get; set; }

        [Display(Name = "Bairro")]
        [StringLength(60)]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Bairro { get; set; }

        [StringLength(100)]
        public string Complemento { get; set; }
    }
}
