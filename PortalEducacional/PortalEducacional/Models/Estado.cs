using System.ComponentModel.DataAnnotations;

namespace PortalEducacional.Models
{
    public class Estado
    {
        public int EstadoId { get; set; }

        public int CodIbge { get; set; }

        [Display(Name = "UF")]
        [StringLength(2)]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string UF { get; set; }

        [Display(Name = "Nome")]
        [StringLength(100)]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Nome { get; set; }
    }
}
