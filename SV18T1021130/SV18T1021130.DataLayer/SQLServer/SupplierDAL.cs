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
    public class SupplierDAL : BaseDAL , ICommonDAL<Supplier>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public SupplierDAL(string connectionString) : base(connectionString)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Supplier data)
        {
            int result = 0;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO dbo.Suppliers (SupplierName, ContactName, Address, City, PostalCode, Country, Phone)
                                    VALUES (@supplierName, @contactName, @address, @city, @postalCode, @country, @phone)
                                    SELECT SCOPE_IDENTITY()";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@supplierName", data.SupplierName);
                cmd.Parameters.AddWithValue("@contactName", data.ContactName);
                cmd.Parameters.AddWithValue("@address", data.Address);
                cmd.Parameters.AddWithValue("@city", data.City);
                cmd.Parameters.AddWithValue("@postalCode", data.PostalCode);
                cmd.Parameters.AddWithValue("@country", data.Country);
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
                                    FROM Suppliers
                                    WHERE (@searchValue = N'')
                                          OR ((SupplierName LIKE @searchValue)
                                              OR (ContactName LIKE @searchValue)
                                              OR (Address LIKE @searchValue)
                                                OR (Phone like @searchValue));";
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
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public bool Delete(int supplierID)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DELETE FROM dbo.Suppliers WHERE SupplierID = @supplierID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@supplierID", supplierID);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public Supplier Get(int supplierID)
        {
            Supplier supplier = new Supplier();
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM dbo.Suppliers WHERE SupplierID = @supplierID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@supplierID", supplierID);
                var data = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                if(data.Read())
                {
                    supplier.SupplierID = Convert.ToInt32(data["SupplierID"]);
                    supplier.SupplierName = Convert.ToString(data["SupplierName"]);
                    supplier.ContactName = Convert.ToString(data["ContactName"]);
                    supplier.Address = Convert.ToString(data["Address"]);
                    supplier.City = Convert.ToString(data["City"]);
                    supplier.PostalCode = Convert.ToString(data["PostalCode"]);
                    supplier.Country = Convert.ToString(data["Country"]);
                    supplier.Phone = Convert.ToString(data["Phone"]);
                }
                data.Close();
                cn.Close();
            }
            return supplier;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public bool InUsed(int supplierID)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT CASE WHEN EXISTS (SELECT * FROM dbo.Products WHERE SupplierID = @supplierID) THEN 1 ELSE 0 END";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@supplierID", supplierID);
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
        public IList<Supplier> List(int page, int pageSize, string searchValue)
        {
            List<Supplier> data = new List<Supplier>();
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select *
                                    from
                                    (
                                        select    *,
                                                row_number() over(order by SupplierName) as RowNumber
                                        from    Suppliers
                                        where    (@searchValue = N'')
                                            or (
                                                    (SupplierName like @searchValue)
                                                    or
                                                    (ContactName like @searchValue)
                                                    or
                                                    (Address like @searchValue)
                                                    or((Phone like @searchValue))
                                                )
                                    ) as t
                                where (@pageSize = 0) or (t.RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                                order by t.RowNumber;";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                var dbReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                while (dbReader.Read())
                {
                    data.Add(new Supplier
                    {
                        SupplierID = Convert.ToInt32(dbReader["SupplierID"]),
                        SupplierName = Convert.ToString(dbReader["SupplierName"]),
                        ContactName = Convert.ToString(dbReader["ContactName"]),
                        Address = Convert.ToString(dbReader["Address"]),
                        City = Convert.ToString(dbReader["City"]),
                        PostalCode = Convert.ToString(dbReader["PostalCode"]),
                        Country = Convert.ToString(dbReader["Country"]),
                        Phone = Convert.ToString(dbReader["Phone"])
                    });
                }
                dbReader.Close();
                cn.Close();
            };
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(Supplier data)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE dbo.Suppliers SET SupplierName = @supplierName, ContactName = @contactName, Address = @address, City = @city, PostalCode = @postalCode, Country = @country, Phone = @phone
                                    WHERE SupplierID = @supplierID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@supplierId", data.SupplierID);
                cmd.Parameters.AddWithValue("@supplierName", data.SupplierName);
                cmd.Parameters.AddWithValue("@contactName", data.ContactName);
                cmd.Parameters.AddWithValue("@address", data.Address);
                cmd.Parameters.AddWithValue("@city", data.City);
                cmd.Parameters.AddWithValue("@postalCode", data.PostalCode);
                cmd.Parameters.AddWithValue("@country", data.Country);
                cmd.Parameters.AddWithValue("@phone", data.Phone);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }
    }
}
