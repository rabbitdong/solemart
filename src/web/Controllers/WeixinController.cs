using SimLogLib;
using Solemart.WeixinAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Solemart.Web.Controllers
{
    public class WeixinController : Controller
    {
        //
        // GET: /Weixin/

        public ActionResult Index(string signature, string timestamp, string nonce, string echostr)
        {
            string token = "solemart";
            if (!CheckSignature.Check(signature, timestamp, nonce, token))
            {
                Log.Instance.WriteLog(string.Format("Weixin entry failed"));
            }

            return Content(echostr);
        }
    }
}
