using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLC
{
    public interface IProductService
    {
        Products Create(Products product);

        Products RetrieveByID(int productId);

        bool Update(Products productToUpdate);

        bool Delete(int productId);

        
    }
}
