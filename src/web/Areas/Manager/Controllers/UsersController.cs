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
    public class UsersController : Controller
    {
        //
        // GET: /Manager/User/

        public ActionResult Index(int? p)
        {
            int totalPageCount = 0;
            int pi = p ?? 0; //表示页索引

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
