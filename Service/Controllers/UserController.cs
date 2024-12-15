using BLL;
using Entities;
using ProxyLayer;
using SLC;
using SecurityLayer; // Asegúrate de importar SecurityLayer para acceder a AuthService
using System;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;
using DAL;



namespace Service.Controllers
{
    public class UserController : ApiController, IUserService
    {
        private readonly AuthService _authService;
        private readonly Sales_DBEntities _context;
        private readonly SessionRepository _sessionRepository;

        // Asegúrate de inyectar el contexto de la base de datos o crear el objeto AuthService
        public UserController()
        {
            var context = new Sales_DBEntities();
            _authService = new AuthService(context);
            _sessionRepository = new SessionRepository();
        }

        [HttpPost]
        public Users Create(Users user)  // Recibe el token de sesión desde el encabezado
        {
            Console.WriteLine($"Encabezados recibidos: {Request.Headers}");
            var sessionToken = Request.Headers.Authorization?.Parameter;
            Console.WriteLine($"Token recuperado: {sessionToken}"); 

            if (!_authService.HasPermission(sessionToken, "CreateUser"))
            {
                throw new UnauthorizedAccessException("No tienes permiso para crear usuarios.");
            }

            var userLogic = new UserLogic();
            var createdUser = userLogic.Create(user);
            return createdUser;
        }

        [HttpDelete]
        public bool Delete(int userId)  // Recibe el token de sesión desde el encabezado
        {
            var sessionToken = Request.Headers.Authorization?.Parameter;
            if (!_authService.HasPermission(sessionToken, "DeleteUser"))
            {
                throw new UnauthorizedAccessException("No tienes permiso para eliminar usuarios.");
            }

            var userLogic = new UserLogic();
            var result = userLogic.Delete(userId);
            return result;
        }

        [HttpGet]
        public Users RetrieveByID(int userId)  // Recibe el token de sesión desde el encabezado
        {
            var sessionToken = Request.Headers.Authorization?.Parameter;
            if (!_authService.HasPermission(sessionToken, "ReadUser"))
            {
                throw new UnauthorizedAccessException("No tienes permiso para ver usuarios.");
            }

            var userLogic = new UserLogic();
            var user = userLogic.RetrieveByID(userId);
            return user;
        }

        [HttpPut]
        public bool Update(Users userToUpdate)  // Recibe el token de sesión desde el encabezado
        {
            var sessionToken = Request.Headers.Authorization?.Parameter;
            if (!_authService.HasPermission(sessionToken, "UpdateUser"))
            {
                throw new UnauthorizedAccessException("No tienes permiso para actualizar usuarios.");
            }

            var userLogic = new UserLogic();
            var result = userLogic.Update(userToUpdate);
            return result;
        }

        [HttpPost]
        public string Login(LoginRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("El objeto de solicitud es nulo.");
            }

            try
            {
                // Crea una instancia de UserLogic
                var userLogic = new UserLogic();

                // Llamamos al método de login en UserLogic, que nos devuelve el token de sesión
                var sessionToken = userLogic.Login(request); // El Login ahora retorna el token de sesión

                if (string.IsNullOrEmpty(sessionToken))
                {
                    return "Credenciales incorrectas."; // Mensaje si el login es incorrecto
                }

                return sessionToken; // Ahora retornamos el token generado
            }
            catch (UnauthorizedAccessException ex)
            {
                return ex.Message; // Retorna el mensaje de error si las credenciales son incorrectas
            }
            catch (Exception ex)
            {
                return $"Error inesperado: {ex.Message}"; // Retorna un mensaje de error general
            }
        }

        [HttpPost]
        public string Logout([FromBody] string sessionToken)
        {
            var session = new Sessions();
            var authService = new AuthService();
            authService.EndSession(sessionToken);
            _sessionRepository.UpdateSession(session);
            return "Sesión cerrada con éxito.";
        }
    }
}
