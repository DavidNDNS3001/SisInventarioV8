using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Marca
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Nombre de Marca Requerido")]
        [MaxLength(80, ErrorMessage ="Nombre de Marca debe ser maximo 80 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Descripción de Marca es Requerido")]
        [MaxLength(100, ErrorMessage = "Descripción de Marca debe ser maximo 100 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Estado es Requerido")]
        public bool Estado { get; set; }

    }
}
