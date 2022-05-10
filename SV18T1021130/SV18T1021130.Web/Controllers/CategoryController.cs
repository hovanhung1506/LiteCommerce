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
    [RoutePrefix("category")]
    public class CategoryController : Controller
    {
        // GET: Category
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int page = 1, string searchValue = "")
        {
            PaginationSearchInput model = Session["CATEGORY_SEARCH"] as PaginationSearchInput;
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
            var data = CommonDataService.ListOfCategories(input.Page, input.PageSize, input.SearchValue, out rowCount);
            Models.CategoryPaginationResultModel model = new Models.CategoryPaginationResultModel
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue,
                RowCount = rowCount,
                Data = data
            };
            Session["CATEGORY_SEARCH"] = input;
            return View(model);
        }

        /// <summary>
        /// Bổ sung 1 loại hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng mới";
            Category model = new Category()
            {
                CategoryID = 0
            };
            return View(model);
        }

        /// <summary>
        /// Chỉnh sửa 1 loại hàng
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        [Route("edit/{categoryID}")]
        public ActionResult Edit(string categoryID)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(categoryID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            var model = CommonDataService.GetCategory(id);
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
        public ActionResult Save(Category model)
        {
            if (string.IsNullOrWhiteSpace(model.CategoryName))
                ModelState.AddModelError("CategoryName", "Tên loại hàng không được để trống");
            if (string.IsNullOrWhiteSpace(model.Description))
                model.Description = "";
            if (!ModelState.IsValid)
            {
                if(model.CategoryID > 0)
                    ViewBag.Title = "Cập nhật thông tin loại hàng";
                else
                    ViewBag.Title = "Bổ sung loại hàng mới";
                return View("Create", model);
            }
            if (model.CategoryID > 0)
            {
                CommonDataService.UpdateCategory(model);
                model = CommonDataService.GetCategory(model.CategoryID);
            }
            else
            {
                CommonDataService.AddCategory(model);
            }
            Models.PaginationSearchInput input = new PaginationSearchInput()
            {
                Page = 1,
                PageSize = 10,
                SearchValue = model.CategoryName
            };
            Session["CATEGORY_SEARCH"] = input;
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa 1 loại hàng
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        [Route("delete/{categoryID}")]
        public ActionResult Delete(string  categoryID)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(categoryID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            if (Request.HttpMethod == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCategory(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
    }
}