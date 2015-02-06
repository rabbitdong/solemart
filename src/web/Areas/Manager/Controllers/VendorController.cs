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
    public class VendorController : Controller
    {
        private VendorManager vm = VendorManager.Instance;
        //
        // GET: /Manager/Vendor/

        public ActionResult Index()
        {
            ViewData["PageCount"] = vm.VendorPageCount;
            int pi = 1; //表示页索引
            if (Request["p"] != null && int.TryParse(Request["p"], out pi)) ;

            return View(vm.GetPagedVendorInfos(pi - 1));
        }

        /// <summary>管理员请求新供应商的处理
        /// </summary>
        /// <returns>返回请求新供应商操作的View</returns>
        public ActionResult New() {
            return View();
        }

        /// <summary>管理员建立新供应商操作的处理
        /// </summary>
        /// <returns>建立新供应商记录的处理结果View</returns>
        public ActionResult NewVendor() {
            string vendor_name = Request["fact_name"];
            string addr = Request["fact_addr"];
            string phone1 = Request["phone1"];
            string contact_name = Request["charge_name"];
            string url = Request["url"];

            if (vm.AddNewVendor(vendor_name, addr, contact_name, phone1, url)) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }
    }
}
