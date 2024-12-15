using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyLayer
{
    public interface IUserProxy
    {
        public interface IUserProxy
        {
            Task<Users> CreateUserAsync(Users user);
            Task<bool> DeleteUserAsync(int userId);
            Task<Users> GetUserAsync(int userId);
            Task<Users> UpdateUserAsync(Users user);
          
        }
    }
}
