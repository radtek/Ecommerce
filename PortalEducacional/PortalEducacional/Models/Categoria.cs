using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEducacional.Models
{
    public class Categoria
    {
        public int CategoriaID { get; set; }

        [Display(Name = "Descrição da Categoria")]
        [Required(ErrorMessage = "* Campo Obrigatório")]
        public string Descricao { get; set; }

        public string Imagem { get; set; }
    }
}
