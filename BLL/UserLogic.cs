using DAL;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurityLayer;

namespace BLL
{
    public class UserLogic
    {
        private readonly AuthService _authService;

        public UserLogic()
        {
            _authService = new AuthService(); // Crear la instancia del servicio de autenticación
        }
        // Método para crear un nuevo usuario
        public Users Create(Users newUser)
        {
            Users result = null;

            // Asegúrate de crear una instancia de AuthService
            var authService = new AuthService();

            using (var r = RepositoryFactory.CreateRepository())
            {
                // Verificamos si el nombre de usuario ya existe
                Users existingUser = r.Retrieve<Users>(u => u.UserName == newUser.UserName);
                if (existingUser == null)
                {
                    // Generamos el hash de la contraseña utilizando la capa de seguridad
                    string hashedPassword = authService.HashPassword(newUser.PasswordHash);

                    // Asignamos el hash al campo PasswordHash del nuevo usuario
                    newUser.PasswordHash = hashedPassword;

                    // Verificamos que el campo Email haya sido proporcionado
                    if (string.IsNullOrEmpty(newUser.Email))
                    {
                        throw new Exception("Email is required");
                    }

                    // Establecemos valores por defecto para los campos requeridos si no se han proporcionado
                    newUser.IsActive = newUser.IsActive != default(bool) ? newUser.IsActive : true; // Campo obligatorio con valor por defecto 1
                    newUser.FailedLoginAttempts = newUser.FailedLoginAttempts != default(int) ? newUser.FailedLoginAttempts : 0; // Campo obligatorio con valor por defecto 0

                    // Validamos que RoleId sea válido antes de insertar
                    var roleExists = r.Retrieve<Roles>(role => role.RoleId == newUser.RoleId) != null;
                    if (!roleExists)
                    {
                        throw new Exception("Invalid RoleId specified");
                    }

                    // Creamos el nuevo usuario en la base de datos
                    result = r.Create(newUser);
                }
                else
                {
                    throw new Exception("User already exists");
                }
            }

            return result;
        }



        public Users RetrieveByID(int userId)
        {
            Users result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                result = r.Retrieve<Users>(u => u.UserId == userId);
                return result;
            }
        }

        public bool Update(Users userToUpdate)
        {
            bool res = false;
            using (var r = RepositoryFactory.CreateRepository())
            {
                Users existingUser = r.Retrieve<Users>(u => u.UserName == userToUpdate.UserName && u.UserId != userToUpdate.UserId);
                if (existingUser == null)
                {
                    res = r.Update(userToUpdate);
                }
                else
                {
                    throw new Exception("Another user with the same username exists");
                }
                return res;
            }
        }

        public bool Delete(int userId)
        {
            bool res = false;
            var user = RetrieveByID(userId);
            if (user != null)
            {
                using (var r = RepositoryFactory.CreateRepository())
                {
                    res = r.Delete(user);
                }
            }
            else
            {
                throw new Exception("User not found");
            }
            return res;
        }

        public string Login(LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                throw new ArgumentException("El nombre de usuario y la contraseña son obligatorios.");
            }

            // Llamamos al servicio de autenticación para validar las credenciales y generar el token
            var sessionToken = _authService.AuthenticateUser(request.Username, request.Password);

            if (string.IsNullOrEmpty(sessionToken))
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas.");
            }
            // Depuración: Verifica que el token fue generado
            System.Diagnostics.Debug.WriteLine("Token generado: " + sessionToken);

            // Si el login es exitoso, devolvemos el token de sesión
            return sessionToken; // Ahora retornamos el token en lugar de un objeto 'Users'
        }
        public List<Users> RetrieveAll()
        {
            List<Users> result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                result = r.RetrieveAll<Users>(); // Llamamos al método RetrieveAll del repositorio
            }
            return result;
        }



    }
}



