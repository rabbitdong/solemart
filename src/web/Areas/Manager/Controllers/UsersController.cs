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
    public class UsersController : Controller
    {
        private UserManager um = UserManager.Instance;
        //
        // GET: /Manager/User/

        public ActionResult Index()
        {
            int pi = 1; //表示页索引
            if (Request["p"] != null && int.TryParse(Request["p"], out pi)) ;

            ViewData["PageCount"] = (um.TotalUserCount + 9) / 10;
            ViewData["CurrentPageIndex"] = pi;

            return View(um.GetUserList(pi - 1, 10));
        }

        /// <summary>管理员修改用户角色的操作处理
        /// </summary>
        /// <param name="id">要修改的用户的ID</param>
        /// <returns>修改的结果View</returns>
        [Authorize(Roles="su")]
        public ActionResult ModifyRole(int id) {
            int newroleid = 0;

            if (Request["newrole"] != null && int.TryParse(Request["newrole"], out newroleid)) {
                if (um.ModifyUserRole(id, um.AllRoles.Single(r => r.RoleID == newroleid))) {
                    return Content("ok");
                }
            }

            return Content("error");
        }
    }
}
