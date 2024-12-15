using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProxyLayer
{
    public class UserRestProxy : IUserProxy
    {
        private readonly string _baseUrl = "http://localhost:52468/api/User/"; // Cambia por la URL real de tu servicio REST.

        public async Task<Users> CreateUserAsync(Users user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsJsonAsync("", user);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<Users>();
                }

                throw new Exception("Error creating user: " + response.ReasonPhrase);
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                var response = await client.DeleteAsync($"/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                throw new Exception("Error deleting user: " + response.ReasonPhrase);
            }
        }

        public async Task<Users> GetUserAsync(int userId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                var response = await client.GetAsync($"/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<Users>();
                }

                throw new Exception("Error fetching user: " + response.ReasonPhrase);
            }
        }


        public async Task<Users> UpdateUserAsync(Users user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                var response = await client.PutAsJsonAsync("", user);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<Users>();
                }

                throw new Exception("Error updating user: " + response.ReasonPhrase);
            }
        }
    }
}
