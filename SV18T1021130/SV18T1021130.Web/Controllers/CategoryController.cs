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
        /// 
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
        /// 
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
                Session["cate"] = model;
            }
            else
            {
                CommonDataService.AddCategory(model);
                int rowCount = 0;
                var list = CommonDataService.ListOfCategories(1, 0, "", out rowCount).ToArray();

                int index = 0, max = list[0].CategoryID;
                for(int i = 0; i < list.Length; i++)
                {
                    if(max < list[i].CategoryID)
                    {
                        max = list[i].CategoryID;
                        index = i;
                    }
                }
                Session["cate"] = list[index];
            }
                
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
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