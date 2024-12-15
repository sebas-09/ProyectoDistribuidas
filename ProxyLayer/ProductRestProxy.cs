using Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ProxyLayer
{
    public class ProductRestProxy : IProductProxy
    {
        private readonly string _baseUrl = "http://localhost:52468/api/User/Products"; // Cambia por la URL real de tu servicio REST.

        public async Task<Products> CreateProductAsync(Products product)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsJsonAsync("", product);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<Products>();
                }

                throw new Exception("Error creating product: " + response.ReasonPhrase);
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                var response = await client.DeleteAsync($"/{productId}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                throw new Exception("Error deleting product: " + response.ReasonPhrase);
            }
        }

        public async Task<Products> GetProductAsync(int productId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                var response = await client.GetAsync($"/{productId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<Products>();
                }

                throw new Exception("Error fetching product: " + response.ReasonPhrase);
            }
        }


        public async Task<Products> UpdateProductAsync(Products product)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                var response = await client.PutAsJsonAsync("", product);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<Products>();
                }

                throw new Exception("Error updating product: " + response.ReasonPhrase);
            }
        }
    }
}
