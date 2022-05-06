using SV18T1021130.BusinessLayer;
using SV18T1021130.DomainModel;
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
            int pageSize = 10;
            int rowCount = 0;
            searchValue = searchValue.Trim();
            var data = CommonDataService.ListOfEmployees(page, pageSize, searchValue, out rowCount);
            Models.EmployeePaginationResultModel model = new Models.EmployeePaginationResultModel
            {
                Page = page,
                PageSize = pageSize,
                SearchValue = searchValue,
                RowCount = rowCount,
                Data = data
            };
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
            if(model == null)
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
        public ActionResult Save (Employee model, string birthDateString, HttpPostedFileBase uploadPhoto)
        {
            try
            {
                DateTime d = DateTime.ParseExact(birthDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model.BirthDate = d;
            }
            catch
            {
                ModelState.AddModelError("BirthDate", "Ngày sinh không hợp lệ " + birthDateString);
            }
            if (uploadPhoto != null)
            {
                string path = Server.MapPath("~/Images/Employees");
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(path, fileName);
                uploadPhoto.SaveAs(filePath);
                model.Photo = $"/Images/Employees/{fileName}";
            }
            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
                ModelState.AddModelError("FullName", "Họ tên không được để trống");
            if (string.IsNullOrEmpty(model.Notes))
                model.Notes = "";
            if (string.IsNullOrEmpty(model.Email))
                ModelState.AddModelError("Email", "Email không được để trống");
            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";
                return View("Create", model);
            }
            model.FirstName = model.FirstName.Trim();
            model.LastName = model.LastName.Trim();
            if (model.EmployeeID == 0)
            {
                if (uploadPhoto == null)
                    model.Photo = "";
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
                if (uploadPhoto == null && string.IsNullOrEmpty(model.Photo))
                    model.Photo = "";
                CommonDataService.UpdateEmployee(model);
                Session["employee"] = model;
            }
            return RedirectToAction("Index");
        }
    }
}