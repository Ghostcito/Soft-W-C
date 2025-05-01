using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Soft_W_C.Data;
using Soft_W_C.Models;

namespace Soft_W_C.Service
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public UserService(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Usuario?> FindByDniAsync(string dni)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.DNI == dni);
        }

        public async Task<bool> IsDniAvailableAsync(string dni)
        {
            return !await _context.Users
                .AnyAsync(u => u.DNI == dni);
        }


        // Método para obtener todos los usuarios registrados
        public async Task<List<Usuario>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        // Método para obtener usuarios con información de roles
        // public async Task<List<UsuarioConRolViewModel>> GetAllUsersWithRolesAsync()
        // {
        //     var usuarios = await _userManager.Users.ToListAsync();
        //     var usuariosConRoles = new List<UsuarioConRolViewModel>();

        //     foreach (var usuario in usuarios)
        //     {
        //         var roles = await _userManager.GetRolesAsync(usuario);
        //         usuariosConRoles.Add(new UsuarioConRolViewModel
        //         {
        //             Usuario = usuario,
        //             Roles = roles.ToList()
        //         });
        //     }

        //     return usuariosConRoles;
        // }
    }
}