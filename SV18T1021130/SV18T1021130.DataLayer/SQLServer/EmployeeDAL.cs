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
    public class EmployeeDAL : BaseDAL, ICommonDAL<Employee>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public EmployeeDAL(string connectionString): base(connectionString)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Employee data)
        {
            int result = 0;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO dbo.Employees (LastName, FirstName, BirthDate, Photo, Notes, Email, Password)
                                    VALUES (@lastName, @firstName, @birthDate, @photo, @notes, @email, NULL)";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@lastName", data.LastName);
                cmd.Parameters.AddWithValue("@firstName", data.FirstName);
                cmd.Parameters.AddWithValue("@birthDate", data.BirthDate);
                cmd.Parameters.AddWithValue("@photo", data.Photo);
                cmd.Parameters.AddWithValue("@notes", data.Notes);
                cmd.Parameters.AddWithValue("@email", data.Email);
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
            {
                searchValue = "%" + searchValue + "%";
            }
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT COUNT(*)
                                    FROM Employees
                                    WHERE (@searchValue = N'')
                                          OR ((Email LIKE @searchValue)
                                              OR LastName + N' ' + FirstName LIKE @searchValue);";
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
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public bool Delete(int employeeID)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DELETE FROM dbo.Employees WHERE EmployeeID = @employeeID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@employeeID", employeeID);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public Employee Get(int employeeID)
        {
            Employee employee = new Employee();
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM dbo.Employees WHERE EmployeeID = @employeeID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@employeeID", employeeID);
                var dbReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                if (dbReader.Read())
                {
                    employee.EmployeeID = Convert.ToInt32(dbReader["EmployeeID"]);
                    employee.LastName = Convert.ToString(dbReader["LastName"]);
                    employee.FirstName = Convert.ToString(dbReader["FirstName"]);
                    employee.BirthDate = Convert.ToDateTime(dbReader["BirthDate"]);
                    employee.Photo = Convert.ToString(dbReader["Photo"]);
                    employee.Notes = Convert.ToString(dbReader["Notes"]);
                    employee.Email = Convert.ToString(dbReader["Email"]);
                    employee.Password = Convert.ToString(dbReader["Password"]);
                }
                dbReader.Close();
                cn.Close();
            }
            return employee;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public bool InUsed(int employeeID)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT CASE WHEN EXISTS (SELECT * FROM dbo.Orders WHERE EmployeeID= @employeeID) THEN 1 ELSE 0 END";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@employeeID", employeeID);
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
        public IList<Employee> List(int page, int pageSize, string searchValue)
        {
            List<Employee> data = new List<Employee>();
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT *
                                    FROM (SELECT EmployeeID, LastName, FirstName, BirthDate, Photo, Notes, Email, Password,
                                                 ROW_NUMBER() OVER (ORDER BY FirstName) AS RowNumber
                                          FROM Employees
                                          WHERE (@searchValue = N'')
                                                OR ((Email LIKE @searchValue)
                                                    OR (LastName + N' ' + FirstName LIKE @searchValue))) AS t
                                    WHERE t.RowNumber
                                    BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize
                                    ORDER BY t.RowNumber;";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                var dbReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                while (dbReader.Read())
                {
                    data.Add(new Employee()
                    {
                        EmployeeID = Convert.ToInt32(dbReader["EmployeeID"]),
                        LastName = Convert.ToString(dbReader["LastName"]),
                        FirstName = Convert.ToString(dbReader["FirstName"]),
                        BirthDate = Convert.ToDateTime(dbReader["BirthDate"]),
                        Photo = Convert.ToString(dbReader["Photo"]),
                        Notes = Convert.ToString(dbReader["Notes"]),
                        Email = Convert.ToString(dbReader["Email"]),
                        Password = Convert.ToString(dbReader["Password"])
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
        public bool Update(Employee data)
        {
            bool result = false;
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE dbo.Employees SET LastName = @lastName, FirstName = @firstName, BirthDate = @birthDate, Photo = @photo, Notes = @notes, Email = @email
                                    WHERE EmployeeID = @employeeID";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@employeeID", data.EmployeeID);
                cmd.Parameters.AddWithValue("@lastName", data.LastName);
                cmd.Parameters.AddWithValue("@firstName", data.FirstName);
                cmd.Parameters.AddWithValue("@birthDate", data.BirthDate);
                cmd.Parameters.AddWithValue("@photo", data.Photo);
                cmd.Parameters.AddWithValue("@notes", data.Notes);
                cmd.Parameters.AddWithValue("@email", data.Email);
                result = cmd.ExecuteNonQuery() > 0;
                cn.Close();
            }
            return result;
        }
    }
}
