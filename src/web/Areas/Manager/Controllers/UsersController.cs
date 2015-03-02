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
    public class UsersController : Controller
    {
        //
        // GET: /Manager/User/

        public ActionResult Index()
        {
            int totalPageCount = 0;
            int pi = 1; //表示页索引
            if (Request["p"] != null && int.TryParse(Request["p"], out pi)) ;

            ViewData["PageCount"] = (UserManager.TotalUserCount + 9) / 10;
            ViewData["CurrentPageIndex"] = pi;

            return View(UserManager.GetUserList(pi, 10, out totalPageCount));
        }

        /// <summary>管理员修改用户角色的操作处理
        /// </summary>
        /// <param name="id">要修改的用户的ID</param>
        /// <returns>修改的结果View</returns>
        [Authorize(Roles = "su")]
        public ActionResult ModifyRole(int id)
        {
            int newroleid = 0;

            if (Request["newrole"] != null && int.TryParse(Request["newrole"], out newroleid))
            {
                if (UserManager.ModifyUserRole(id, ""))
                {
                    return Content("ok");
                }
            }

            return Content("error");
        }
    }
}
