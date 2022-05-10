using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SV18T1021130.BusinessLayer;
using SV18T1021130.DomainModel;
using SV18T1021130.Web.Models;

namespace SV18T1021130.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [RoutePrefix("supplier")]
    public class SupplierController : Controller
    {
        // GET: Supplier
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int page = 1, string searchValue = "")
        {
            PaginationSearchInput model = Session["SUPPLIER_SEARCH"] as PaginationSearchInput;
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
            var data = CommonDataService.ListOfSuppliers(input.Page, input.PageSize, input.SearchValue, out rowCount);
            Models.SupplierPaginationResultModel model = new Models.SupplierPaginationResultModel
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue,
                RowCount = rowCount,
                Data = data
            };
            Session["SUPPLIER_SEARCH"] = input;
            return View(model);
        }

        /// <summary>
        /// Bổ sung 1 nhà cung cấp
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            Supplier model = new Supplier()
            {
                SupplierID = 0
            };

            return View(model);
        }

        /// <summary>
        /// Chỉnh sửa 1 nhà cung cấp
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        [Route("edit/{supplierID}")]
        public ActionResult Edit(string supplierID)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(supplierID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            var model = CommonDataService.GetSupplier(id);
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
        public ActionResult Save(Supplier model)
        {
            if (string.IsNullOrWhiteSpace(model.SupplierName))
                ModelState.AddModelError("SupplierName", "Tên nhà cung cấp không được để trống");
            if (string.IsNullOrWhiteSpace(model.ContactName))
                ModelState.AddModelError("ContactName", "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(model.Address))
                ModelState.AddModelError("Address", "Địa chỉ không được để trống");
            if (string.IsNullOrWhiteSpace(model.City))
                ModelState.AddModelError("City", "Thành phố không được để trống");
            if (string.IsNullOrWhiteSpace(model.PostalCode))
                ModelState.AddModelError("PostalCode", "Mã bưu chính không được để trống");
            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError("Phone", "Số điện thoại không được để trống");
            if (!ModelState.IsValid)
            {
                if (model.SupplierID > 0)
                    ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
                else
                    ViewBag.Title = "Bổ sung nhà cung cấp";
                return View("Create", model);
            }
            if (model.SupplierID > 0)
            {
                CommonDataService.UpdateSupplier(model);
                model = CommonDataService.GetSupplier(model.SupplierID);
            }
            else
                CommonDataService.AddSupplier(model);
            PaginationSearchInput input = new PaginationSearchInput()
            {
                Page = 1,
                PageSize = 10,
                SearchValue = model.SupplierName
            };
            Session["SUPPLIER_SEARCH"] = input;
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa 1 nhà cung cấp
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        [Route("delete/{supplierID}")]
        public ActionResult Delete(string supplierID)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(supplierID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            if (Request.HttpMethod == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetSupplier(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
    }
}