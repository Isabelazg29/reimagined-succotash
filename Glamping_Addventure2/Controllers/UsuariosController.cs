﻿using Microsoft.AspNetCore.Mvc;
using Glamping_Addventure2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Glamping_Addventure.Models.Recursos;
using Glamping_Addventure.Models.Servicios.Contrato;
using Glamping_Addventure.Models.Servicios.Implementación;

namespace Glamping_Addventure2.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly GlampingAddventureContext _context;
        private readonly IUsuarioService _usuarioServicio;

        public UsuariosController(GlampingAddventureContext context, IUsuarioService usuarioServicio)
        {
            _context = context;
            _usuarioServicio = usuarioServicio;
        }


        // GET: Usuarios
        public IActionResult Index()
        {
            var usuarios = _context.Usuarios.Include(u => u.IdrolNavigation).ToList();
            return View(usuarios);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewBag.Roles = _context.Roles.ToList();
            return View(); // Este método cargará la vista con los roles disponibles.
        }

        // POST: Usuarios/Create
        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreUsuario,Email,Apellido,TipoDocumento,NumeroDocumento,Direccion,Telefono,Idrol,Contrasena")] Usuario usuario, int Idrol, string confirmarContrasena)
        {
            if (Idrol <= 0)
            {
                ModelState.AddModelError("Idrol", "Debe seleccionar un tipo de rol.");
            }

            if (string.IsNullOrWhiteSpace(confirmarContrasena) || usuario.Contrasena != confirmarContrasena)
            {
                ModelState.AddModelError("Contrasena", "Las contraseñas no coinciden.");
            }

            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                ModelState.AddModelError("Email", "El correo electrónico ya está registrado.");
            }

            if (_context.Usuarios.Any(u => u.NumeroDocumento == usuario.NumeroDocumento))
            {
                ModelState.AddModelError("NumeroDocumento", "El número de documento ya está registrado.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _context.Roles.ToList(); // Para mantener los roles en el formulario
                return View(usuario);
            }

            usuario.Contrasena = Utilidades.EncriptarClave(usuario.Contrasena);
            usuario.Idrol = Idrol;

            Usuario usuario_creado = await _usuarioServicio.SaveUsuario(usuario);
            if (usuario_creado != null && usuario_creado.Idusuario > 0)
            {
                return RedirectToAction("Index");  // Redirige al Index de Usuarios
            }

            ModelState.AddModelError("", "No se pudo crear el usuario.");
            ViewBag.Roles = _context.Roles.ToList(); // Mantener los roles en la vista
            return View(usuario);
        }



        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }


        // GET: Usuarios/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewBag.Roles = _context.Roles.ToList();
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Idusuario,NombreUsuario,Email,Apellido,TipoDocumento,NumeroDocumento,Direccion,Telefono,Idrol")] Usuario usuario)
        {
            if (id != usuario.Idusuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var usuarioExistente = _context.Usuarios.Find(id);
                if (usuarioExistente != null)
                {
                    usuarioExistente.NombreUsuario = usuario.NombreUsuario;
                    usuarioExistente.Email = usuario.Email;
                    usuarioExistente.Apellido = usuario.Apellido;
                    usuarioExistente.TipoDocumento = usuario.TipoDocumento;
                    usuarioExistente.NumeroDocumento = usuario.NumeroDocumento;
                    usuarioExistente.Direccion = usuario.Direccion;
                    usuarioExistente.Telefono = usuario.Telefono;
                    usuarioExistente.Idrol = usuario.Idrol;

                    _context.Update(usuarioExistente);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.Roles = _context.Roles.ToList();
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound("El ID proporcionado es nulo.");
            }

            var usuario = _context.Usuarios.FirstOrDefault(m => m.Idusuario == id);
            if (usuario == null)
            {
                return NotFound("El usuario no existe en la base de datos.");
            }

            return View(usuario);
        }

        // POST: Usuarios/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound("El usuario que intentas eliminar no existe.");
            }

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }
}
