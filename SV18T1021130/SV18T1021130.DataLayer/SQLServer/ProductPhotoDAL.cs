using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV18T1021130.DataLayer.SQLServer
{
    public class ProductPhotoDAL : BaseDAL, IProductPhotoDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public ProductPhotoDAL(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(ProductPhoto data)
        {
            int result = 0;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO dbo.ProductPhotos (ProductID, Photo, Description, DisplayOrder, IsHidden)
                                    VALUES (@productID, @photo, @description, @displayOder, @isHidden)
                                    SELECT SCOPE_IDENTITY()";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@productID", data.ProductID);
                cmd.Parameters.AddWithValue("@photo", data.Photo);
                cmd.Parameters.AddWithValue("@description", data.Description);
                cmd.Parameters.AddWithValue("@isHidden", data.IsHidden);
                cmd.Parameters.AddWithValue("@displayOder", data.DisplayOrder);
                result = Convert.ToInt32(cmd.ExecuteScalar());
                cn.Close();
            }
                return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public bool Delete(int productID, int? photoID)
        {
            bool result = false;
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DELETE FROM dbo.ProductPhotos WHERE PhotoID = @photoID AND ProductID = @productID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@photoID", photoID);
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
        /// <param name="photoID"></param>
        /// <returns></returns>
        public ProductPhoto Get(int productID, int? photoID)
        {
            ProductPhoto p = new ProductPhoto();
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM dbo.ProductPhotos WHERE PhotoID = @photoID AND ProductID = @productID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@photoID", photoID);
                cmd.Parameters.AddWithValue("@productID", productID);
                var db = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                if (db.Read())
                {
                    p.PhotoID = Convert.ToInt32(db["PhotoID"]);
                    p.ProductID = Convert.ToInt32(db["ProductID"]);
                    p.Photo = Convert.ToString(db["Photo"]);
                    p.Description = Convert.ToString(db["Description"]);
                    p.DisplayOrder = Convert.ToInt32(db["DisplayOrder"]);
                    p.IsHidden = Convert.ToString(db["IsHidden"]);
                }
                db.Close();
                cn.Close();
            }
            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public IList<ProductPhoto> List(int productID)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM dbo.ProductPhotos WHERE ProductID = @productID ORDER BY DisplayOrder";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@productID", productID);
                var db = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                while (db.Read())
                {
                    data.Add(new ProductPhoto()
                    {
                        PhotoID = Convert.ToInt32(db["PhotoID"]),
                        ProductID = Convert.ToInt32(db["ProductID"]),
                        Photo = Convert.ToString(db["Photo"]),
                        Description = Convert.ToString(db["Description"]),
                        DisplayOrder = Convert.ToInt32(db["DisplayOrder"]),
                        IsHidden = Convert.ToString(db["IsHidden"])
                    });
                }
                db.Close();
                cn.Close();
            }
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(ProductPhoto data)
        {
            bool result = false;
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE dbo.ProductPhotos SET Photo = @photo, Description = @description, DisplayOrder = @displayOrder, IsHidden = @isHidden
                                    WHERE PhotoID = @photoID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@photo", data.Photo);
                cmd.Parameters.AddWithValue("@description", data.Description);
                cmd.Parameters.AddWithValue("@displayOrder", data.DisplayOrder);
                cmd.Parameters.AddWithValue("@photoID", data.PhotoID);
                cmd.Parameters.AddWithValue("@isHidden", data.IsHidden);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }
    }
}
