using SimLogLib;
using Solemart.WeixinAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Solemart.SystemUtil;
using Solemart.WeixinAPI.CommonAPIs;
using Solemart.WeixinAPI.Entities;
using Solemart.WeixinAPI.Entities.Menu;
using Newtonsoft.Json;
using Solemart.WeixinAPI.Base.Entities;
using Solemart.WeixinAPI.Base;
using WeixinAPI.Entities.Menu;

namespace Solemart.Web.Controllers
{
    public class WeixinTestController : Controller
    {
        //
        // GET: /WeixinTest/

        /// <summary>
        /// The interface for validating from the weixin
        /// </summary>
        /// <returns></returns>
        /// signature  微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。  
        /// timestamp  时间戳  
        /// nonce  随机数  
        /// echostr  随机字符串  
        public ActionResult Index(string signature, string timestamp, string nonce, string echostr)
        {
            if (!CheckSignature.Check(signature, timestamp, nonce, ConfigSettings.WeixinToken))
            {
                Log.Instance.WriteLog(string.Format("Weixin entry failed"));
                
            }

            return Content(echostr);
        }

        public ActionResult Operate()
        {            
            return View();
        }

        public ActionResult CreateMenu(string data)
        {
            string accessToken = AccessTokenContainer.TryGetToken(ConfigSettings.TestWeixinAppID, ConfigSettings.TestWeixinAppSecret);
            JsonMenu jsonMenu = JsonConvert.DeserializeObject<JsonMenu>(data);
            MenuJsonResult jsonResult = new MenuJsonResult { menu = jsonMenu };

            jsonResult = CommonApi.GetMenuFromJsonResult(jsonResult);
            WxJsonResult result = CommonApi.CreateMenu(accessToken, jsonResult.Buttons);
            if (result.errcode == WeixinReturnCode.Success)
                return Content("ok");
            else
                return Content("error");
        }

        public ActionResult GetMenu()
        {
            string accessToken = AccessTokenContainer.TryGetToken(ConfigSettings.TestWeixinAppID, ConfigSettings.TestWeixinAppSecret);
            MenuJsonResult menuResult = CommonApi.GetMenu(accessToken);
            return Json(menuResult);
        }
    }
}
