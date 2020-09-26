﻿using System;
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
        public ActionResult Edit(int id)
        {
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
        public ActionResult Delete(int id)
        {
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
