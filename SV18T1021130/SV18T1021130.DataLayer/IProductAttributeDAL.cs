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
    public interface IProductAttributeDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<ProductAttribute> List(int productID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(ProductAttribute data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        ProductAttribute Get(int productID, int? attributeID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(ProductAttribute data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        bool Delete(int productID, int? attributeID);
    }
}
