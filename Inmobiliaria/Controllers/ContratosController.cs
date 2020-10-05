using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

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
        
        public ActionResult Index(DateTime FechaDeInicio, DateTime FechaDeFinalizacion, int IdInmueble, string Estado="Todos")
        {
            IList<Contrato> lista=new List<Contrato>();
            
            string fechaDeInicio = FechaDeInicio.ToString("yyyy-MM-dd");
            string fechaDeFinalizacion = FechaDeFinalizacion.ToString("yyyy-MM-dd");

            

            if (IdInmueble == 0 )
            {               

                if (FechaDeInicio < FechaDeFinalizacion)
                {
                    
                   lista = repositorio.ObtenerTodosDonde(IdInmueble, fechaDeInicio, fechaDeFinalizacion);
                
                }
                else
                {
                    if(FechaDeInicio == new DateTime() || FechaDeFinalizacion == new DateTime())
                        lista = repositorio.ObtenerTodos();
                    else
                        TempData["Error"] = RepositorioBase.mensajeError("fechas");
                }
            }
            else
            {
                if (FechaDeInicio < FechaDeFinalizacion)
                {

                    lista = repositorio.ObtenerTodosDonde(IdInmueble, fechaDeInicio, fechaDeFinalizacion);

                }
                else
                {
                    if (FechaDeInicio == new DateTime() || FechaDeFinalizacion == new DateTime())
                    {
                        TempData["IdInmueble"] = IdInmueble;
                        lista = repositorio.ObtenerTodosDonde(IdInmueble, fechaDeInicio, fechaDeFinalizacion);
                    }
                    else
                        TempData["Error"] = RepositorioBase.mensajeError("fechas");
                }
            }
            

            if (FechaDeInicio != new DateTime())
                TempData["FechaDeInicio"] = fechaDeInicio;

            if (FechaDeFinalizacion != new DateTime())
                TempData["FechaDeFinalizacion"] = fechaDeFinalizacion;


            ViewBag.Estado = Estado;

            Inmueble i = repoInmueble.ObtenerPorId(IdInmueble);
            ViewBag.InmDireccion = (i != null) ? i.Direccion : "";

            TempData["returnUrl"] = "/" + RouteData.Values["controller"] + Request.QueryString.Value;
            

            return View(lista);

        }

        // GET: ContratosController/Details/5
        public ActionResult Details(int id,string returnUrl)
        {


            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            return View(repositorio.ObtenerPorId(id));
        }

        public ActionResult SetearFechasContrato(string returnUrl,string fechaDeInicio, string fechaDeFinalizacion)
        {
            if (fechaDeInicio != "0001-01-01")
                TempData["FechaDeInicio"] = fechaDeInicio;

            if (fechaDeFinalizacion != "0001-01-01")
                TempData["FechaDeFinalizacion"] = fechaDeFinalizacion;

            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            return View();     
        }

        // GET: ContratosController/Create
        public ActionResult Create(string returnUrl, DateTime FechaDeInicio, DateTime FechaDeFinalizacion)
        {


            TempData["returnUrl"] = "/" + RouteData.Values["controller"] + "/SetearFechasContrato" + Request.QueryString.Value;

            ViewBag.Inmuebles = repoInmueble.ObtenerTodosDisponiblesPorFechas(FechaDeInicio.ToString("yyyy-MM-dd"), FechaDeFinalizacion.ToString("yyyy-MM-dd")); //todos los inmuebles disponibles
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();

            TempData["FechaDeInicio"]= FechaDeInicio.ToString("yyyy-MM-dd");
            TempData["FechaDeFinalizacion"]= FechaDeFinalizacion.ToString("yyyy-MM-dd");

            if (FechaDeFinalizacion > FechaDeInicio)
            {

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
            else
            {
                TempData["Error"] = "La fecha de Finalizacion debe ser mayor a la de Inicio";
                return View("SetearFechasContrato");
            }
        }

        // POST: ContratosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato entidad, string returnUrl)
        {

            try
            {
                if (ModelState.IsValid && entidad.FechaDeFinalizacion > entidad.FechaDeInicio)
                {

                    repositorio.Alta(entidad);
                    TempData["Mensaje"] = RepositorioBase.mensajeExitoso("create");


                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                    ViewBag.Inquilinos = repoInquilino.ObtenerTodos();


                    if (entidad.FechaDeFinalizacion <= entidad.FechaDeInicio)
                        TempData["Error"] = "La fecha de Finalizacion debe ser mayor a la de Inicio";

                    TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
                    return View(entidad);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();

                TempData["Error"] = RepositorioBase.mensajeError("create");
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: ContratosController/Edit/5
        public ActionResult Edit(int id, string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;

            var entidad = repositorio.ObtenerPorId(id);
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();


            return View(entidad);
        }

        // POST: ContratosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Contrato entidad)
        {
            try
            {
                
                entidad.IdContrato = id;
                repositorio.Modificacion(entidad);

                TempData["Mensaje"] = RepositorioBase.mensajeExitoso("edit");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();

                TempData["Error"] = RepositorioBase.mensajeError("edit");
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: ContratosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;

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
