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
    [RoutePrefix("order")]
    public class OrderController : Controller
    {
        // GET: Oder
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Title = "Tạo đơn hàng";
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [Route("edit/{orderID}")]
        public ActionResult Edit(int orderID)
        {
            ViewBag.Title = "Cập nhật thông tin đơn hàng";
            return View("Create");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [Route("delete/{orderID}")]
        public ActionResult Delete(int orderID)
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [Route("details/{orderID}")]
        public ActionResult Details(int orderID)
        {
            return View();
        }
    }
}