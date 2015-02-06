using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Solemart.EntityLib;
using Solemart.BusinessLib;
using MyUser = Solemart.EntityLib.User;

namespace Solemart.Web.Controllers
{
    public class ProductController : Controller
    {
        private ProductManager pm = ProductManager.Instance;
        /// <summary>用户选择产品页时的返回
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Category cate = CategoryManager.Instance.GetFirstChildCate();
            ViewData["CurrentCateID"] = cate.CateID;

            int pi = 1; //表示页索引
            if (Request["p"] != null && int.TryParse(Request["p"], out pi)) ;

            int CurrentCatePageCount = 0;
            IList<Product> products = pm.GetPagedProductsByCategory(cate, pi - 1, 9, out CurrentCatePageCount);
            ViewData["PageCount"] = CurrentCatePageCount;
            ViewData["CurrentPageIndex"] = pi;
            
            return View(products);
        }

        /// <summary>选择某个类别时的显示
        /// </summary>
        /// <param name="id">类别的ID</param>
        /// <returns>该类别的View</returns>
        public ActionResult Cate(int id) {
            ViewData["CurrentCateID"] = id;

            Category cate = CategoryManager.Instance.GetCateById(id);
            int pi = 1; //表示页索引
            if (Request["p"] != null && int.TryParse(Request["p"], out pi)) ;

            int CurrentCatePageCount = 0;
            IList<Product> products = pm.GetPagedProductsByCategory(cate, pi - 1, 9, out CurrentCatePageCount);
            ViewData["PageCount"] = CurrentCatePageCount;
            ViewData["CurrentPageIndex"] = pi;

            return View("Index", products);
        }

        /// <summary>显示产品详细内容的页面
        /// </summary>
        /// <param name="id">要显示的产品的ID</param>
        /// <returns>产品详细页面</returns>
        public ActionResult Detail(int id) {
            Product CurrentProduct = pm.GetProductByID(id);

            return View(CurrentProduct);
        }

        /// <summary>用户写产品评论的处理
        /// </summary>
        /// <param name="id">要编写评论的产品ID</param>
        /// <returns>编写评论的结果View</returns>
        public ActionResult PostComment(int id) {
            string content = Request["cnt"];
            int level = 5;
            MyUser user = Session["user"] as MyUser;
            //if (user == null || user == MyUser.Anonymous) {
            //    Response.Write("/login.aspx?ReturnUrl=" + Server.UrlEncode("/p/pd.aspx?pid=") + CurrentProduct.ProductID);
            //}

            if (Request["level"] == null || !int.TryParse(Request["level"], out level)) {
                return Content("error");
            }

            Product CurrentProduct = ProductManager.Instance.GetProductByID(id);

            if (pm.CommentProduct(user, CurrentProduct, (EvaluteGrade)level, content)) {
                var comm_json = new { Name = user.Name, Time = DateTime.Now.ToLongDateString(), Level = level, Content = content, IsSuccess=true };
                return Json(comm_json);
            }
            else {
                return Json(new { IsSuccess = false, Message = "error" });
            }
        }
    }
}
