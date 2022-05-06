using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV18T1021130.DataLayer.SQLServer
{
    /// <summary>
    /// 
    /// </summary>
    public class ProductAttributeDAL : BaseDAL, IProductAttributeDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public ProductAttributeDAL(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(ProductAttribute data)
        {
            int result = 0;
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO dbo.ProductAttributes (ProductID, AttributeName, AttributeValue, DisplayOrder)
                                    VALUES (@productID, @attributeName, @attributeValue, @displayOrder)
                                    SELECT SCOPE_IDENTITY()";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@productID", data.ProductID);
                cmd.Parameters.AddWithValue("@attributeName", data.AttributeName);
                cmd.Parameters.AddWithValue("@attributeValue", data.AttributeValue);
                cmd.Parameters.AddWithValue("@displayOrder", data.DisplayOrder);
                result = Convert.ToInt32(cmd.ExecuteScalar());
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="AttributeID"></param>
        /// <returns></returns>
        public bool Delete(int productID, int? AttributeID)
        {
            bool result = false;
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DELETE FROM dbo.ProductAttributes WHERE AttributeID = @attributeID AND ProductID = @productID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@attributeID", AttributeID);
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
        /// <param name="AttributeID"></param>
        /// <returns></returns>
        public ProductAttribute Get(int productID, int? AttributeID)
        {
            ProductAttribute p = new ProductAttribute();
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM dbo.ProductAttributes WHERE AttributeID = @attributeID AND ProductID = @productID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@attributeID", AttributeID);
                cmd.Parameters.AddWithValue("@productID", productID);
                var db = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                if (db.Read())
                {
                    p.AttributeID = Convert.ToInt32(db["AttributeID"]);
                    p.ProductID = Convert.ToInt32(db["ProductID"]);
                    p.AttributeName = Convert.ToString(db["AttributeName"]);
                    p.AttributeValue = Convert.ToString(db["AttributeValue"]);
                    p.DisplayOrder = Convert.ToInt32(db["DisplayOrder"]);
                }
                cn.Close();
            }
            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public IList<ProductAttribute> List(int productID)
        {
            List<ProductAttribute> data = new List<ProductAttribute>();
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM dbo.ProductAttributes
                                    WHERE ProductID = @productID
                                    ORDER BY DisplayOrder";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@productID", productID);
                var dbReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                while (dbReader.Read())
                {
                    data.Add(new ProductAttribute()
                    {
                        AttributeID = Convert.ToInt32(dbReader["AttributeID"]),
                        ProductID = Convert.ToInt32(dbReader["ProductID"]),
                        AttributeName = Convert.ToString(dbReader["AttributeName"]),
                        AttributeValue = Convert.ToString(dbReader["AttributeValue"]),
                        DisplayOrder = Convert.ToInt32(dbReader["DisplayOrder"])
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
        public bool Update(ProductAttribute data)
        {
            bool result = false;
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE dbo.ProductAttributes SET AttributeName = @attributeName, AttributeValue = @attributeValue, DisplayOrder = @displayOrder
                                    WHERE AttributeID = @attributeID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@attributeName", data.AttributeName);
                cmd.Parameters.AddWithValue("@attributeValue", data.AttributeValue);
                cmd.Parameters.AddWithValue("@displayOrder", data.DisplayOrder);
                cmd.Parameters.AddWithValue("@attributeID", data.AttributeID);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }
    }
}
