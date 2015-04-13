using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;

namespace Solemart.Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "Super,Operator")]
    public class BillBoardController : Controller
    {
        //
        // GET: /Manager/BillBoard/

        public ActionResult Index()
        {
            List<BulletinItem> bulletins = BulletinManager.GetValidBillBoardList();
            return View(bulletins);
        }

        /// <summary>管理员请求创建一个新公告的处理
        /// </summary>
        /// <returns>创建新公告的结果</returns>
        public ActionResult Create(BulletinItem bulletin)
        {
            string content = Request["content"];
            DateTime publish_time = DateTime.Now;
            DateTime end_time = DateTime.Now;

            if (!DateTime.TryParse(Request["publishtime"], out publish_time))
            {
                return Content("error");
            }

            if (!DateTime.TryParse(Request["endtime"], out end_time))
            {
                return Content("error");
            }

            if(BulletinManager.CreateNewBulletin(bulletin))
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
