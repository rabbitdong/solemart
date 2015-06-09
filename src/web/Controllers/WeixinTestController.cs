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
using Solemart.WeixinAPI.Entities.Request;
using System.Xml.Linq;
using Solemart.WeixinAPI.Base.XmlUtility;
using System.IO;
using System.Text;
using System.Net;

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
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, ConfigSettings.WeixinToken))
            {
                Log.Instance.WriteLog(string.Format("Weixin get entry failed"));
                return Content("Parameter error!");
            }

            return Content(echostr);
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, ConfigSettings.WeixinToken))
            {
                Log.Instance.WriteLog(string.Format("Weixin post entry failed"));
                return Content("Parameter error!");
            }

            postModel.Token = ConfigSettings.WeixinToken;
            postModel.EncodingAESKey = "";//根据自己后台的设置保持一致
            postModel.AppId = ConfigSettings.TestWeixinAppID;//根据自己后台的设置保持一致

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            //var maxRecordCount = 10;  
            Request.InputStream.Position = 0;
            long dataLength = Request.InputStream.Length;
            var bytes = new byte[dataLength];
            Request.InputStream.Read(bytes, 0, (int)dataLength);
            string xmlContent = Request.ContentEncoding.GetString(bytes, 0, (int)dataLength);

            Log.Instance.WriteLog(string.Format("Weixin post data[{0}]", WebUtility.HtmlEncode(xmlContent)));
            return Content("");

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            //var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, maxRecordCount);
            //try
            //{

            //    /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
            //     * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
            //    messageHandler.OmitRepeatedMessage = true;

            //    //执行微信处理过程
            //    messageHandler.Execute();

            //    //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
            //    return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
            //    //return new WeixinResult(messageHandler);//v0.8+
            //}
            //catch (Exception ex)
            //{
            //    return Content("");
            //}
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
