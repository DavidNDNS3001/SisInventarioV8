﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Bodega
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Nombre Requerido")]
        [MaxLength(80, ErrorMessage ="Nombre debe ser maximo 80 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Descripción es Requerido")]
        [MaxLength(100, ErrorMessage = "Descripción debe ser maximo 100 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Estado es Requerido")]
        public bool Estado { get; set; }

    }
}