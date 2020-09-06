using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    public class ContratosController : Controller
    {
        private readonly RepositorioContrato repositorio;
        private readonly RepositorioInmueble repoInmueble;
        private readonly RepositorioInquilino repoInquilino;
        public ContratosController(IConfiguration config)
        {
            this.repositorio = new RepositorioContrato(config);
            this.repoInmueble = new RepositorioInmueble(config);
            this.repoInquilino = new RepositorioInquilino(config);
        }

        // GET: ContratosController
        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: ContratosController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorio.ObtenerPorId(id));
        }

        // GET: ContratosController/Create
        public ActionResult Create()
        {
            ViewBag.Inmuebles = repoInmueble.ObtenerTodosDisponibles();
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
            ViewBag.Estado = new String[] { "Vigente", "No Vigente"};
            if (ViewBag.Inmuebles.Count != 0 && ViewBag.Inquilinos.Count != 0)
            {
                return View();
            }
            else
            {
                if(ViewBag.Inquilinos.Count == 0)
                    TempData["ErrorInq"] = "Inserte algun INQUILINO primero";
                else
                    TempData["ErrorInm"] = "No hay INMUEBLE disponible";

                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ContratosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato entidad)
        {
            try
            {
                if (ModelState.IsValid && entidad.FechaDeFinalizacion > entidad.FechaDeInicio)
                {
                    var inmueble = repoInmueble.ObtenerPorId(entidad.IdInmueble);
                    inmueble.Estado = "Ocupado";
                    repositorio.Alta(entidad);
                    
                    repoInmueble.Modificacion(inmueble);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                    ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                    ViewBag.Estado = new String[] { "Vigente", "No Vigente" };
                    
                    if(entidad.FechaDeFinalizacion <= entidad.FechaDeInicio)
                        TempData["Error"] = "La fecha de Finalizacion debe ser mayor a la de Inicio";

                    return View(entidad);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                ViewBag.Estado = new String[] { "Vigente", "No Vigente" };
                TempData["Error"] = "Error al Crear";
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: ContratosController/Edit/5
        public ActionResult Edit(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
            ViewBag.Estado = new String[] { "Vigente", "No Vigente" };
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: ContratosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Contrato entidad)
        {
            try
            {
                var inmueble = repoInmueble.ObtenerPorId(entidad.IdInmueble);
                inmueble.Estado = "Ocupado";

                
                entidad.IdContrato = id;
                repositorio.Modificacion(entidad);
                repoInmueble.Modificacion(inmueble);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                ViewBag.Estado = new String[] { "Vigente", "No Vigente" };
                TempData["Error"] = "Error en la Edicion";
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: ContratosController/Delete/5
        public ActionResult Delete(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: ContratosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Contrato entidad)
        {
            try
            {
                entidad = repositorio.ObtenerPorId(id);
                var inmueble = repoInmueble.ObtenerPorId(entidad.IdInmueble);
                inmueble.Estado = "Disponible";
                repositorio.Baja(id);
                repoInmueble.Modificacion(inmueble);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error en la Eliminación";
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }
    }
}
