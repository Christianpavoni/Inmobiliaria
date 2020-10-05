using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioPropietario repoPropietario;


        public InmueblesController(IRepositorioInmueble repositorio, IRepositorioPropietario repoPropietario)
        {
            this.repositorio = repositorio;
            this.repoPropietario = repoPropietario;

        }

        // GET: InmueblesController
        public ActionResult Index(DateTime FechaDeInicio, DateTime FechaDeFinalizacion, string Estado, int IdPropietario, int mostrar)
        {
            IList<Inmueble> lista = new List<Inmueble>();

            string fechaDeInicio = FechaDeInicio.ToString("yyyy-MM-dd");
            string fechaDeFinalizacion = FechaDeFinalizacion.ToString("yyyy-MM-dd");

            if (FechaDeInicio < FechaDeFinalizacion && mostrar == 1)
            {              
                lista = repositorio.ObtenerTodosDisponiblesPorFechas(fechaDeInicio, fechaDeFinalizacion);
                TempData["mostrar"] = mostrar;
            }
            else {
                if (FechaDeInicio != new DateTime() || FechaDeFinalizacion != new DateTime())
                    TempData["ErrorF"] = RepositorioBase.mensajeError("fechas");

            if (IdPropietario == 0 && String.IsNullOrEmpty(Estado))
            {
                lista = repositorio.ObtenerTodos();
            }

            if (IdPropietario != 0 && String.IsNullOrEmpty(Estado))
            {
                lista = repositorio.ObtenerTodosDonde(IdPropietario,Estado);

            }    
            
            if(IdPropietario == 0 && !String.IsNullOrEmpty(Estado))
            {
                lista = repositorio.ObtenerTodosDonde(IdPropietario, Estado);

            }

            if (IdPropietario != 0 && !String.IsNullOrEmpty(Estado))
            {
                lista = repositorio.ObtenerTodosDonde(IdPropietario, Estado);

            }
            }

            if (FechaDeInicio != new DateTime())
                TempData["FechaDeInicio"] = fechaDeInicio;

            if (FechaDeFinalizacion != new DateTime())
                TempData["FechaDeFinalizacion"] = fechaDeFinalizacion;

            TempData["returnUrl"] = "/" + RouteData.Values["controller"] + Request.QueryString.Value;

            var MostrarEstados = Inmueble.ObtenerEstados();
            MostrarEstados.Add("Todos");
            ViewBag.MostrarEstados = MostrarEstados;

            Propietario p = repoPropietario.ObtenerPorId(IdPropietario);
            ViewBag.PropNombreApellido = (p != null) ? p.Nombre + " " + p.Apellido : "";

            return View(lista);
        }


        // GET: InmueblesController/Details/5
        public ActionResult Details(int id, string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            return View(repositorio.ObtenerPorId(id));

        }

        // GET: InmueblesController/Create
        public ActionResult Create(string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;

            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            ViewBag.TipoDeUso = Inmueble.ObtenerTiposDeUsos();
            ViewBag.Estado = Inmueble.ObtenerEstados();
            ViewBag.TipoDeInmueble = Inmueble.ObtenerTiposDeInmuebles();
            if (ViewBag.Propietarios.Count != 0)
            {
                return View();
            }
            else
            {
                TempData["Error"] = RepositorioBase.mensajeErrorInsert("PROPIETARIOS");
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
                    TempData["Mensaje"] = RepositorioBase.mensajeExitoso("create");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                    ViewBag.TipoDeUso = Inmueble.ObtenerTiposDeUsos();
                    ViewBag.Estado = Inmueble.ObtenerEstados();
                    ViewBag.TipoDeInmueble = Inmueble.ObtenerTiposDeInmuebles();
                    TempData["Error"] = RepositorioBase.mensajeError("create");
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
        public ActionResult Edit(int id, string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;

            var entidad = repositorio.ObtenerPorId(id);
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            ViewBag.TipoDeUso = Inmueble.ObtenerTiposDeUsos();
            ViewBag.Estado = Inmueble.ObtenerEstados();
            ViewBag.TipoDeInmueble = Inmueble.ObtenerTiposDeInmuebles();

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
                TempData["Mensaje"] = RepositorioBase.mensajeExitoso("edit");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                ViewBag.TipoDeUso = Inmueble.ObtenerTiposDeUsos();
                ViewBag.Estado = Inmueble.ObtenerEstados();
                ViewBag.TipoDeInmueble = Inmueble.ObtenerTiposDeInmuebles();

                TempData["Error"] = RepositorioBase.mensajeError("edit");
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: InmueblesController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;

            var entidad = repositorio.ObtenerPorId(id);

            return View(entidad);
        }

        // POST: InmueblesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Inmueble entidad)
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
