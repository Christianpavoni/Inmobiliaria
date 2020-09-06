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
    public class InmueblesController : Controller
    {
        private readonly RepositorioInmueble repositorio;
        private readonly RepositorioPropietario repoPropietario;

        public InmueblesController(IConfiguration config)
        {
            this.repositorio = new RepositorioInmueble(config);
            this.repoPropietario = new RepositorioPropietario(config);
        }

        // GET: InmueblesController
        public ActionResult Index()
        {

                var lista = repositorio.ObtenerTodos();

            return View(lista);
        }


        // GET: InmueblesController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorio.ObtenerPorId(id));
        }

        // GET: InmueblesController/Create
        public ActionResult Create()
        {
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            ViewBag.TipoDeUso = new String[] { "Comercial", "Residencial" };
            ViewBag.Estado = new String[] { "Ocupado", "Disponible", "Suspendido"};
            ViewBag.TipoDeInmueble = new String[] { "Local", "Deposito", "Casa", "Departamento" };
            if (ViewBag.Propietarios.Count != 0)
            {
                return View();
            }
            else
            {
                TempData["Error"] = "Inserte algun PROPIETARIO primero";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: InmueblesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(entidad);
                    TempData["Id"] = entidad.IdInmueble;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                    ViewBag.TipoDeUso = new String[] { "Comercial", "Residencial" };
                    ViewBag.Estado = new String[] { "Ocupado", "Disponible", "Suspendido" };
                    ViewBag.TipoDeInmueble = new String[] { "Local", "Deposito", "Casa", "Departamento" };
                    return View(entidad);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: InmueblesController/Edit/5
        public ActionResult Edit(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            ViewBag.TipoDeUso = new String[] { "Comercial", "Residencial" };
            ViewBag.Estado = new String[] { "Ocupado", "Disponible", "Suspendido" };
            ViewBag.TipoDeInmueble = new String[] { "Local", "Deposito", "Casa", "Departamento" };
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: InmueblesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble entidad)
        {
            try
            {
                entidad.IdInmueble = id;
                repositorio.Modificacion(entidad);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                ViewBag.TipoDeUso = new String[] { "Comercial", "Residencial" };
                ViewBag.Estado = new String[] { "Ocupado", "Disponible", "Suspendido" };
                ViewBag.TipoDeInmueble = new String[] { "Local", "Deposito", "Casa", "Departamento" };
                TempData["Error"] = "Error en la Edicion";
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: InmueblesController/Delete/5
        public ActionResult Delete(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: InmueblesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inmueble entidad)
        {
            try
            {
                repositorio.Baja(id);
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
