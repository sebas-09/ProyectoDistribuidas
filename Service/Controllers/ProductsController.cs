using BLL;
using Entities;
using SecurityLayer;
using SLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Service.Controllers
{
    public class ProductsController : ApiController, IProductService
    {
        private readonly AuthService _authService;
        private readonly Sales_DBEntities _context;

        // Asegúrate de inyectar el contexto de la base de datos o crear el objeto AuthService
        public ProductsController()
        {
            var context = new Sales_DBEntities();
            _authService = new AuthService(context);
        }

        [HttpPost]
        public Products Create(Products products)  // Recibe el token de sesión desde el encabezado
        {
            var sessionToken = Request.Headers.Authorization?.Parameter;
            if (!_authService.HasPermission(sessionToken, "CreateProduct"))
            {
                throw new UnauthorizedAccessException("No tienes permiso para crear productos.");
            }

            var productLogic = new ProductLogic();
            var product = productLogic.Create(products);
            return product;
        }

        [HttpDelete]
        public bool Delete(int id)  // Recibe el token de sesión desde el encabezado
        {
            var sessionToken = Request.Headers.Authorization?.Parameter;
            if (!_authService.HasPermission(sessionToken, "DeleteProduct"))
            {
                throw new UnauthorizedAccessException("No tienes permiso para eliminar productos.");
            }

            var productLogic = new ProductLogic();
            var product = productLogic.Delete(id);
            return product;
        }

        [HttpGet]
        public Products RetrieveByID(int id)  // Recibe el token de sesión desde el encabezado
        {
            var sessionToken = Request.Headers.Authorization?.Parameter;
            if (!_authService.HasPermission(sessionToken, "ReadProduct"))
            {
                throw new UnauthorizedAccessException("No tienes permiso para ver productos.");
            }

            var productLogic = new ProductLogic();
            var product = productLogic.RetriveByID(id);
            return product;
        }

        [HttpPut]
        public bool Update(Products productToUpdate)  // Recibe el token de sesión desde el encabezado
        {
            var sessionToken = Request.Headers.Authorization?.Parameter;
            if (!_authService.HasPermission(sessionToken, "UpdateProduct"))
            {
                throw new UnauthorizedAccessException("No tienes permiso para actualizar productos.");
            }

            var productLogic = new ProductLogic();
            var result = productLogic.Update(productToUpdate);
            return result;
        }


    }
}