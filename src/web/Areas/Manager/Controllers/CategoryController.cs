using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Solemart.EntityLib;
using Solemart.BusinessLib;

namespace Solemart.Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "su,operator")]
    public class CategoryController : Controller
    {
        //
        // GET: /Manager/Category/

        public ActionResult New()
        {
            return View(CategoryManager.Instance.Categories);
        }

        /// <summary>用户建立新类别请求的处理
        /// </summary>
        /// <returns>返回建立新列表的结果View</returns>
        public ActionResult Create() {
            string cate_name = Request["cate_name"];
            string cate_addr = Request["cate_desc"];
            int sup_cate_id = -1;
            int.TryParse(Request["super_cate_id"], out sup_cate_id);

            bool result = CategoryManager.Instance.AddNewCategory(cate_name, cate_addr, sup_cate_id);
            if (result) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }

        /// <summary>管理员请求修改列表操作的处理
        /// </summary>
        /// <returns>返回请求修改类别操作的View</returns>
        public ActionResult Modify() {
            List<Category> AllCates = CategoryManager.Instance.AllCategories;
            return View(AllCates);
        }

        public ActionResult ModifyCate() {
            int src_cate_id = 0;
            int desc_cate_id = 0;

            if (int.TryParse(Request["src_cate"], out src_cate_id)
                && int.TryParse(Request["desc_cate"], out desc_cate_id)) {

                if (src_cate_id == desc_cate_id) {
                    return Content("error");
                }

                if (CategoryManager.Instance.ChangeCateToOtherSuperCate(src_cate_id, desc_cate_id)) {
                    return Content("ok");
                }
                else {
                    return Content("error");
                }
            }

            return Content("error");
        }
    }
}
