using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class MarcaRepositorio : Repositorio<Marca>, IMarcaRepositorio
    {

        private readonly ApplicationDbContext _db;

        public MarcaRepositorio(ApplicationDbContext db) : base(db)
        {
           _db = db;
        }

        public void Actualizar(Marca marca)
        {
           var marcaDB= _db.Marcas.FirstOrDefault(b=>b.Id==marca.Id);
            if (marcaDB != null)
            {
                marcaDB.Nombre = marca.Nombre;
                marcaDB.Descripcion = marca.Descripcion;
                marcaDB.Estado = marca.Estado;
                _db.SaveChanges();

            }
        }
    }
}
