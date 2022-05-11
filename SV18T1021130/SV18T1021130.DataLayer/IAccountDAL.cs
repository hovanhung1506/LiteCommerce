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
        Employee Login(string email, string password);
        bool ChangePassword(string email, string newPassword);
        bool CheckPassword(string email, string oldPassword);
    }
}
