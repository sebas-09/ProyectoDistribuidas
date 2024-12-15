using SecurityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FrontLayer.Controllers
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        private readonly AuthService _authService;

        public AuthorizeUserAttribute()
        {
            _authService = new AuthService();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionToken = filterContext.HttpContext.Session["SessionToken"]?.ToString();

            if (string.IsNullOrEmpty(sessionToken) || !_authService.IsSessionValid(sessionToken))
            {
                filterContext.Result = new RedirectResult("/User/Login");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}