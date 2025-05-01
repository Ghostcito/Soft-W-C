using System;
using System.Collections.Generic;
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

        public UserService(ApplicationDbContext context)
        {
            _context = context;
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
    }
}