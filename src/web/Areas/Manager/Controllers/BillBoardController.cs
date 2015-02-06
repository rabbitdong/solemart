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
    public class BillBoardController : Controller
    {
        private BillboardManager bbm = BillboardManager.Instance;
        //
        // GET: /Manager/BillBoard/

        public ActionResult Index()
        {
            List<BillBoard> BillBoards = bbm.GetValidBillBoardList();
            return View(BillBoards);
        }

        /// <summary>管理员请求创建一个新公告的处理
        /// </summary>
        /// <returns>创建新公告的结果</returns>
        public ActionResult Create() {
            string content = Request["content"];
            DateTime publish_time = DateTime.Now;
            DateTime end_time = DateTime.Now;

            if (!DateTime.TryParse(Request["publishtime"], out publish_time)) {
                return Content("error");
            }

            if (!DateTime.TryParse(Request["endtime"], out end_time)) {
                return Content("error");
            }

            BillBoard bb = BillboardManager.Instance.CreateNewBillBoard(content, end_time);
            if (bb != null) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }
    }
}
