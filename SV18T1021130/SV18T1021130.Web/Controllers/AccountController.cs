using SV18T1021130.BusinessLayer;
using SV18T1021130.DomainModel;
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
    public class AccountController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string username, string password)
        {
            username = username.ToLower();
            Employee employee = CommonAccountService.Login(username, password);
            if (employee != null)
            {
                System.Web.Security.FormsAuthentication.SetAuthCookie(username, false);
                Session["account"] = employee;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserName = username;

            ViewBag.Message = "Đăng nhập thất bại";
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword(string oldPassword, string newPassword, string comfirmPassword)
        {
            if (Request.HttpMethod == "POST")
            {
                Employee employee = Session["account"] as Employee;
                //if (employee == null)
                //    return RedirectToAction("Logout");
                if (string.IsNullOrEmpty(oldPassword))
                    ModelState.AddModelError("oldPassword", "Nhập mật khẩu cũ");
                if (string.IsNullOrEmpty(newPassword))
                    ModelState.AddModelError("newPassword", "Nhập mật khẩu mới");
                if (string.IsNullOrEmpty(comfirmPassword))
                    ModelState.AddModelError("comfirmPassword", "Nhập lại mật khẩu mới");
                if(!string.IsNullOrEmpty(newPassword) && ! string.IsNullOrEmpty(comfirmPassword))
                    if (newPassword != comfirmPassword)
                        ModelState.AddModelError("comfirmPassword", "Mật khẩu mới và mật khẩu nhập lại không trùng khớp");
                if (!string.IsNullOrEmpty(oldPassword))
                {
                    bool checkPassword = CommonAccountService.CheckPassword(employee.Email, oldPassword);
                    if (!checkPassword)
                        ModelState.AddModelError("oldPassword", "Mật khẩu cũ không đúng");
                }

                if (!ModelState.IsValid)
                    return View();
                CommonAccountService.ChangePassword(employee.Email, newPassword);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}