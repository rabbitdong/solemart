using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeixinAPI.Entities.Menu
{
    /// <summary>
    /// 每一个单项按钮
    /// </summary>
    public class JsonMenuItem
    {
        public JsonMenuItem()
        {
            sub_button = new List<JsonMenuItem>();
        }

        public string type { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string url { get; set; }

        public List<JsonMenuItem> sub_button { get; set; }
    }

    /// <summary>
    /// 按钮的根菜单项
    /// </summary>
    public class JsonMenu
    {
        public JsonMenu()
        {
            this.button = new List<JsonMenuItem>();
        }

        public List<JsonMenuItem> button { get; set; }
    }
}
