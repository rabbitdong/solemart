﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;


namespace Solemart.Web.Controllers
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
            SolemartUser user = User as SolemartUser;

            if (AdviseManager.NewAdvise(user.UserID, content)) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }
    }
}
