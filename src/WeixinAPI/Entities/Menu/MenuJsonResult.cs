using Solemart.WeixinAPI.Base.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeixinAPI.Entities.Menu
{
    public class MenuJsonResult : WxJsonResult
    {
        /// <summary>
        /// 返回结果就是一个menu对象（Json表示形式）
        /// </summary>
        public JsonMenu menu { get; set; }

        /// <summary>
        /// 语义的表示形式
        /// </summary>
        public CustomMenu Buttons { get; set; }
    }
}
