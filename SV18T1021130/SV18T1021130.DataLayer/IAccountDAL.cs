using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV18T1021130.DataLayer
{
    public interface IAccountDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Employee Login(string email, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        bool ChangePassword(string email, string newPassword);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="oldPassword"></param>
        /// <returns></returns>
        bool CheckPassword(string email, string oldPassword);
    }
}
