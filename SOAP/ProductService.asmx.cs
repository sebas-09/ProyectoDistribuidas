using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SOAP
{
    /// <summary>
    /// Descripción breve de ProductService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class ProductService : System.Web.Services.WebService
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Sales_DB"].ConnectionString;

        // Crear un producto
        [WebMethod]
        public string CreateProduct(string productName, int categoryId, decimal unitPrice, int unitsInStock)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Products (ProductName, CategoryID, UnitPrice, UnitsInStock) VALUES (@ProductName, @CategoryID, @UnitPrice, @UnitsInStock)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductName", productName);
                cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                cmd.Parameters.AddWithValue("@UnitsInStock", unitsInStock);

                conn.Open();
                cmd.ExecuteNonQuery();
                return "Product created successfully.";
            }
        }

        // Leer un producto por ID
        [WebMethod]
        public DataSet GetProduct(int productId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Products WHERE ProductID = @ProductID";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@ProductID", productId);

                DataSet productData = new DataSet();
                adapter.Fill(productData);
                return productData;
            }
        }

        // Actualizar un producto
        [WebMethod]
        public string UpdateProduct(int productId, string productName, int categoryId, decimal unitPrice, int unitsInStock)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Products SET ProductName = @ProductName, CategoryID = @CategoryID, UnitPrice = @UnitPrice, UnitsInStock = @UnitsInStock WHERE ProductID = @ProductID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductID", productId);
                cmd.Parameters.AddWithValue("@ProductName", productName);
                cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                cmd.Parameters.AddWithValue("@UnitsInStock", unitsInStock);

                conn.Open();
                cmd.ExecuteNonQuery();
                return "Product updated successfully.";
            }
        }

        // Eliminar un producto
        [WebMethod]
        public string DeleteProduct(int productId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Products WHERE ProductID = @ProductID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductID", productId);

                conn.Open();
                cmd.ExecuteNonQuery();
                return "Product deleted successfully.";
            }
        }
    }
}
