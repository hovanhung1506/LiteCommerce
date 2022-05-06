using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV18T1021130.DataLayer.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Product data)
        {
            int result = 0;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO dbo.Products (ProductName, SupplierID, CategoryID, Unit, Price, Photo)
                                    VALUES (@productName, DEFAULT, DEFAULT, @unit, @price, @photo)
                                    SELECT SCOPE_IDENTITY()";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@productName", data.ProductName);
                cmd.Parameters.AddWithValue("@unit", data.Unit);
                cmd.Parameters.AddWithValue("@price", data.Price);
                cmd.Parameters.AddWithValue("@photo", data.Photo);
                result = Convert.ToInt32(cmd.ExecuteScalar());
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public int Count(int categoryID, int supplierID, string searchValue)
        {
            int count = 0;
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT COUNT(*)
                                    FROM dbo.Products AS P
                                    WHERE ((@categoryID = 0) OR (P.CategoryID = @categoryID))
                                            AND ((@supplierID = 0) OR (P.SupplierID = @supplierID))
                                            AND ((@searchValue = '') OR (P.ProductName LIKE @searchValue));";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@categoryID", categoryID);
                cmd.Parameters.AddWithValue("@supplierID", supplierID);
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                count = Convert.ToInt32(cmd.ExecuteScalar());
                cn.Close();
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool Delete(int productID)
        {
            bool result = false;
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DELETE FROM dbo.Products WHERE ProductID = @productID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@productID", productID);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public Product Get(int productID)
        {
            Product product = new Product();
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM dbo.Products WHERE ProductID = @productID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@productID", productID);
                var dbReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                if (dbReader.Read())
                {
                    product.ProductID = Convert.ToInt32(dbReader["ProductID"]);
                    product.ProductName = Convert.ToString(dbReader["ProductName"]);
                    product.Unit = Convert.ToString(dbReader["Unit"]);
                    product.Price = Convert.ToString(dbReader["Price"]).Substring(0, Convert.ToString(dbReader["Price"]).Length - 2);
                    product.Photo = Convert.ToString(dbReader["Photo"]);
                }
                dbReader.Close();
                cn.Close();
            }
            return product;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool InUsed(int productID)
        {
            bool result = false;
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT CASE WHEN EXISTS (SELECT * FROM dbo.OrderDetails WHERE ProductID = @productID) THEN 1 ELSE 0 END";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@productID", productID);
                result = Convert.ToBoolean(cmd.ExecuteScalar());
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public IList<Product> List(int page, int pageSize, int categoryID, int supplierID, string searchValue)
        {
            List<Product> data = new List<Product>();
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT *
                                    FROM (SELECT *,
                                                    ROW_NUMBER() OVER (ORDER BY ProductName) AS RowNumber
                                            FROM dbo.Products AS P
                                            WHERE ((@categoryID = 0) OR (P.CategoryID = @categoryID))
                                                AND ((@supplierID = 0) OR (P.SupplierID = @supplierID))
                                                AND ((@searchValue = '') OR (P.ProductName LIKE N'%'+ @searchValue + N'%'))
	                                        ) AS t
                                    WHERE (@pageSize = 0) OR (t.RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
                                    ORDER BY t.RowNumber;";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@categoryID", categoryID);
                cmd.Parameters.AddWithValue("@supplierID", supplierID);
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                var dbReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                while (dbReader.Read())
                {
                    data.Add(new Product()
                    {
                        ProductID = Convert.ToInt32(dbReader["ProductID"]),
                        ProductName = Convert.ToString(dbReader["ProductName"]),
                        Unit = Convert.ToString(dbReader["Unit"]),
                        Price = Convert.ToString(dbReader["Price"]).Substring(0, Convert.ToString(dbReader["Price"]).Length - 2),
                        Photo = Convert.ToString(dbReader["Photo"])
                    });
                }
                dbReader.Close();
                cn.Close();
            }
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(Product data)
        {
            bool result = false;
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE dbo.Products SET ProductName = @productName, Unit = @unit, Price = @price, Photo = @photo
                                    WHERE ProductID = @productID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@productName", data.ProductName);
                cmd.Parameters.AddWithValue("@unit", data.Unit);
                cmd.Parameters.AddWithValue("@photo", data.Photo);
                cmd.Parameters.AddWithValue("@productID", data.ProductID);
                cmd.Parameters.AddWithValue("@price", data.Price);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }
    }
}
