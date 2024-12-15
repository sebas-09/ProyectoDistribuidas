using DAL;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ProductLogic
    {
        public Products Create(Products newProduct)
        {
            Products result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                Products res = r.Retrieve<Products>(p => p.ProductName == newProduct.ProductName);
                if (res == null)
                {
                    result = r.Create(newProduct);
                }
                else
                {
                    throw new Exception("Product already exists");
                }
                return result;
            }
        }

        public Products RetriveByID(int ID)
        {
            Products Result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                Result = r.Retrieve<Products>(p => p.ProductID == ID);

                return Result;
            }


        }

        public bool Update(Products productToUpdate)
        {
            bool res = false;
            using (var r = RepositoryFactory.CreateRepository())
            {
                Products temp =
                r.Retrieve<Products>(p => p.ProductName == productToUpdate.ProductName
                && p.ProductID
                != productToUpdate.ProductID);
                if (temp == null)
                {
                    res = r.Update(productToUpdate);
                }
                else
                {

                }
                return res;
            }
        }

        public bool Delete(int ID)
        {
            bool res = false;
            var product = RetriveByID(ID);
            if (product != null)
            {
                if (product.UnitsInStock == 0)
                {
                    using (var r = RepositoryFactory.CreateRepository())
                    {
                        res = r.Delete(product);
                    }
                }
                else
                {

                }



            }
            else
            {

            }
            return res;

        }

        public List<Products> FilterByCategoryID(int categotyID)
        {
            List<Products> result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                result = r.Filter<Products>(p => p.CategoryID == categotyID);
            }
            return result;
        }

    }
}

