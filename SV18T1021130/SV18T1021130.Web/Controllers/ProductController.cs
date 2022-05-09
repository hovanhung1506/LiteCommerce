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
    [RoutePrefix("product")]
    public class ProductController : Controller
    {
        // GET: Product
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ProductSearchInput model = Session["PRODUCT_SEARCH"] as ProductSearchInput;
            int rowCount = 0;
            var categories = CommonDataService.ListOfCategories(1, 0, "", out rowCount);
            var suppliers = CommonDataService.ListOfSuppliers(1, 0, "", out rowCount);
            if (model == null)
            {
                model = new ProductSearchInput()
                {
                    Categories = categories,
                    Suppliers = suppliers,
                    CategoryID = 0,
                    SupplierID = 0,
                    Page = 1,
                    PageSize = 10,
                    SearchValue = "",
                };
            }
            else
            {
                model.Categories = categories;
                model.Suppliers = suppliers;
            }
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ActionResult Search(Models.ProductSearchInput input)
        {
            int rowCount = 0;
            var products = CommonProductService.ListOfPrudcts(input.Page, input.PageSize, input.CategoryID, input.SupplierID, input.SearchValue, out rowCount);
            ProductPaginationResultModel model = new ProductPaginationResultModel()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                Products = products,
                RowCount = rowCount,
                SearchValue = input.SearchValue
            };
            Session["PRODUCT_SEARCH"] = input;
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            Product model = new Product()
            {
                ProductID = 0
            };
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        [Route("edit/{productID}")]
        public ActionResult Edit(string productID)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(productID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            var product = CommonProductService.GetProduct(id);
            var productPhotos = CommonProductService.ListOfProductPhotos(id);
            var productAttributes = CommonProductService.ListOfProductAttributes(id);
            Models.ProductResultModel model = new Models.ProductResultModel
            {
                product = product,
                productAttributes = productAttributes,
                productPhotos = productPhotos
            };
            return View("Edit", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        [Route("delete/{productID}")]
        public ActionResult Delete(string productID)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(productID);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            var model = CommonProductService.GetProduct(id);
            if (Request.HttpMethod == "POST")
            {
                if (model.Photo != "")
                {
                    string path = Server.MapPath("~/Images/Products");
                    string fileName = model.Photo.Split('/')[3];
                    string filePath = System.IO.Path.Combine(path, fileName);
                    CommonDataService.DeleteImage(filePath);
                }
                var listProductPhoto = CommonProductService.ListOfProductPhotos(model.ProductID);
                var listProductAttribute = CommonProductService.ListOfProductAttributes(model.ProductID);
                foreach (var item in listProductAttribute)
                    CommonProductService.DeleteProductAttribute(model.ProductID, item.AttributeID);
                foreach (var item in listProductPhoto)
                {
                    var p = CommonProductService.GetProductPhoto(item.ProductID, item.PhotoID);
                    if (p.Photo != "")
                    {
                        string path = Server.MapPath("~/Images/Products");
                        string fileName = p.Photo.Split('/')[3];
                        string filePath = System.IO.Path.Combine(path, fileName);
                        CommonDataService.DeleteImage(filePath);
                    }
                    CommonProductService.DeleteProductPhoto(item.ProductID, item.PhotoID);
                }
                CommonProductService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="productID"></param>
        /// <param name="photoID"></param>
        /// <returns></returns>
        [Route("photo/{method}/{productID}/{photoID?}")]
        public ActionResult Photo(string method, int productID, int? photoID)
        {
            var model = new SV18T1021130.DomainModel.ProductPhoto
            {
                PhotoID = 0
            };
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh";
                    model.ProductID = productID;
                    break;
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh";
                    model = CommonProductService.GetProductPhoto(productID, photoID);
                    break;
                case "delete":
                    model = CommonProductService.GetProductPhoto(productID, photoID);
                    if (!string.IsNullOrEmpty(model.Photo))
                    {
                        string path = Server.MapPath("~/Images/Products");
                        string fileName = model.Photo.Split('/')[3];
                        string filePath = System.IO.Path.Combine(path, fileName);
                        CommonDataService.DeleteImage(filePath);
                    }
                    CommonProductService.DeleteProductPhoto(productID, photoID);
                    return RedirectToAction("Edit", new { productID = productID });
                default:
                    return RedirectToAction("Index");
            }
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="productID"></param>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        [Route("attribute/{method}/{productID}/{attributeID?}")]
        public ActionResult Attribute(string method, int productID, int? attributeID)
        {
            var model = new SV18T1021130.DomainModel.ProductAttribute
            {
                AttributeID = 0
            };
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    model.ProductID = productID;
                    break;
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính";
                    model = CommonProductService.GetProductAttribute(productID, attributeID);
                    break;
                case "delete":
                    CommonProductService.DeleteProductAttribute(productID, attributeID);
                    return RedirectToAction("Edit", new { productID = productID });
                default:
                    return RedirectToAction("Index");
            }
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="uploadPhoto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(Product model, HttpPostedFileBase uploadPhoto)
        {
            if (string.IsNullOrEmpty(model.ProductName))
                ModelState.AddModelError("ProductName", "Tên mặt hàng không được để trống");
            if (string.IsNullOrEmpty(model.Unit))
                ModelState.AddModelError("Unit", "Đơn vị tính không được để trống");
            if (string.IsNullOrEmpty(model.Price))
                ModelState.AddModelError("Price", "Giá không được để trống");
            else
                try
                {
                    model.Price = model.Price.Replace(',', '.');
                    double p = double.Parse(model.Price);
                    //model.Price = Math.Round(p, 2, MidpointRounding.AwayFromZero).ToString();
                    if (p <= 0)
                        ModelState.AddModelError("Price", "Giá phải lớn hơn 0");
                }
                catch
                {
                    ModelState.AddModelError("Price", "Giá không hợp lệ");
                }
            if (uploadPhoto == null && model.ProductID == 0)
                ModelState.AddModelError("Photo", "Ảnh không được để trống");
            if (!ModelState.IsValid && model.ProductID == 0)
                return View("Create", model);
            if(!ModelState.IsValid && model.ProductID > 0)
            {
                var productPhotos = CommonProductService.ListOfProductPhotos(model.ProductID);
                var productAttributes = CommonProductService.ListOfProductAttributes(model.ProductID);
                Models.ProductResultModel prs = new Models.ProductResultModel
                {
                    product = model,
                    productAttributes = productAttributes,
                    productPhotos = productPhotos
                };
                return View("Edit", prs);
            }
            if (uploadPhoto != null)
            {
                string path = Server.MapPath("~/Images/Products");
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(path, fileName);
                uploadPhoto.SaveAs(filePath);
                model.Photo = $"/Images/Products/{fileName}";
            }
            if (model.ProductID == 0)
            {
                CommonProductService.AddProduct(model);
            }
            else
            {
                var p = CommonProductService.GetProduct(model.ProductID);
                if (uploadPhoto != null)
                {
                    if (!string.IsNullOrEmpty(p.Photo))
                    {
                        string path = Server.MapPath("~/Images/Products");
                        string fileName = p.Photo.Split('/')[3];
                        string filePath = System.IO.Path.Combine(path, fileName);
                        CommonDataService.DeleteImage(filePath);
                    }
                }
                CommonProductService.UpdateProduct(model);
                model = CommonProductService.GetProduct(model.ProductID);
                Session["product"] = model;
            }
            ProductSearchInput input = new ProductSearchInput()
            {
                Page = 1,
                PageSize = 10,
                SearchValue = model.ProductName
            };
            Session["PRODUCT_SEARCH"] = input;
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="uploadPhoto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveProductPhoto(ProductPhoto model, HttpPostedFileBase uploadPhoto)
        {
            if (uploadPhoto != null)
            {
                string path = Server.MapPath("~/Images/Products");
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(path, fileName);
                uploadPhoto.SaveAs(filePath);
                model.Photo = $"/Images/Products/{fileName}";
            }

            if (string.IsNullOrEmpty(model.Description))
                ModelState.AddModelError("Description", "Mô tả không được để trống");
            if (model.DisplayOrder <= 0)
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị phải lớn hơn 0");
            if (uploadPhoto == null && model.PhotoID == 0)
                ModelState.AddModelError("Photo", "Ảnh không được để trống");
            if (!ModelState.IsValid)
                return View("Photo", model);

            model.IsHidden = model.IsHidden == "on" ? "1" : "0";

            if (model.PhotoID == 0)
            {
                if (uploadPhoto == null)
                    model.Photo = "";
                CommonProductService.AddProductPhoto(model);
            }
            else
            {
                if (uploadPhoto != null)
                {
                    var p = CommonProductService.GetProductPhoto(model.ProductID, model.PhotoID);
                    string path = Server.MapPath("~/Images/Products");
                    string fileName = p.Photo.Split('/')[3];
                    string filePath = System.IO.Path.Combine(path, fileName);
                    CommonDataService.DeleteImage(filePath);
                }
                CommonProductService.UpdateProductPhoto(model);
            }
            return RedirectToAction("edit/" + model.ProductID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveProductAttribute(ProductAttribute model)
        {
            if (string.IsNullOrEmpty(model.AttributeName))
                ModelState.AddModelError("AttributeName", "Tên thuộc tính không được để trống");
            if (string.IsNullOrEmpty(model.AttributeValue))
                ModelState.AddModelError("AttributeValue", "Giá trị thuộc tính không để trống");
            if (model.DisplayOrder <= 0)
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị phải lớn hơn 0");
            if (!ModelState.IsValid)
                return View("Attribute", model);
            model.AttributeName = model.AttributeName.Trim();
            model.AttributeValue = model.AttributeValue.Trim();
            if (model.AttributeID == 0)
                CommonProductService.AddProductAttribute(model);
            else
                CommonProductService.UpdateProductAttribute(model);
            return RedirectToAction("edit/" + model.ProductID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{type}/{id}")]
        public ActionResult Redirect(string type, int id)
        {
            ProductSearchInput input = Session["PRODUCT_SEARCH"] as ProductSearchInput;
            switch (type)
            {
                case "supplier":
                    if (input == null)
                    {
                        input = new ProductSearchInput()
                        {
                            SupplierID = id,
                            Page = 1,
                            PageSize = 10,
                            SearchValue = "",
                            CategoryID = 0
                        };
                    }
                    else
                    {
                        input.CategoryID = 0;
                        input.SupplierID = id;
                    }
                    Session["PRODUCT_SEARCH"] = input;
                    break;
                case "category":
                    if (input == null)
                    {
                        input = new ProductSearchInput()
                        {
                            SupplierID = 0,
                            Page = 1,
                            PageSize = 10,
                            SearchValue = "",
                            CategoryID = id
                        };
                    }
                    else
                    {
                        input.SupplierID = 0;
                        input.CategoryID = id;
                    }
                    Session["PRODUCT_SEARCH"] = input;
                    break;
                default:
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}