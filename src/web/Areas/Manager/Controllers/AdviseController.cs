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
    public class AdviseController : Controller
    {
        private AdviseManager am = AdviseManager.Instance;
        //
        // GET: /Manager/Advise/

        public ActionResult Index()
        {
            List<Adviser> Advisers = am.GetPagedAdvise(0);
            return View(Advisers);
        }

    }
}
