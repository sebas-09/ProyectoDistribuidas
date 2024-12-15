using Entities;
using System.Collections.Generic;

namespace ProxyLayer
{
    public interface IProductProxy
    {
        Task<Products> CreateProductAsync(Products product);
        Task<bool> DeleteProductAsync(int productId);
        Task<Products> GetProductAsync(int productId);
        Task<Products> UpdateProductAsync(Products product);
    }
}
