using DAL;
using Entities;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Diagnostics;

namespace SecurityLayer
{
    public class AuthService
    {
        private readonly AuthRepository _authRepository;
        private readonly SessionRepository _sessionRepository;
        private readonly Sales_DBEntities _context;


        public AuthService(Sales_DBEntities context)
        {
            _context = context;
        }
        public AuthService()
        {
            _authRepository = new AuthRepository();
            _sessionRepository = new SessionRepository();
            
        }

        // Método para autenticar al usuario
        public string AuthenticateUser(string username, string password)
        {
            // Validaciones iniciales
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentException("El nombre de usuario y la contraseña son obligatorios.");

            // Encriptar la contraseña
            string hashedPassword = HashPassword(password);

            // Validar credenciales
            var user = _authRepository.GetUserByCredentials(username, hashedPassword);

            if (user == null || !user.IsActive)
            {
                RegisterFailedAttempt(username);
                throw new UnauthorizedAccessException("Credenciales inválidas o cuenta deshabilitada.");
            }

            // Verificar bloqueo de cuenta
            if (user.AccountLockedUntil != null && user.AccountLockedUntil > DateTime.Now)
                throw new UnauthorizedAccessException("La cuenta está bloqueada temporalmente. Intenta más tarde.");

            // Reiniciar intentos fallidos
            ResetFailedAttempts(user.UserId);

            // Registrar última conexión
            user.LastLogin = DateTime.Now;
            _authRepository.UpdateUser(user);

            // Crear sesión
            var sessionToken = GenerateSessionToken(); // Método para generar el token
            var session = new Sessions
            {
                UserId = user.UserId,
                SessionToken = sessionToken,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(1), // Sesión expira en 1 hora
                IsActive = true
            };
            _sessionRepository.CreateSession(session); // Aquí creas la sesión en la base de datos

            // Registrar evento de login exitoso
            LogEvent(user.UserId, "Login Successful", "Inicio de sesión exitoso");

            return sessionToken; // Ahora retorna el token de sesión
        }


        public void EndSession(string sessionToken)
        {
            _sessionRepository.EndSession(sessionToken);
        }
        public string GenerateSessionToken()
        {
            return Guid.NewGuid().ToString();
        }

        public bool IsSessionValid(string sessionToken)
        {
            using (var sessionRepository = new SessionRepository())
            {
                var session = sessionRepository.GetSessionByToken(sessionToken);
                return session != null;
            }
        }


        // Método para registrar intentos fallidos
        public void RegisterFailedAttempt(string username)
        {
            var user = _authRepository.GetUserByUsername(username);

            if (user != null)
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5) // Límite de intentos fallidos
                {
                    user.AccountLockedUntil = DateTime.Now.AddMinutes(15); // Bloquear por 15 minutos
                }

                _authRepository.UpdateUser(user);
            }
        }

        // Método para reiniciar intentos fallidos después de un inicio de sesión exitoso
        public void ResetFailedAttempts(int userId)
        {
            var user = _authRepository.GetUserById(userId);
            if (user != null)
            {
                user.FailedLoginAttempts = 0;
                user.AccountLockedUntil = null;
                _authRepository.UpdateUser(user);
            }
        }

        // Método para generar un hash seguro de la contraseña
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Método para obtener el usuario a partir del token de sesión
        public Users GetUserFromSessionToken(string sessionToken)
        {
            Debug.WriteLine($"Validando token: {sessionToken}");
            var session = _context.Sessions.FirstOrDefault(s => s.SessionToken == sessionToken && s.IsActive);
            if (session == null)
            {
                throw new UnauthorizedAccessException("Token de sesión inválido o la sesión ha expirado.");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == session.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuario no encontrado para el token de sesión proporcionado.");
            }

            return user;
        }

        // Verificar si el usuario tiene el permiso necesario
        public bool HasPermission(string sessionToken, string permissionName)
        {
            var user = GetUserFromSessionToken(sessionToken);
            int roleId = user.RoleId;

            var permissions = _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permissions.PermissionName)
                .ToList();

            return permissions.Contains(permissionName);
        }

        // Método para registrar eventos en auditoría
        public void LogEvent(int? userId, string eventType, string details)
        {
            using (var context = new Sales_DBEntities())
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    EventType = eventType,
                    EventDateTime = DateTime.UtcNow,
                    Details = details
                };

                context.AuditLog.Add(auditLog);
                context.SaveChanges();
            }
        }
        

    }
}
