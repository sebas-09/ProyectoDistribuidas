using Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class AuthRepository : IDisposable
    {
        private readonly Sales_DBEntities _context;

        public AuthRepository()
        {
            _context = new Sales_DBEntities();
        }

        // Método para validar las credenciales del usuario
        public Users GetUserByCredentials(string username, string hashedPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(hashedPassword))
                throw new ArgumentNullException("El nombre de usuario o la contraseña no pueden ser nulos o vacíos.");

            // Busca el usuario en la base de datos con las credenciales exactas
            return _context.Users.FirstOrDefault(u =>
                u.UserName.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase) &&
                u.PasswordHash == hashedPassword);
        }

        // Obtener usuario por nombre de usuario
        public Users GetUserByUsername(string username)
        {
            return _context.Users
                           .FirstOrDefault(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        // Obtener usuario por ID
        public Users GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        // Actualizar información del usuario
        public void UpdateUser(Users user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser != null)
            {
                existingUser.FailedLoginAttempts = user.FailedLoginAttempts;
                existingUser.AccountLockedUntil = user.AccountLockedUntil;
                existingUser.IsActive = user.IsActive;
                _context.SaveChanges();
            }
        }

        // Obtener permisos de un usuario
        public List<Permissions> GetPermissionsByUser(int userId)
        {
            var userRoles = _context.Users.Where(u => u.UserId == userId)
                                          .Select(u => u.RoleId)
                                          .ToList();

            return _context.RolePermissions
                           .Where(rp => userRoles.Contains(rp.RoleId))
                           .Select(rp => rp.Permissions)
                           .ToList();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
