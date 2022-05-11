using SV18T1021130.BusinessLayer;
using SV18T1021130.DomainModel;
using SV18T1021130.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SV18T1021130.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [RoutePrefix("customer")]
    public class CustomerController : Controller
    {
        // GET: Customer
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public ActionResult Index(int page = 1, string searchValue = "")
        {
            PaginationSearchInput model = Session["CUSTOMER_SEARCH"] as PaginationSearchInput;
            if (model == null)
            {
                model = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = 10,
                    SearchValue = ""
                };
            }
            return View(model);
        }

        /// <summary>
        /// Tìm kiếm phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ActionResult Search(Models.PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCustomers(input.Page, input.PageSize, input.SearchValue, out rowCount);
            Models.CustomerPaginationResultModel model = new Models.CustomerPaginationResultModel
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue,
                RowCount = rowCount,
                Data = data
            };
            Session["CUSTOMER_SEARCH"] = input;
            return View(model);
        }

        /// <summary>
        /// Bo sung khach hang
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng mới";
            Customer model = new Customer()
            {
                CustomerID = 0
            };
            return View(model);
        }

        /// <summary>
        /// Thay doi thong tin khach hang
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        [Route("edit/{customerID}")]
        public ActionResult Edit(string customerID)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(customerID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            // Sử dụng view của create
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");
            return View("Create", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(Customer model)
        {
            if (string.IsNullOrWhiteSpace(model.CustomerName))
                ModelState.AddModelError("CustomerName", "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(model.ContactName))
                ModelState.AddModelError("ContactName", "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(model.Address))
                ModelState.AddModelError("Address", "Địa chỉ không được để trống");
            if (string.IsNullOrWhiteSpace(model.Country))
                ModelState.AddModelError("Country", "Quốc gia không được để trống");
            if (string.IsNullOrWhiteSpace(model.City))
                model.City = "";
            if (string.IsNullOrWhiteSpace(model.PostalCode))
                model.PostalCode = "";
            //Nếu dữ liệu đầu vào không hợp lệ thì trả lại giao diện và thông báo lỗi
            if (!ModelState.IsValid)
            {
                if (model.CustomerID > 0)
                    ViewBag.Title = "Cập nhật thông tin khách hàng";
                else
                    ViewBag.Title = "Bổ sung khách hàng mới";
                return View("Create", model);
            }
            if (model.CustomerID > 0)
            {
                CommonDataService.UpdateCustomer(model);
                //model = CommonDataService.GetCustomer(model.CustomerID);
            }
            else
            {
                CommonDataService.AddCustomer(model);
            }
            Models.PaginationSearchInput input = new PaginationSearchInput()
            {
                Page = 1,
                PageSize = 10,
                SearchValue = model.CustomerName
            };
            Session["CUSTOMER_SEARCH"] = input;
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xoa khach hang
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        [Route("delete/{customerID}")]
        public ActionResult Delete(string customerID)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(customerID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            if (Request.HttpMethod == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }


    }
}