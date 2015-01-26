using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xxx.EntityLib;
using Xxx.BusinessLib;
using SolemartUser = Xxx.EntityLib.User;

namespace Xxx.Web.Controllers
{
    public class HelpController : Controller
    {
        //
        // GET: /Help/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About() {
            return View();
        }

        /// <summary>用户提出新建议的处理
        /// </summary>
        /// <returns>返回处理结果</returns>
        public ActionResult NewAdvise() {
            string content = Request["content"];
            SolemartUser user = Session["user"] as SolemartUser;

            if (AdviseManager.Instance.NewAdvise(user, content)) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }
    }
}
