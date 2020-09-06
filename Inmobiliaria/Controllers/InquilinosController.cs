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
    public class InquilinosController : Controller
    {
        private readonly RepositorioInquilino repositorio;
        

        public InquilinosController(IConfiguration config)
        {
            this.repositorio = new RepositorioInquilino(config);
            
        }

        // GET: InquilinosController
        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        // GET: InquilinosController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorio.ObtenerPorId(id));
        }

        // GET: InquilinosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InquilinosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(inquilino);
                    TempData["Id"] = inquilino.IdInquilino;
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(inquilino);
            }
            catch (Exception ex)
            {
                
                    ViewBag.Error = ex.Message;
                    ViewBag.StackTrace = ex.StackTrace;
                    return View(inquilino);
                
            }
        }
            // GET: InquilinosController/Edit/5
            public ActionResult Edit(int id)
            {
                return View(repositorio.ObtenerPorId(id));
            }

            // POST: InquilinosController/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit(int id, IFormCollection collection)
            {
                Inquilino p = null;
                try
                {
                    p = repositorio.ObtenerPorId(id);
                    p.Nombre = collection["Nombre"];
                    p.Apellido = collection["Apellido"];
                    p.Dni = collection["Dni"];
                    p.Email = collection["Email"];
                    p.Telefono = collection["Telefono"];
                    p.LugarDeTrabajo = collection["LugarDeTrabajo"];
                    p.NombreGarante = collection["NombreGarante"];
                    p.DniGarante = collection["DniGarante"];
                    p.TelefonoGarante = collection["TelefonoGarante"];
					p.EmailGarante = collection["EmailGarante"];
                    repositorio.Modificacion(p);
                    TempData["Mensaje"] = "Datos guardados correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error en la Edicion";
                    ViewBag.Error = ex.Message;
                    ViewBag.StackTrate = ex.StackTrace;
                    return View(p);
                }
            }

            // GET: InquilinosController/Delete/5
            public ActionResult Delete(int id)
            {
                return View(repositorio.ObtenerPorId(id));
            }

            // POST: InquilinosController/Delete/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Delete(int id, Inquilino entidad)
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
