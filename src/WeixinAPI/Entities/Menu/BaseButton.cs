/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：BaseButton.cs
    文件功能描述：所有菜单按钮基类
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solemart.WeixinAPI.Entities.Menu
{
    /// <summary>
    /// 所有按钮的基类（简单和包含子菜单的按钮）
    /// </summary>
    public class BaseButton
    {
        public BaseButton() { }

        public BaseButton(string name)
        {
            this.name = name;
            sub_button = new List<BaseButton>();
        }

        /// <summary>
        /// 按钮的名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Sub button of the button.
        /// </summary>
        public List<BaseButton> sub_button { get; set; }
    }

    /// <summary>
    /// 所有按钮基类
    /// </summary>
    public class SimpleButton : BaseButton
    {
        public SimpleButton() { }

        public SimpleButton(string name) : base(name) { }

        /// <summary>
        /// 按钮的类型
        /// </summary>
        public ButtonType type { get; set; }
    }

    /// <summary>
    /// click按钮类
    /// </summary>
    public class ClickButton : SimpleButton
    {
        public ClickButton() 
        {
            this.type = ButtonType.click;
        }

        public string key { get; set; }
        public ClickButton(string name)
            : base(name)
        {
            this.type = ButtonType.click;
        }

        public ClickButton(string name, string key)
            : this(name)
        {
            this.key = key;
        }
    }

    /// <summary>
    /// view按钮类
    /// </summary>
    public class ViewButton : SimpleButton
    {
        public string url { get; set; }

        public ViewButton()
        {
            this.type = ButtonType.view;
        }

        public ViewButton(string name) : base(name)
        {
            this.type = ButtonType.view;
        }

        public ViewButton(string name, string url)
            : this(name)
        {
            this.url = url;
        }
    }
}
