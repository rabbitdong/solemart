﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;
using Solemart.WebUtil;

namespace Solemart.Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "Super,Operator")]
    public class VendorController : Controller
    {
        //
        // GET: /Manager/Vendor/

        public ActionResult Index(int? p)
        {
            int totalPageCount = 0;
            int pi = p ?? 0; //表示页索引

            return View(VendorManager.GetPagedVendorInfos(pi, 10, out totalPageCount));
        }

        /// <summary>管理员请求新供应商的处理
        /// </summary>
        /// <returns>返回请求新供应商操作的View</returns>
        public ActionResult New()
        {
            return View();
        }

        /// <summary>管理员建立新供应商操作的处理
        /// </summary>
        /// <returns>建立新供应商记录的处理结果View</returns>
        public ActionResult NewVendor(VendorItem vendor)
        {
            if (VendorManager.AddNewVendor(vendor))
                return Content(WebResult<string>.SuccessResult.ResponseString);
            return Content(WebResult<string>.NormalErrorResult.ResponseString);
        }
    }
}
