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
    public class CountryDAL : BaseDAL, ICountryDAL
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString"></param>
        public CountryDAL(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Lấy ra danh sách các quốc gia
        /// </summary>
        /// <returns></returns>
        public IList<Country> List()
        {
            List<Country> data = new List<Country>();
            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM dbo.Countries";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = cn;
                var dbReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                while (dbReader.Read())
                {
                    data.Add(new Country()
                    {
                        CountryName = Convert.ToString(dbReader["CountryName"])
                    });
                }
                dbReader.Close();
                cn.Close();
            }
                return data;
        }
    }
}
