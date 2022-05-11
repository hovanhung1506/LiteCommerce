using SV18T1021130.DataLayer;
using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV18T1021130.BusinessLayer
{
    public class CommonAccountService
    {
        public static readonly IAccountDAL accountDB;

        static CommonAccountService()
        {
            string provider = ConfigurationManager.ConnectionStrings["DB"].ProviderName;
            string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
            switch (provider)
            {
                case "SQLServer":
                    accountDB = new DataLayer.SQLServer.AccountDAL(connectionString);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Employee Login(string email, string password)
        {
            return accountDB.Login(email, password);
        }

        /// <summary>
        /// Kiểm tra mật khẩu cũ lúc đổi mật khẩu mới
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CheckPassword (string email, string oldPassword)
        {
            return accountDB.CheckPassword(email, oldPassword);
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool ChangePassword(string email, string newPassword)
        {
            return accountDB.ChangePassword(email, newPassword);
        }
    }
}
