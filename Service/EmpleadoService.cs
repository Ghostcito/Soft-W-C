using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Soft_W_C.Data;
using Soft_W_C.Models;

namespace Soft_W_C.Service
{
    public class EmpleadoService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly UserService _userService;
        public EmpleadoService(ApplicationDbContext context, UserManager<Usuario> userManager, IHttpContextAccessor httpContextAccessor, UserService userService)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public async Task<IEnumerable<Usuario>> GetEmpleados()
        {
            var userPrincipal = _userService.GetCurrentUserAsync().Result;
            var usuarios = await _userManager.Users.ToListAsync();
            if (_userManager.GetRolesAsync(userPrincipal).Result.Contains("Administrador"))
            {
                return usuarios.Where(u => !_userManager.GetRolesAsync(u).Result.Contains("Administrador")).ToList();
            }
            else
            {
                return GetEmpleadosOfSuper(userPrincipal.Id).Result;
            }
        }

        public async Task<IEnumerable<Usuario>> GetEmpleadosOfSuper(string id)
        {
            var supervisiones = await _context.Supervision
                .Where(s => s.SupervisorId == id)
                .ToListAsync();

            var empleadosIds = supervisiones.Select(s => s.EmpleadoId).ToList();
            var empleados = await _context.Users
                .Where(u => empleadosIds.Contains(u.Id))
                .ToListAsync();
            return empleados;
        }


    }
}