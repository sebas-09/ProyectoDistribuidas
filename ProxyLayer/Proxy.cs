using SLC;
using Entities;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System;

namespace ProxyLayer
{
    public class Proxy : IProductService
    {
        private readonly string BaseAddress = "http://localhost:52468"; // Ajusta según tu configuración del servicio.

        // Métodos para interactuar con el servicio SOAP o REST se implementarán aquí.

        public async Task<Products> CreateAsync(Products newProduct)
        {
            return await SendPost<Products, Products>
            ("/api/Products/Create", newProduct);
        }
        public Products Create(Products newProduct)
        {
            Products Result = null;
            // Ejecutar la tarea en un nuevo hilo
            // para que no se bloquee el hilo síncrono
            // con Wait esperamos la operación asíncrona
            Task.Run(async () => Result =
            await CreateAsync(newProduct)).Wait();
            return Result;
        }
        public async Task<bool> UpdateAsync(Products productToUpdate)
        {
            return await SendPost<bool, Products>
            ("/api/Products/Update", productToUpdate);
        }
        public bool Update(Products productToUpdate)
        {
            bool Result = false;
            Task.Run(async () => Result = await
            UpdateAsync(productToUpdate)).Wait();
            return Result;
        }
        public async Task<bool> DeleteAsync(int ID)
        {
            return await SendGet<bool>($"/api/Products/Delete/{ID}");
        }
        public bool Delete(int ID)
        {
            bool Result = false;
            Task.Run(async () => Result = await DeleteAsync(ID)).Wait();
            return Result;
        }

        public async Task<Products> RetrieveByIDAsync(int ID)
        {
            return await SendGet<Products>($"/api/Products/RetrieveByID/{ID}");
        }
        public Products RetrieveByID(int ID)
        {
            Products Result = null;
            Task.Run(async () =>
            {
                Result = await RetrieveByIDAsync(ID);
            }).Wait(); // Esperamos la tarea asincrónica
            return Result;
        }


        public async Task<T> SendPost<T, PostData>
            (string requestURI, PostData data)
        {
            T Result = default(T);
            using (var Client = new HttpClient())
            {
                try
                {
                    // URL Absoluto
                    requestURI = BaseAddress + requestURI;
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add
                    (new MediaTypeWithQualityHeaderValue("application/json"));
                    var JSONData = JsonConvert.SerializeObject(data);
                    HttpResponseMessage Response =
                    await Client.PostAsync(requestURI,
                    new StringContent(JSONData.ToString(),
                    Encoding.UTF8, "application/json"));
                    var ResultWebAPI = await Response.Content.ReadAsStringAsync();
                    Result = JsonConvert.DeserializeObject<T>(ResultWebAPI);
                }
                catch (Exception ex)
                {
                    // Manejar la excepción
                }
            }
            return Result;
        }

        public async Task<T> SendGet<T>(string requestURI)
        {
            T Result = default(T);
            using (var Client = new HttpClient())
            {
                try
                {
                    requestURI = BaseAddress + requestURI; // URL Absoluto
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Usamos GetAsync para controlar el código de estado HTTP
                    var Response = await Client.GetAsync(requestURI);
                    if (Response.IsSuccessStatusCode)  // Verifica si la respuesta fue exitosa
                    {
                        var ResultJSON = await Response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(ResultJSON))  // Asegúrate de que la respuesta no esté vacía
                        {
                            Result = JsonConvert.DeserializeObject<T>(ResultJSON);
                        }
                        else
                        {
                            Console.WriteLine("La respuesta está vacía.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error en la solicitud HTTP: {Response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en la solicitud: {ex.Message}");
                }
            }
            return Result;
        }


    }
}
