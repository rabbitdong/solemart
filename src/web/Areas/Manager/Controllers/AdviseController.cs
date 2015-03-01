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
    public class AdviseController : Controller
    {
        //
        // GET: /Manager/Advise/

        public ActionResult Index()
        {
            //List<AdviserItem> Advisers = AdviseManager.GetPagedAdvise();
            return View();
        }

    }
}
