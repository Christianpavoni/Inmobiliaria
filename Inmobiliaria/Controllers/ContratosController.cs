using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class ContratosController : Controller
    {
        private readonly IRepositorioContrato repositorio;
        private readonly IRepositorioInmueble repoInmueble;
        private readonly IRepositorioInquilino repoInquilino;
        public ContratosController(IRepositorioContrato repositorio, IRepositorioInmueble repoInmueble, IRepositorioInquilino repoInquilino)
        {
            this.repositorio = repositorio;
            this.repoInmueble = repoInmueble;
            this.repoInquilino = repoInquilino;
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
            ViewBag.Estado = Contrato.ObtenerEstados();
            if (ViewBag.Inmuebles.Count != 0 && ViewBag.Inquilinos.Count != 0)
            {
                return View();
            }
            else
            {
                if(ViewBag.Inquilinos.Count == 0)
                    TempData["ErrorInq"] = RepositorioBase.mensajeErrorInsert("INQUILINO");
                else
                    TempData["ErrorInm"] = RepositorioBase.mensajeErrorInsert("INMUEBLE");

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
                    TempData["Mensaje"] = RepositorioBase.mensajeExitoso("create");

                    repoInmueble.Modificacion(inmueble);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                    ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                    ViewBag.Estado = Contrato.ObtenerEstados();

                    if (entidad.FechaDeFinalizacion <= entidad.FechaDeInicio)
                        TempData["Error"] = "La fecha de Finalizacion debe ser mayor a la de Inicio";

                    return View(entidad);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                ViewBag.Estado = Contrato.ObtenerEstados();
                TempData["Error"] = RepositorioBase.mensajeError("create");
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
            ViewBag.Estado = Contrato.ObtenerEstados();

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
                TempData["Mensaje"] = RepositorioBase.mensajeExitoso("edit");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                ViewBag.Estado = Contrato.ObtenerEstados();
                TempData["Error"] = RepositorioBase.mensajeError("edit");
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: ContratosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);

            return View(entidad);
        }

        // POST: ContratosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Contrato entidad)
        {
            try
            {
                entidad = repositorio.ObtenerPorId(id);
                var inmueble = repoInmueble.ObtenerPorId(entidad.IdInmueble);
                inmueble.Estado = "Disponible";
                repositorio.Baja(id);
                repoInmueble.Modificacion(inmueble);
                TempData["Mensaje"] = RepositorioBase.mensajeExitoso("delete");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = RepositorioBase.mensajeError("delete");
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }
    }
}
