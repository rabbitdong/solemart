using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;

namespace Solemart.Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "su,operator")]
    public class VendorController : Controller
    {
        //
        // GET: /Manager/Vendor/

        public ActionResult Index()
        {
            int totalPageCount = 0;
            int pi = 1; //表示页索引
            if (Request["p"] != null && int.TryParse(Request["p"], out pi)) ;

            return View(VendorManager.GetPagedVendorInfos(pi, 10, out totalPageCount));
        }

        /// <summary>管理员请求新供应商的处理
        /// </summary>
        /// <returns>返回请求新供应商操作的View</returns>
        public ActionResult New()
        {
            return View();
        }

        /// <summary>管理员建立新供应商操作的处理
        /// </summary>
        /// <returns>建立新供应商记录的处理结果View</returns>
        public ActionResult NewVendor(VendorItem vendor)
        {
            if (VendorManager.AddNewVendor(vendor))
            {
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }
    }
}
