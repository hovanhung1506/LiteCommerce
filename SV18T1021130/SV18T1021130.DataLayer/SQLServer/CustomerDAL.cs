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
    public class CustomerDAL : BaseDAL, ICommonDAL<Customer>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString"></param>
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Customer data)
        {
            int result = 0;
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO dbo.Customers (CustomerName, ContactName, Address, City, PostalCode, Country)
                                    VALUES (@customerName, @contactName, @address, @city, @postalCode, @country)
                                    SELECT SCOPE_IDENTITY();";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@customerName", data.CustomerName);
                cmd.Parameters.AddWithValue("@contactName", data.ContactName);
                cmd.Parameters.AddWithValue("@address", data.Address);
                cmd.Parameters.AddWithValue("@city", data.City);
                cmd.Parameters.AddWithValue("@postalCode", data.PostalCode);
                cmd.Parameters.AddWithValue("@country", data.Country);
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
                                    FROM Customers
                                    WHERE (@searchValue = N'')
                                          OR ((CustomerName LIKE @searchValue)
                                              OR (ContactName LIKE @searchValue)
                                              OR (Address LIKE @searchValue));";
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
        /// <param name="customerID"></param>
        /// <returns></returns>
        public bool Delete(int customerID)
        {
            bool result = false;
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DELETE FROM dbo.Customers WHERE CustomerID = @customerID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@customerID", customerID);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public Customer Get(int customerID)
        {
            Customer customer = new Customer();
            using(SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM dbo.Customers WHERE CustomerID = @customerID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@customerID", customerID);
                var data = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                if (data.Read())
                {
                    customer.CustomerID = Convert.ToInt32(data["CustomerID"]);
                    customer.CustomerName = Convert.ToString(data["CustomerName"]);
                    customer.ContactName = Convert.ToString(data["ContactName"]);
                    customer.Address = Convert.ToString(data["Address"]);
                    customer.City = Convert.ToString(data["City"]);
                    customer.PostalCode = Convert.ToString(data["PostalCode"]);
                    customer.Country = Convert.ToString(data["Country"]);
                }
                data.Close();
                cn.Close();
            }
            return customer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public bool InUsed(int customerID)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT CASE WHEN EXISTS (SELECT * FROM dbo.Orders WHERE CustomerID = @customerID) THEN 1 ELSE 0 END";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@customerID", customerID);
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
        public IList<Customer> List(int page, int pageSize, string searchValue)
        {
            List<Customer> data = new List<Customer>();
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select *
                                    from
                                    (
                                        select    *,
                                                row_number() over(order by CustomerName) as RowNumber
                                        from    Customers
                                        where    (@searchValue = N'')
                                            or (
                                                    (CustomerName like @searchValue)
                                                    or
                                                    (ContactName like @searchValue)
                                                    or
                                                    (Address like @searchValue)
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
                    data.Add(new Customer()
                    {
                        CustomerID = Convert.ToInt32(dbReader["CustomerID"]),
                        CustomerName = Convert.ToString(dbReader["CustomerName"]),
                        ContactName = Convert.ToString(dbReader["ContactName"]),
                        Address = Convert.ToString(dbReader["Address"]),
                        PostalCode = Convert.ToString(dbReader["PostalCode"]),
                        Country = Convert.ToString(dbReader["Country"]),
                        City = Convert.ToString(dbReader["City"])
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
        public bool Update(Customer data)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE dbo.Customers SET CustomerName = @customerName, ContactName = @contactName, Address = @address, City = @city, PostalCode = @postalCode, Country = @country
                                    WHERE CustomerID = @customerID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@customerID", data.CustomerID);
                cmd.Parameters.AddWithValue("@customerName", data.CustomerName);
                cmd.Parameters.AddWithValue("@contactName", data.ContactName);
                cmd.Parameters.AddWithValue("@address", data.Address);
                cmd.Parameters.AddWithValue("@city", data.City);
                cmd.Parameters.AddWithValue("@postalCode", data.PostalCode);
                cmd.Parameters.AddWithValue("@country", data.Country);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }
    }
}
