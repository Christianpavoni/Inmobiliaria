using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class PropietariosController : Controller
    {
        private readonly IRepositorioPropietario repositorio;
        private IConfiguration config;


        public PropietariosController(IRepositorioPropietario repositorio, IConfiguration config)
        {
            this.repositorio = repositorio;
            this.config = config;



        }

        // GET: PropietariosController
        public ActionResult Index(int id)
        {

            TempData["returnUrl"] = "/" + RouteData.Values["controller"] + Request.QueryString.Value;

            if (id == 0) { 
                var lista = repositorio.ObtenerTodos();
                return View(lista);
            }
            else
            {
                Propietario p = repositorio.ObtenerPorId(id);
                return View(p);
            }

            
        }

        // GET: PropietariosController/Details/5

        public ActionResult Details(int id, string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/"+RouteData.Values["controller"].ToString() : returnUrl;

            return View(repositorio.ObtenerPorId(id));
        }

        // GET: PropietariosController/Create
        public ActionResult Create(string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            return View();
        }

        // POST: PropietariosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: propietario.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    propietario.Clave = hashed;
                    repositorio.Alta(propietario);
                    TempData["Mensaje"] = RepositorioBase.mensajeExitoso("create");
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(propietario);
            }
            catch (Exception ex)
            {
                TempData["Error"] = RepositorioBase.mensajeError("create");
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View(propietario);
            }
        }

        // GET: PropietariosController/Edit/5
        public ActionResult Edit(int id, string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            return View(repositorio.ObtenerPorId(id));
        }

        // POST: PropietariosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            Propietario p = null;
            try
            {
                p = repositorio.ObtenerPorId(id);
                p.Nombre = collection["Nombre"];
                p.Apellido = collection["Apellido"];
                p.Dni = collection["Dni"];
                p.Email = collection["Email"];
                p.Telefono = collection["Telefono"];
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

        // GET: PropietariosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            return View(repositorio.ObtenerPorId(id));
            
        }

        // POST: PropietariosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Propietario entidad)
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
