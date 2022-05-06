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
    public interface IProductPhotoDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<ProductPhoto> List(int productID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(ProductPhoto data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        ProductPhoto Get(int productID, int? photoID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(ProductPhoto data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        bool Delete(int productID, int? photoID);
    }
}
