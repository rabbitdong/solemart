using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;
using Solemart.SystemUtil;
using Solemart.Web.Models;

namespace Solemart.Web.Controllers
{
    public class ProductController : Controller
    {
        /// <summary>用户选择产品页时的返回
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? p)
        {
            CategoryItem cate = CategoryManager.Instance.GetFirstChildCate();
            ViewData["CurrentCateID"] = cate.CategoryID;

            int pi = p ?? 0; //表示页索引

            int CurrentCatePageCount = 0;
            IList<ProductItem> products = ProductManager.GetPagedProductsByCategory(cate.CategoryID, pi, 10, out CurrentCatePageCount);
            ViewData["PageCount"] = CurrentCatePageCount;
            ViewData["CurrentPageIndex"] = pi;

            return View(products);
        }

        /// <summary>选择某个类别时的显示
        /// </summary>
        /// <param name="id">类别的ID</param>
        /// <returns>该类别的View</returns>
        public ActionResult Cate(int id, int? p)
        {
            ViewData["CurrentCateID"] = id;

            CategoryItem cate = CategoryManager.Instance.GetCategoryById(id);
            int pi = p ?? 0; //表示页索引

            int CurrentCatePageCount = 0;
            IList<ProductItem> products = ProductManager.GetPagedProductsByCategory(cate.CategoryID, pi, 9, out CurrentCatePageCount);
            ViewData["PageCount"] = CurrentCatePageCount;
            ViewData["CurrentPageIndex"] = pi;

            return View("Index", products);
        }

        /// <summary>显示产品详细内容的页面
        /// </summary>
        /// <param name="id">要显示的产品的ID</param>
        /// <returns>产品详细页面</returns>
        public ActionResult Detail(int id)
        {
            SaledProductItem saleInfo = ProductManager.GetSaledProductByID(id);
            ProductItem product = ProductManager.GetProductWithBrandByID(id);
            List<ProductImageItem> images = ProductManager.GetProductNoLogoImage(id);
            int commentCount = ProductManager.GetProductCommentCount(id);

            Cart cart = (User as SolemartUser).Cart;
            CartItem item = cart.CartItems.FirstOrDefault(c => c.ProductID == id);
            if (item != null)
                ViewBag.CartItem = item.Amount;
            else
                ViewBag.CartItem = 0;

            string remainAmountString = string.Empty;
            if (product.Unit == "斤")
                remainAmountString = string.Format("{0}", product.StockCount - product.ReserveCount);
            else
                remainAmountString = string.Format("{0:d}", (int)(product.StockCount - product.ReserveCount));
            ProductDetailViewModel model = new ProductDetailViewModel
            {
                ProductID = id,
                ProductName = product.ProductName,
                ProductDescription = product.Description,
                Price = saleInfo.Price,
                Unit = product.Unit,
                Discount = saleInfo.Discount,
                SpecialFlag = saleInfo.SpecialFlag,
                RemainAmount = remainAmountString,
                BrandDescription = product.Brand.Description,
                BrandLogo = product.Brand.BrandLogo,
                BrandUrl = product.Brand.BrandUrl,
                BrandName = product.Brand.ZhName,
                CommentCount = commentCount,
                Images = images
            };

            return View(model);
        }

        /// <summary>
        /// The the comment list of product
        /// </summary>
        /// <param name="id">The product id</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetComment(int id, int pageIndex, int pageSize)
        {
            int totalPageCount = 0;
            IList<ProductCommentItem> comments = ProductManager.GetProductComment(id, pageIndex, pageSize, out totalPageCount);

            return Json(comments);
        }

        /// <summary>用户写产品评论的处理
        /// </summary>
        /// <param name="id">要编写评论的产品ID</param>
        /// <returns>编写评论的结果View</returns>
        public ActionResult PostComment(int id)
        {
            string content = Request["cnt"];
            int level = 5;
            SolemartUser user = Session["user"] as SolemartUser;
            //if (user == null || user == MyUser.Anonymous) {
            //    Response.Write("/login.aspx?ReturnUrl=" + Server.UrlEncode("/p/pd.aspx?pid=") + CurrentProduct.ProductID);
            //}

            if (Request["level"] == null || !int.TryParse(Request["level"], out level))
            {
                return Content("error");
            }

            SaledProductItem CurrentProduct = ProductManager.GetSaledProductByID(id);

            if (ProductManager.CommentProduct(user.UserID, CurrentProduct.ProductID, (EvaluteGrade)level, content))
            {
                var comm_json = new { Name = user.UserName, Time = DateTime.Now.ToLongDateString(), Level = level, Content = content, IsSuccess = true };
                return Json(comm_json);
            }
            else
            {
                return Json(new { IsSuccess = false, Message = "error" });
            }
        }
    }
}
