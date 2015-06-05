using Solemart.WeixinAPI.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeixinAPI.Entities.Menu
{
    /// <summary>
    /// 微信的自定义菜单结构
    /// </summary>
    public class CustomMenu
    {
        public CustomMenu()
        {
            this.button = new List<BaseButton>();
        }

        /// <summary>
        /// 菜单的按钮组
        /// </summary>
        public List<BaseButton> button { get; set; }
    }
}
