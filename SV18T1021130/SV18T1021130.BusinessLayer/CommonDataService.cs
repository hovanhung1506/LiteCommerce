using SV18T1021130.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV18T1021130.DomainModel;
using System.Configuration;
using System.IO;

namespace SV18T1021130.BusinessLayer
{
    /// <summary>
    /// Các chức năng nghiệp vụ liên quan đến dữ liệu chung
    /// (nhà cung cấp, khách hàng, người giao hàng, nhân viên, loại hàng)
    /// </summary>
    public class CommonDataService
    {
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICountryDAL countryDB;
        /// <summary>
        /// 
        /// </summary>
        static CommonDataService()
        {
            string provider = ConfigurationManager.ConnectionStrings["DB"].ProviderName;
            string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;

            switch (provider)
            {
                case "SQLServer":
                    categoryDB = new DataLayer.SQLServer.CategoryDAL(connectionString);
                    customerDB = new DataLayer.SQLServer.CustomerDAL(connectionString);
                    supplierDB = new DataLayer.SQLServer.SupplierDAL(connectionString);
                    shipperDB = new DataLayer.SQLServer.ShipperDAL(connectionString);
                    employeeDB = new DataLayer.SQLServer.EmployeeDAL(connectionString);
                    countryDB = new DataLayer.SQLServer.CountryDAL(connectionString);
                    break;
                default:
                    categoryDB = new DataLayer.FakeDB.CategoryDAL();
                    break;
            }
        }


        /// <summary>
        /// Lấy danh sách quốc gia
        /// </summary>
        /// <returns></returns>
        public static List<Country> ListOfCountries()
        {
            return countryDB.List().ToList();
        }


        /// <summary>
        /// Xóa ảnh
        /// </summary>
        /// <param name="path">Đường dẫn</param>
        /// <returns></returns>
        public static bool DeleteImage(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// Lấy danh sách các loại hàng
        /// </summary>
        /// <returns></returns>
        public static List<Category> ListOfCategories(int page, int pageSize, string searchValue, out int rowCount)
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        /// <summary>
        /// Lấy thông tin 1 loại hàng theo mã loại hàng
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public static Category GetCategory(int categoryID)
        {
            return categoryDB.Get(categoryID);
        }
        /// <summary>
        /// Thêm 1 loại hàng mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin 1 loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 loại hàng
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int categoryID)
        {
            if (categoryDB.InUsed(categoryID))
                return false;
            return categoryDB.Delete(categoryID);
        }
        /// <summary>
        /// Kiểm tra xem 1 loại hàng có dữ liệu liên quan hay không ?
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public static bool InUsedCategory(int categoryID)
        {
            return categoryDB.InUsed(categoryID);
        }




        /// <summary>
        /// Lấy danh sách khách hàng, phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param> 0 để lấy toàn bộ
        /// <param name="searchValue"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(int page, int pageSize, string searchValue, out int rowCount)
        {
            if (pageSize < 0)
                pageSize = 0;
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách nhà cung cấp
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public static Customer GetCustomer(int customerID)
        {
            return customerDB.Get(customerID);
        }
        /// <summary>
        /// Bổ sung 1 khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin 1 khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 khách hàng
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int customerID)
        {
            if (customerDB.InUsed(customerID))
                return false;
            return customerDB.Delete(customerID);
        }
        /// <summary>
        /// Kiểm tra xem khách hàng có dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public static bool InUsedCustomer(int customerID)
        {
            return customerDB.InUsed(customerID);
        }





        /// <summary>
        /// Lấy thông tin nhà cung cấp, phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(int page, int pageSize, string searchValue, out int rowCount)
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin chi tiết 1 nhà cung cấp theo mã nhà cung cấp
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public static Supplier GetSupplier(int supplierID)
        {
            return supplierDB.Get(supplierID);
        }
        /// <summary>
        /// Bổ sung 1 nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }
        /// <summary>
        /// Cập nhật 1 nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 nhà cung cấp
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int supplierID)
        {
            if (supplierDB.InUsed(supplierID))
                return false;
            return supplierDB.Delete(supplierID);
        }
        /// <summary>
        /// Kiểm tra 1 nhà cung cấp có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public static bool InUsedSupplier(int supplierID)
        {
            return supplierDB.InUsed(supplierID);
        }





        /// <summary>
        /// Tìm kiếm và lấy danh sách người giao hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(int page, int pageSize, string searchValue, out int rowCount)
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin 1 người giao hàng theo mã người giao hàng
        /// </summary>
        /// <param name="shipperID"></param>
        /// <returns></returns>
        public static Shipper GetShipper(int shipperID)
        {
            return shipperDB.Get(shipperID);
        }
        /// <summary>
        /// Bổ sung 1 người giao hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin 1 người giao hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 người giao hàng
        /// </summary>
        /// <param name="shipperID"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int shipperID)
        {
            if (shipperDB.InUsed(shipperID))
                return false;
            return shipperDB.Delete(shipperID);
        }
        public static bool InUsedShipper(int shipperID)
        {
            return shipperDB.InUsed(shipperID);
        }





        /// <summary>
        /// Lấy danh sách nhân viên, phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(int page, int pageSize, string searchValue, out int rowCount)
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin chi tiết của nhân viên theo mã nhân viên
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public static Employee GetEmployee(int employeeID)
        {
            return employeeDB.Get(employeeID);
        }
        /// <summary>
        /// Bổ sung 1 nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }
        /// <summary>
        /// Cập nhât thông tin 1 nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 nhân viên
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int employeeID)
        {
            if (employeeDB.InUsed(employeeID))
                return false;
            return employeeDB.Delete(employeeID);
        }
        /// <summary>
        /// Kiểm tra 1 nhân viên có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public static bool InUsedEmployee(int employeeID)
        {
            return employeeDB.InUsed(employeeID);
        }
        
    }    
}
