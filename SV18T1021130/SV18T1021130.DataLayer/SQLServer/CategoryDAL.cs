using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SV18T1021130.DataLayer.SQLServer
{
    public class CategoryDAL : BaseDAL, ICommonDAL<Category>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public CategoryDAL (string connectionString) : base(connectionString)
        {
            
        }

        /// <summary>
        /// Bổ sung 1 loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Category data)
        {
            int result = 0;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO dbo.Categories (CategoryName, Description, ParentCategoryId)
                                    VALUES (@categoryName, @description, NULL)
                                    SELECT SCOPE_IDENTITY()";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@categoryName", data.CategoryName);
                cmd.Parameters.AddWithValue("@description", data.Description);
                result = Convert.ToInt32(cmd.ExecuteScalar());
                cn.Close();
            }
            return result;
        }
        /// <summary>
        /// Đếm số loại hàng
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public int Count(string searchValue)
        {
            int count = 0;
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT COUNT(*)
                                    FROM Categories
                                    WHERE (@searchValue = N'')
                                          OR ((CategoryName LIKE @searchValue));";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                count = Convert.ToInt32(cmd.ExecuteScalar());
                cn.Close();
            }
            return count;
        }

        /// <summary>
        /// Xóa 1 loại hàng
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public bool Delete(int categoryID)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DELETE FROM dbo.Categories WHERE CategoryID = @categoryID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@categoryID", categoryID);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// Lấy ra 1 loại hàng
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public Category Get(int categoryID)
        {
            Category category = new Category();
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM dbo.Categories WHERE CategoryID = @categoryID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@categoryID", categoryID);
                var data = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (data.Read())
                {
                    category.CategoryID = Convert.ToInt32(data["CategoryID"]);
                    category.CategoryName = Convert.ToString(data["CategoryName"]);
                    category.Description = Convert.ToString(data["Description"]);
                }
                data.Close();
                cn.Close();
            }
            return category;
        }

        /// <summary>
        /// Kiểm tra 1 loại hàng có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public bool InUsed(int categoryID)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT CASE WHEN EXISTS (SELECT * FROM dbo.Products WHERE CategoryID = @categoryID) THEN 1 ELSE 0 END";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@categoryID", categoryID);
                result = Convert.ToBoolean(cmd.ExecuteScalar());
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// Lấy ra danh sách loại hàng
        /// </summary>
        /// <returns></returns>
        public IList<Category> List(int page, int pageSize, string searchValue)
        {
            List<Category> data = new List<Category>();
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";

            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select *
                                    from
                                    (
                                        select    *,
                                                row_number() over(order by CategoryName) as RowNumber
                                        from    Categories
                                        where    (@searchValue = N'')
                                            or (
                                                    (CategoryName like @searchValue)
                                                )
                                    ) as t
                                where (@pageSize = 0) or (t.RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                                order by t.RowNumber;";
                cmd.CommandType = System.Data.CommandType.Text;
                //cmd.CommandText = "ListOfCategories";
                //cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                var dbReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                while (dbReader.Read())
                {
                    data.Add(new Category()
                    {
                        CategoryID = Convert.ToInt32(dbReader["CategoryID"]),
                        CategoryName = Convert.ToString(dbReader["CategoryName"]),
                        Description = Convert.ToString(dbReader["Description"])
                    });
                }
                dbReader.Close();
                cn.Close();
            }
                return data;
        }

        /// <summary>
        /// Chỉnh sửa 1 lại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(Category data)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE dbo.Categories SET CategoryName = @categoryName, Description = @description
                                    WHERE CategoryID = @categoryID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@categoryID", data.CategoryID);
                cmd.Parameters.AddWithValue("@categoryName", data.CategoryName);
                cmd.Parameters.AddWithValue("@description", data.Description);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }
    }
}
