using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Xxx.Web.Areas.Acknowledge.Controllers
{
    public class EBooksController : Controller
    {
        //
        // GET: /Acknowledge/EBooks/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>查看PDF内容的详细页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles="users,su,operator")]
        public ActionResult Details(int id) {
            if (id == 1) {
                HttpContext.Response.AddHeader("content-disposition", "Inline; filename=苏菲的世界.pdf");
                FileStream fs = new FileStream(Server.MapPath("/resources/ebooks/苏菲的世界.pdf"), FileMode.Open);                
                return new FileStreamResult(fs, "application/pdf");
            }

            return Content("no valid id!");
        }

        /// <summary>查看PDF内容的详细页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "users,su,operator")]
        public ActionResult Download(int id) {
            if (id == 1) {
                HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + Server.UrlEncode("苏菲的世界.pdf"));
                FileStream fs = new FileStream(Server.MapPath("/resources/ebooks/苏菲的世界.pdf"), FileMode.Open);
                return new FileStreamResult(fs, "application/pdf");
            }

            return Content("no valid id!");
        }
    }
}
