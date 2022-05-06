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
            if(username == "admin@abc.com" && password == "1")
            {
                System.Web.Security.FormsAuthentication.SetAuthCookie(username, false);
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
        public ActionResult ChangePassword()
        {
            return View();
        }
    }
}