﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

namespace Inmobiliaria.Controllers
{
    
    public class UsuariosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly IRepositorioUsuario repositorio;

        public UsuariosController(IConfiguration configuration, IWebHostEnvironment environment, IRepositorioUsuario repositorio)
        {
            this.configuration = configuration;
            this.environment = environment;
            this.repositorio = repositorio;
        }
        //GET: Usuarios
        [Authorize(Policy = "Administrador")]
        public ActionResult Index()
        {
            TempData["returnUrl"] = "/" + RouteData.Values["controller"] + Request.QueryString.Value;
            var usuarios = repositorio.ObtenerTodos();
            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id, string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            var e = repositorio.ObtenerPorId(id);
            return View(e);
        }

        // GET: Usuarios/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Usuario u)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: u.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                u.Clave = hashed;
                u.Rol = User.IsInRole("Administrador") ? u.Rol : (int)enRoles.Empleado;
                var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                int res = repositorio.Alta(u);
                
                if (u.AvatarFile != null && u.IdUsuario > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                    string fileName = "avatar_" + u.IdUsuario + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads", fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    repositorio.Modificacion(u);
                }
                TempData["Mensaje"] = RepositorioBase.mensajeExitoso("create");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                TempData["Error"] = RepositorioBase.mensajeError("create");
                return View();
            }
        }

        // GET: Usuarios/Edit/5
        [Authorize(Policy = "Empleado")]
        public ActionResult Perfil(string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            ViewData["Title"] = "Mi perfil";
            var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View("Edit", u);
        }

        // GET: Usuarios/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id,string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            ViewData["Title"] = "Editar usuario";
            var u = repositorio.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(u);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Usuario u)
        {
            var vista = "Edit";
            
            try
            {
                if (User.IsInRole("Empleado"))
                {
                    Usuario user = repositorio.ObtenerPorEmail(User.Identity.Name);
                    u.IdUsuario = user.IdUsuario;
                    u.Rol = user.Rol;

                }
                else
                {
                    u.IdUsuario = id;
                }
                if (u.AvatarFile != null)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                    string fileName = "avatar_" + u.IdUsuario + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads", fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                }
                else
                {
                    var us = repositorio.ObtenerPorId(u.IdUsuario);
                    u.Avatar = us.Avatar;
                }

                repositorio.Modificacion(u);
                TempData["Mensaje"] = RepositorioBase.mensajeExitoso("edit");
                if (User.IsInRole("Empleado"))
                {
                    return RedirectToAction("Perfil", "Usuarios");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                TempData["Error"] = RepositorioBase.mensajeError("edit");
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(vista, u);
            }
        }

        // GET: Usuarios/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id,string returnUrl)
        {
            TempData["returnUrl"] = String.IsNullOrEmpty(returnUrl) ? "/" + RouteData.Values["controller"].ToString() : returnUrl;
            var entidad = repositorio.ObtenerPorId(id);

            return View(entidad);
        }

        // POST: Usuarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Usuario entidad)
        {
            try
            {
                entidad = repositorio.ObtenerPorId(id);

                repositorio.Baja(id);

                if (System.IO.File.Exists(environment.WebRootPath + "/Uploads/avatar_" + id + Path.GetExtension(entidad.Avatar)))
                    System.IO.File.Delete(environment.WebRootPath + "/Uploads/avatar_" + id + Path.GetExtension(entidad.Avatar));

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
        

        [AllowAnonymous]
        // GET: Usuarios/Login/
        public ActionResult Login()
        {
            return View();
        }

        // POST: Usuarios/Login/
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: login.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    var e = repositorio.ObtenerPorEmail(login.Usuario);
                    if (e == null || e.Clave != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos");
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                        new Claim(ClaimTypes.Role, e.RolNombre),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction(nameof(Index), "Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: /salir
        [Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
