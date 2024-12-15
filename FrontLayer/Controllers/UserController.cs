using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;  // Llamadas a la lógica de negocio
using Entities;  // Modelos de entidades
using SecurityLayer;
using DAL;

namespace FrontLayer.Controllers
{
    public class UserController : Controller
    {
        private readonly UserLogic _userLogic;
        private readonly AuthService _authService;
        private readonly AuthRepository _authRepository;

        public UserController()
        {
            _userLogic = new UserLogic();
            _authService = new AuthService();
            _authRepository = new AuthRepository();
        }

        // GET: Login Page
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // GET: Create User
        [HttpGet]
        public ActionResult Create()
        {
            return View(new Users());
        }

        // POST: Create User
        [HttpPost]
        [AuthorizeUser]
        public ActionResult Create(Users newUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _userLogic.Create(newUser);
                    return RedirectToAction("RetrieveAll");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(newUser);
        }

        // GET: Edit User
        [HttpGet]
        public ActionResult RetrieveByID(int userId)
        {
            var user = _userLogic.RetrieveByID(userId);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View("Edit", user);
        }

        // POST: Update User
        [HttpPost]
        [AuthorizeUser]
        public ActionResult Update(Users userToUpdate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _userLogic.Update(userToUpdate);
                    return RedirectToAction("RetrieveAll");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View("Edit", userToUpdate);
        }

        // GET: Retrieve all users
        [HttpGet]
        [AuthorizeUser]
        public ActionResult RetrieveAll()
        {
            var users = _userLogic.RetrieveAll();
            return View(users);
        }

        // POST: Delete User
        [HttpPost]
        [AuthorizeUser]
        public ActionResult Delete(int userId)
        {
            try
            {
                _userLogic.Delete(userId);
                return RedirectToAction("RetrieveAll");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return RedirectToAction("RetrieveAll");
            }
        }


        // POST: Authenticate User
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            try
            {
                // Verificar si el usuario tiene la cuenta bloqueada
                var user = _authRepository.GetUserByUsername(username);
                if (user != null && user.AccountLockedUntil > DateTime.Now)
                {
                    // Si la cuenta está bloqueada
                    ViewBag.Error = $"Account is locked until {user.AccountLockedUntil}. Please try again later.";
                    return View();
                }

                // Intentar autenticar al usuario
                var sessionToken = _userLogic.Login(new LoginRequest
                {
                    Username = username,
                    Password = password
                });

                // Guardar el token en la sesión
                Session["SessionToken"] = sessionToken;

                // Reiniciar los intentos fallidos si el login es exitoso
                _authService.ResetFailedAttempts(user.UserId);

                // Registrar evento de login exitoso
                _authService.LogEvent(user.UserId, "Login Success", $"User {username} successfully logged in.");

                // Redirigir al dashboard u otra página
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                // Si el login falla, registrar el intento fallido
                _authService.RegisterFailedAttempt(username);

                // Registrar evento de login fallido
                _authService.LogEvent(null, "Login Failed", $"Failed login attempt for user {username}. Error: {ex.Message}");

                ViewBag.Error = ex.Message;
                return View();
            }
        }



        // GET: Logout
        [HttpGet]
        public ActionResult Logout()
        {
            var token = Session["SessionToken"]?.ToString();
            if (!string.IsNullOrEmpty(token))
            {
                _authService.EndSession(token);
            }

            Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Dashboard (Requiere validación de sesión)
        [HttpGet]
        [AuthorizeUser] // Custom Authorization Filter
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}