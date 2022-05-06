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
    public abstract class BaseDAL
    {
        /// <summary>
        /// Chuỗi tham số kết nối CSDL SQLServer
        /// </summary>
        protected string _connectioString;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseDAL(string connectionString)
        {
            _connectioString = connectionString;
        }

        /// <summary>
        /// Tạo và mở 1 kết nối CSDL
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = _connectioString;
            cn.Open();
            return cn;
        }
    }
}
