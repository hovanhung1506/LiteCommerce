using SV18T1021130.BusinessLayer;
using SV18T1021130.DomainModel;
using SV18T1021130.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SV18T1021130.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [RoutePrefix("employee")]
    public class EmployeeController : Controller
    {
        // GET: Employee
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int page = 1, string searchValue = "")
        {
            PaginationSearchInput model = Session["EMPLOYEE_SEARCH"] as PaginationSearchInput;
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
            var data = CommonDataService.ListOfEmployees(input.Page, input.PageSize, input.SearchValue, out rowCount);
            Models.EmployeePaginationResultModel model = new Models.EmployeePaginationResultModel
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue,
                RowCount = rowCount,
                Data = data
            };
            Session["EMPLOYEE_SEARCH"] = input;
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên mới";
            Employee model = new Employee()
            {
                EmployeeID = 0
            };
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        [Route("edit/{employeeID}")]
        public ActionResult Edit(string employeeID)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            int id = 0;
            try
            {
                id = Convert.ToInt32(employeeID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");

            return View("Create", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        [Route("delete/{employeeID}")]
        public ActionResult Delete(string employeeID)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(employeeID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetEmployee(id);
            if (Request.HttpMethod == "POST")
            {
                if (!string.IsNullOrEmpty(model.Photo))
                {
                    string path = Server.MapPath("~/Images/Employees");
                    string fileName = model.Photo.Split('/')[3];
                    string filePath = System.IO.Path.Combine(path, fileName);
                    CommonDataService.DeleteImage(filePath);
                }
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="birthDateString"></param>
        /// <param name="uploadPhoto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(Employee model, string birthDateString, HttpPostedFileBase uploadPhoto)
        {
            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
                ModelState.AddModelError("FullName", "Họ tên không được để trống");
            try
            {
                DateTime d = DateTime.ParseExact(birthDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model.BirthDate = d;
            }
            catch
            {
                ModelState.AddModelError("BirthDate", "Ngày sinh " + birthDateString + " không hợp lệ ");
            }
            if (string.IsNullOrEmpty(model.Email))
                ModelState.AddModelError("Email", "Email không được để trống");
            if (string.IsNullOrEmpty(model.Notes))
                model.Notes = "";
            if (uploadPhoto == null && model.EmployeeID == 0)
                ModelState.AddModelError("Photo", "Ảnh không được để trống");
            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";
                return View("Create", model);
            }
            if (uploadPhoto != null)
            {
                string path = Server.MapPath("~/Images/Employees");
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(path, fileName);
                uploadPhoto.SaveAs(filePath);
                model.Photo = $"/Images/Employees/{fileName}";
            }
            model.FirstName = model.FirstName.Trim();
            model.LastName = model.LastName.Trim();
            if (model.EmployeeID == 0)
            {
                CommonDataService.AddEmployee(model);
            }
            else
            {
                var em = CommonDataService.GetEmployee(model.EmployeeID);
                if (uploadPhoto != null)
                {
                    if (!string.IsNullOrEmpty(em.Photo))
                    {
                        string path = Server.MapPath("~/Images/Employees");
                        string fileName = em.Photo.Split('/')[3];
                        string filePath = System.IO.Path.Combine(path, fileName);
                        CommonDataService.DeleteImage(filePath);
                    }
                }
                CommonDataService.UpdateEmployee(model);
                model = CommonDataService.GetEmployee(model.EmployeeID);
            }
            Models.PaginationSearchInput input = new PaginationSearchInput()
            {
                Page = 1,
                PageSize = 10,
                SearchValue = $"{model.FirstName} {model.LastName}",
            };
            Session["EMPLOYEE_SEARCH"] = input;
            return RedirectToAction("Index");
        }
    }
}