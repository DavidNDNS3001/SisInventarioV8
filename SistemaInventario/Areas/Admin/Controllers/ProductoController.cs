﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SistemaInventario.AccesoDatos.Repositorio;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;   

        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo; 
            _webHostEnvironment = webHostEnvironment;   
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownLista("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownLista("Marca"),
                PadreLista=_unidadTrabajo.Producto.ObtenerTodosDropDownLista("Producto")
            };

            if (id == null)
            {
                //crear nuevo Producto
                productoVM.Producto.Estado = true;
                return View(productoVM);
            }
            else
            {
                productoVM.Producto= await _unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());   
                if(productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }         
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
              

                if (productoVM.Producto.Id == 0)
                {
                    //crer Producto
                    string upload = webRootPath + DS.ImagenRuta;
                    string fileName=Guid.NewGuid().ToString();  
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream=new FileStream(Path.Combine(upload,fileName+extension),FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);    
                    }
                    productoVM.Producto.ImagenUrl = fileName + extension;
                    await _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                }
                else
                {
                    //actualizar
                    var objProducto = await _unidadTrabajo.Producto.ObtenerPrimero(p => p.Id == productoVM.Producto.Id, isTracking: false);
                    if (files.Count>0)  //cargar una nueva imagen 
                    {
                        string upload=webRootPath+ DS.ImagenRuta;   
                        string fileName=Guid.NewGuid().ToString();
                        string extension= Path.GetExtension(files[0].FileName);
                        //Borrar imagen anterior
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);
                        if(System.IO.File.Exists(anteriorFile)) 
                        { 
                            System.IO.File.Delete(anteriorFile);
                        }
                        using (var fileStream=new FileStream(Path.Combine(upload,fileName+extension),FileMode.Create)) 
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productoVM.Producto.ImagenUrl= fileName + extension;    
                    } //caso contrario si no se carga una imagen se conserva la imagen
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                }
                TempData[DS.Exitosa] = "Producto se guardo con exito!";
                await _unidadTrabajo.Guardar();
                return View("Index");
            }  //si el model no es valido
            productoVM.CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownLista("Categoria");
            productoVM.MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownLista("Marca");
            productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropDownLista("Producto");
            return View(productoVM);    
        }



        #region API

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades:"Categoria,Marca");
             return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var productoDb = await _unidadTrabajo.Producto.Obtener(id);
            if (productoDb == null)
            {
                return Json(new { success = false, message = "Error al borrar Producto" });
            }

            //remover la imagen
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRuta;
            var anteriorFile = Path.Combine(upload,productoDb.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);    
            }

            _unidadTrabajo.Producto.Remover(productoDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto borrado exitosamente" });
        }

        [ActionName("ValidarSerie")]
        public async Task<IActionResult> ValidarSerie(string serie, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Producto.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b => b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim() && b.Id != id);
            }
            if (valor)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });

        }

        #endregion
    }
}
