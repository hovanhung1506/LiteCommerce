using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV18T1021130.DataLayer.SQLServer
{
    public class ShipperDAL : BaseDAL, ICommonDAL<Shipper>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public ShipperDAL(string connectionString) : base(connectionString) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Shipper data)
        {
            int result = 0;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO dbo.Shippers (ShipperName, Phone)
                                    VALUES (@shipperName, @phone)
                                    SELECT SCOPE_IDENTITY()";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@shipperName", data.ShipperName);
                cmd.Parameters.AddWithValue("@phone", data.Phone);
                result = Convert.ToInt32(cmd.ExecuteScalar());
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// 
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
                                    FROM Shippers
                                    WHERE (@searchValue = N'')
                                          OR ((ShipperName LIKE @searchValue)
                                              OR (Phone LIKE @searchValue));";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                count = Convert.ToInt32(cmd.ExecuteScalar());
                cn.Close();
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shipperID"></param>
        /// <returns></returns>
        public bool Delete(int shipperID)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DELETE FROM dbo.Shippers WHERE ShipperID = @shipperID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@shipperID", shipperID);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shipperID"></param>
        /// <returns></returns>
        public Shipper Get(int shipperID)
        {
            Shipper shipper = new Shipper();
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM dbo.Shippers WHERE ShipperID = @shipperID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@shipperID", shipperID);
                var data = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                if (data.Read())
                {
                    shipper.ShipperID = Convert.ToInt32(data["ShipperID"]);
                    shipper.ShipperName = Convert.ToString(data["ShipperName"]);
                    shipper.Phone = Convert.ToString(data["Phone"]);
                }
                data.Close();
                cn.Close();
            }
            return shipper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shipperID"></param>
        /// <returns></returns>
        public bool InUsed(int shipperID)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT CASE WHEN EXISTS (SELECT * FROM dbo.Orders WHERE ShipperID = @shipperID) THEN 1 ELSE 0 END";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@shipperID", shipperID);
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
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public IList<Shipper> List(int page, int pageSize, string searchValue)
        {
            List<Shipper> data = new List<Shipper>();
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select *
                                    from
                                    (
                                        select    *,
                                                row_number() over(order by ShipperName) as RowNumber
                                        from    Shippers
                                        where    (@searchValue = N'')
                                            or (
                                                    (ShipperName like @searchValue)
                                                    or
                                                    (Phone like @searchValue)
                                                )
                                    ) as t
                                where    t.RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize
                                order by t.RowNumber;";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                var dbReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                while (dbReader.Read())
                {
                    data.Add(new Shipper()
                    {
                        ShipperID = Convert.ToInt32(dbReader["ShipperID"]),
                        ShipperName = Convert.ToString(dbReader["ShipperName"]),
                        Phone = Convert.ToString(dbReader["Phone"])
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
        public bool Update(Shipper data)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE dbo.Shippers SET ShipperName = @shipperName, Phone = @phone
                                    WHERE ShipperID = @shipperID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@shipperID", data.ShipperID);
                cmd.Parameters.AddWithValue("@shipperName", data.ShipperName);
                cmd.Parameters.AddWithValue("@phone", data.Phone);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }
    }
}
