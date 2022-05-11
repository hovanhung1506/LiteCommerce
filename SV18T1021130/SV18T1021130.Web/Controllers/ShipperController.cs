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
    [RoutePrefix("shipper")]
    public class ShipperController : Controller
    {
        // GET: Shipper
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int page = 1, string searchValue = "")
        {
            PaginationSearchInput model = Session["SHIPPER_SEARCH"] as PaginationSearchInput;
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
            var data = CommonDataService.ListOfShippers(input.Page, input.PageSize, input.SearchValue, out rowCount);
            Models.ShipperPaginationResultModel model = new Models.ShipperPaginationResultModel
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue,
                RowCount = rowCount,
                Data = data
            };
            Session["SHIPPER_SEARCH"] = input;
            return View(model);
        }

        /// <summary>
        /// Bổ sung 1 người giao hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng mới";
            Shipper model = new Shipper()
            {
                ShipperID = 0
            };
            return View(model);
        }

        /// <summary>
        /// Chỉnh sửa 1 ngườ giao hàng
        /// </summary>
        /// <param name="shipperID"></param>
        /// <returns></returns>
        [Route("edit/{shipperID}")]
        public ActionResult Edit(string shipperID)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(shipperID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            ViewBag.Title = "Cập nhật thông tin người giao hàng";
            var model = CommonDataService.GetShipper(id);
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
        public ActionResult Save(Shipper model)
        {
            if (string.IsNullOrWhiteSpace(model.ShipperName))
                ModelState.AddModelError("ShipperName", "Tên người hàng không được để trống");
            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError("Phone", "Số điện thoại không được để trống");
            if (!ModelState.IsValid)
            {
                if (model.ShipperID > 0)
                    ViewBag.Title = "Cập nhật thông tin người giao hàng";
                else
                    ViewBag.Title = "Bổ sung người giao hàng mới";
                return View("Create", model);
            }
            if (model.ShipperID > 0)
                CommonDataService.UpdateShipper(model);
            else
                CommonDataService.AddShipper(model);
            Models.PaginationSearchInput input = new PaginationSearchInput()
            {
                Page = 1,
                PageSize = 10,
                SearchValue = model.ShipperName,
            };
            Session["SHIPPER_SEARCH"] = input;
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa 1 người giao hàng
        /// </summary>
        /// <param name="shipperID"></param>
        /// <returns></returns>
        [Route("delete/{shipperID}")]
        public ActionResult Delete(string shipperID)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(shipperID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            if(Request.HttpMethod == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetShipper(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
    }
}