using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using SoapServiceReference;

namespace ProxyLayer
{
    public class ProductSoapProxy : IProductProxy
    {
        private readonly ProductServiceSoapClient _soapClient;

        // Constructor que inicializa el cliente SOAP
        public ProductSoapProxy()
        {
            _soapClient = new ProductServiceSoapClient(ProductServiceSoapClient.EndpointConfiguration.ProductServiceSoap);
        }

        // Implementación del método CreateProductAsync
        public async Task<Products> CreateProductAsync(Products product)
        {
            var response = await _soapClient.CreateProductAsync(product.ProductName, product.CategoryID, product.UnitPrice, product.UnitsInStock);
            if (response == "Product created successfully.")
            {
                return product; // Retorna el producto creado
            }
            else
            {
                throw new Exception("Error creating product via SOAP service");
            }
        }

        // Implementación del método DeleteProductAsync
        public async Task<bool> DeleteProductAsync(int productId)
        {
            var response = await _soapClient.DeleteProductAsync(productId);
            return response == "Product deleted successfully.";
        }

        // Implementación del método GetProductAsync
        public async Task<Products> GetProductAsync(int productId)
        {
            var result = await _soapClient.GetProductAsync(productId);
            var product = result.Nodes.FirstOrDefault(); // Solo tomamos el primer elemento, ya que es único

            if (product != null)
            {
                return new Products
                {
                    ProductID = int.Parse(product.Element("ProductID")?.Value),
                    ProductName = product.Element("ProductName")?.Value,
                    CategoryID = int.Parse(product.Element("CategoryID")?.Value),
                    UnitPrice = decimal.Parse(product.Element("UnitPrice")?.Value),
                    UnitsInStock = int.Parse(product.Element("UnitsInStock")?.Value)
                };
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        // Implementación del método UpdateProductAsync
        public async Task<Products> UpdateProductAsync(Products product)
        {
            var response = await _soapClient.UpdateProductAsync(product.ProductID, product.ProductName, product.CategoryID, product.UnitPrice, product.UnitsInStock);
            if (response == "Product updated successfully.")
            {
                return product; // Retorna el producto actualizado
            }
            else
            {
                throw new Exception("Error updating product via SOAP service");
            }
        }
    }
}
