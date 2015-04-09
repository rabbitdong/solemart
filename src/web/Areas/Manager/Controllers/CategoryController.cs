using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;
using Solemart.SystemUtil;
using Solemart.WebUtil;

namespace Solemart.Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "Super,Operator")]
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
        public ActionResult Create(CategoryItem category)
        {
            Result<string> result = CategoryManager.Instance.AddNewCategory(category);
            if (result.ResultCode == ResultCode.Success)
                return Content(WebResult<string>.SuccessResult.ResponseString);

            return Content(WebResult<string>.NormalErrorResult.ResponseString);
        }

        /// <summary>管理员请求修改列表操作的处理
        /// </summary>
        /// <returns>返回请求修改类别操作的View</returns>
        public ActionResult Modify()
        {
            List<CategoryItem> AllCates = CategoryManager.Instance.AllCategories;
            return View(AllCates);
        }

        public ActionResult ModifyCate()
        {
            int src_cate_id = 0;
            int desc_cate_id = 0;

            if (int.TryParse(Request["src_cate"], out src_cate_id)
                && int.TryParse(Request["desc_cate"], out desc_cate_id))
            {

                if (src_cate_id == desc_cate_id)
                {
                    return Content("error");
                }

                if (CategoryManager.Instance.ChangeCateToOtherSuperCate(src_cate_id, desc_cate_id))
                {
                    return Content("ok");
                }
                else
                {
                    return Content("error");
                }
            }

            return Content("error");
        }
    }
}
