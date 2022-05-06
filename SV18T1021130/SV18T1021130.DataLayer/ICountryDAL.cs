using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV18T1021130.DataLayer
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICountryDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<Country> List();
    }
}
