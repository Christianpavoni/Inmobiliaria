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
    public class InquilinosController : Controller
    {
        private readonly IRepositorioInquilino repositorio;
        

        public InquilinosController(IRepositorioInquilino repositorio)
        {
            this.repositorio = repositorio;
            
        }

        // GET: InquilinosController
        public ActionResult Index()
        {
            TempData["returnUrl"] = "/" + RouteData.Values["controller"] + Request.QueryString.Value;

            var lista = repositorio.ObtenerTodos();

            return View(lista);
        }

        // GET: InquilinosController/Details/5
        public ActionResult Details(int id, string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            return View(repositorio.ObtenerPorId(id));
        }

        // GET: InquilinosController/Create
        public ActionResult Create(string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
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
                    TempData["Mensaje"] = RepositorioBase.mensajeExitoso("create");
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(inquilino);
            }
            catch (Exception ex)
            {
                
                    ViewBag.Error = ex.Message;
                    ViewBag.StackTrace = ex.StackTrace;
                    TempData["Error"] = RepositorioBase.mensajeError("create");
                    return View(inquilino);
                
            }
        }
            // GET: InquilinosController/Edit/5
            public ActionResult Edit(int id,string returnUrl)
            {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
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
                    TempData["Mensaje"] = RepositorioBase.mensajeExitoso("edit");
                return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                TempData["Error"] = RepositorioBase.mensajeError("edit");
                ViewBag.Error = ex.Message;
                    ViewBag.StackTrate = ex.StackTrace;
                    return View(p);
                }
            }

        // GET: InquilinosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id,string returnUrl)
            {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            return View(repositorio.ObtenerPorId(id));
            }

            // POST: InquilinosController/Delete/5
            [HttpPost]
            [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Inquilino entidad)
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
