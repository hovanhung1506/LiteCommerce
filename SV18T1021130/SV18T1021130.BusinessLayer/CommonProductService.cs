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
    public class CommonProductService
    {
        private static readonly IProductDAL productDB;
        private static readonly IProductPhotoDAL productPhotoDB;
        private static readonly IProductAttributeDAL productAttributeDB;
        static CommonProductService()
        {
            string provider = ConfigurationManager.ConnectionStrings["DB"].ProviderName;
            string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;

            switch (provider)
            {
                case "SQLServer":
                    productDB = new DataLayer.SQLServer.ProductDAL(connectionString);
                    productAttributeDB = new DataLayer.SQLServer.ProductAttributeDAL(connectionString);
                    productPhotoDB = new DataLayer.SQLServer.ProductPhotoDAL(connectionString);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Lấy ra danh sách các mặt hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Product> ListOfPrudcts(int page, int pageSize, int categoryID, int supplierID, string searchValue, out int rowCount)
        {
            rowCount = productDB.Count(categoryID, supplierID, searchValue);
            return productDB.List(page, pageSize, categoryID, supplierID, searchValue).ToList();
        }
        /// <summary>
        /// Lấy ra 1 mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static Product GetProduct(int productID)
        {
            return productDB.Get(productID);
        }
        /// <summary>
        /// Bổ sung 1 mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }
        /// <summary>
        /// Chỉnh sửa 1 mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool DeleteProduct(int productID)
        {
            if (productDB.InUsed(productID))
                return false;
            return productDB.Delete(productID);
        }
        /// <summary>
        /// Kiểm tra xem 1 mặt hàng có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool InUsedProduct(int productID)
        {
            return productDB.InUsed(productID);
        }




        /// <summary>
        /// Lấy ra danh sách thư viên ảnh của 1 mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductPhoto> ListOfProductPhotos(int productID)
        {
            return productPhotoDB.List(productID).ToList();
        }
        /// <summary>
        /// Lấy ra 1 ảnh
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static ProductPhoto GetProductPhoto(int productID, int? photoID)
        {
            return productPhotoDB.Get(productID, photoID);
        }
        /// <summary>
        /// Bổ sung 1 ảnh mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddProductPhoto(ProductPhoto data)
        {
            return productPhotoDB.Add(data);
        }
        /// <summary>
        /// Chỉnh sửa 1 ảnh
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProductPhoto(ProductPhoto data)
        {
            return productPhotoDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 ảnh
        /// </summary>
        /// <param name="PhotoID"></param>
        /// <returns></returns>
        public static bool DeleteProductPhoto(int productID, int? photoID)
        {
            return productPhotoDB.Delete(productID, photoID);
        }




        /// <summary>
        /// Lấy ra danh sách thuộc tính của 1 mặt hàng
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public static List<ProductAttribute> ListOfProductAttributes(int productID)
        {
            return productAttributeDB.List(productID).ToList();
        }
        /// <summary>
        /// Bổ sung 1 thuộc tính
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddProductAttribute(ProductAttribute data)
        {
            return productAttributeDB.Add(data);
        }
        /// <summary>
        /// Lấy ra 1 thuộc tính
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public static ProductAttribute GetProductAttribute(int productID, int? attributeID)
        {
            return productAttributeDB.Get(productID, attributeID);
        }
        /// <summary>
        /// Chỉnh sửa 1 thuộc tính
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProductAttribute(ProductAttribute data)
        {
            return productAttributeDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 thuộc tính
        /// </summary>
        /// <param name="AttributeID"></param>
        /// <returns></returns>
        public static bool DeleteProductAttribute(int productID, int? attributeID)
        {
            return productAttributeDB.Delete( productID, attributeID);
        }
    }
}
