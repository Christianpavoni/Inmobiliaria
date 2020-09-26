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
    public class PagosController : Controller
    {
        private readonly IRepositorioPago repositorio;
        private readonly IRepositorioContrato repoContrato;
        
        public PagosController(IRepositorioPago repositorio, IRepositorioContrato repoContrato)
        {
            this.repositorio = repositorio;
            this.repoContrato = repoContrato;
            
        }

        // GET: PagosController
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
            ViewBag.Contratos = repoContrato.ObtenerTodosVigentes();
            
            if (ViewBag.Contratos.Count != 0 )
            {
                return View();
            }
            else
            {
                
                TempData["Error"] = RepositorioBase.mensajeErrorInsert("CONTRATO");

                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ContratosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pago entidad)
        {
            try
            {
                if (ModelState.IsValid) {

                    repositorio.Alta(entidad);
                    TempData["Mensaje"] = RepositorioBase.mensajeExitoso("create");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                ViewBag.Contratos = repoContrato.ObtenerTodosVigentes();

                return View(entidad);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Contratos = repoContrato.ObtenerTodosVigentes();

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
            ViewBag.Contratos = repoContrato.ObtenerTodosVigentes();
            
            return View(entidad);
        }

        // POST: ContratosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Pago entidad)
        {
            try
            {
                entidad.IdPago = id;
                repositorio.Modificacion(entidad);
                TempData["Mensaje"] = RepositorioBase.mensajeExitoso("edit");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Contratos = repoContrato.ObtenerTodosVigentes();

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
        public ActionResult Delete(int id, Pago entidad)
        {
            try
            {
                repositorio.Baja(id);                
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
