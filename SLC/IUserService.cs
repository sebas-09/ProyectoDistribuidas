using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLC
{
    public interface IUserService
    {
        Users Create(Users user);

        Users RetrieveByID(int userId);

        bool Update(Users userToUpdate);

        bool Delete(int userId);
        string Login(LoginRequest request);
        string Logout(string sessionToken);





    }
}
