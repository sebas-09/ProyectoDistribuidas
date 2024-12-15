using Entities;
using Newtonsoft.Json;
using SLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProxyLayer
{
    public class UserProxy  
    {

        private readonly string BaseAddress = "http://localhost:52468";
        // Método para crear un usuario
        public Users Create(Users user)
        {
            return Task.Run(async () => await SendPost<Users, Users>("/api/user", user)).Result;
        }

        // Método para obtener un usuario por ID
        public Users RetrieveByID(int userId)
        {
            return Task.Run(async () => await SendGet<Users>($"/api/user/{userId}")).Result;
        }

        // Método para actualizar un usuario
        public bool Update(Users userToUpdate)
        {
            return Task.Run(async () => await SendPost<bool, Users>("/api/user/update", userToUpdate)).Result;
        }

        // Método para eliminar un usuario
        public bool Delete(int userId)
        {
            return Task.Run(async () => await SendPost<bool, int>("/api/user/delete", userId)).Result;
        }

        

        public async Task<T> SendPost<T, PostData>(string requestURI, PostData data)
        {
            T result = default;
            using (var client = new HttpClient())
            {
                try
                {
                    requestURI = BaseAddress + requestURI;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var jsonData = JsonConvert.SerializeObject(data);
                    HttpResponseMessage response = await client.PostAsync(requestURI, new StringContent(jsonData, Encoding.UTF8, "application/json"));
                    var resultWebAPI = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<T>(resultWebAPI);
                }
                catch (Exception ex)
                {
                    // Manejar excepción
                }
            }
            return result;
        }
        public async Task<T> SendGet<T>(string requestURI)
        {
            T result = default;
            using (var client = new HttpClient())
            {
                try
                {
                    requestURI = BaseAddress + requestURI;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var resultJSON = await client.GetStringAsync(requestURI);
                    result = JsonConvert.DeserializeObject<T>(resultJSON);
                }
                catch (Exception ex)
                {
                    // Manejar excepción
                }
            }
            return result;
        }
        private async Task<T> SendPut<T, PostData>(string requestURI, PostData data)
        {
            T result = default;
            using (var client = new HttpClient())
            {
                requestURI = BaseAddress + requestURI;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jsonData = JsonConvert.SerializeObject(data);
                HttpResponseMessage response = await client.PutAsync(requestURI, new StringContent(jsonData, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var resultJSON = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<T>(resultJSON);
                }
            }
            return result;
        }

        private async Task<T> SendDelete<T>(string requestURI)
        {
            T result = default;
            using (var client = new HttpClient())
            {
                requestURI = BaseAddress + requestURI;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.DeleteAsync(requestURI);
                if (response.IsSuccessStatusCode)
                {
                    var resultJSON = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<T>(resultJSON);
                }
            }
            return result;
        }
        // Método para el login
        public Users Login(LoginRequest request)
        {
            var username = request.Username;
            var password = request.Password;
            var loginRequest = new { Username = username, Password = password };
            return Task.Run(async () => await SendPost<Users, object>("/api/user/login", loginRequest)).Result;
        }



    }
}