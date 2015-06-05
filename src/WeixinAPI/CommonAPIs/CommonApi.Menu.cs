/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：CommonApi.Menu.cs
    文件功能描述：通用自定义菜单接口
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
 
    修改标识：Senparc - 20150312
    修改描述：开放代理请求超时时间
 
    修改标识：Senparc - 201503232
    修改描述：修改字符串是否为空判断方式（感谢dusdong）
----------------------------------------------------------------*/

/*
    API：http://mp.weixin.qq.com/wiki/13/43de8269be54a0a6f64413e4dfa94f39.html
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Solemart.WeixinAPI.Entities;
using Solemart.WeixinAPI.Entities.Menu;
using Solemart.WeixinAPI.Base.Exceptions;
using Solemart.WeixinAPI.Base.HttpUtility;
using Solemart.WeixinAPI.Base;
using Newtonsoft.Json;
using SimLogLib;
using Solemart.WeixinAPI.Base.Entities;
using WeixinAPI.Entities.Menu;

namespace Solemart.WeixinAPI.CommonAPIs
{
    public partial class CommonApi
    {
        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="menuData">菜单内容</param>
        /// <returns></returns>
        public static WxJsonResult CreateMenu(string accessToken, CustomMenu menuData, int timeOut = Config.TIME_OUT)
        {
            var urlFormat = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}";
            return CommonJsonSend.Send(accessToken, urlFormat, menuData, timeOut: timeOut);
        }

        #region GetMenu
        /// <summary>
        /// 获取当前菜单，如果菜单不存在，将返回null
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static MenuJsonResult GetMenu(string accessToken)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}", accessToken);

            var jsonString = RequestUtility.HttpGet(url, Encoding.UTF8);
            //var finalResult = GetMenuFromJson(jsonString);

            MenuJsonResult finalResult;
            try
            {
                var jsonResult = JsonConvert.DeserializeObject<MenuJsonResult>(jsonString);
                if (jsonResult.menu == null || jsonResult.menu.button.Count == 0)
                {
                    throw new WeixinException(jsonResult.errmsg);
                }

                finalResult = GetMenuFromJsonResult(jsonResult);
            }
            catch (WeixinException ex)
            {
                Log.Instance.WriteError(ex.ToString());
                finalResult = null;
            }

            return finalResult;
        }

        /// <summary>
        /// 根据微信返回的Json数据得到可用的GetMenuResult结果
        /// </summary>
        /// <param name="resultFull"></param>
        /// <returns></returns>
        public static MenuJsonResult GetMenuFromJsonResult(MenuJsonResult resultFull)
        {
            MenuJsonResult result = null;
            try
            {
                //重新整理按钮信息
                CustomMenu bg = new CustomMenu();
                foreach (var rootButton in resultFull.menu.button)
                {
                    if (string.IsNullOrEmpty(rootButton.name))
                    {
                        continue;//没有设置一级菜单
                    }

                    var availableSubButton = rootButton.sub_button == null ? 0 : rootButton.sub_button.Count(z => !string.IsNullOrEmpty(z.name));//可用二级菜单按钮数量
                    if (availableSubButton == 0)
                    {
                        //底部单击按钮
                        if (rootButton.type == null ||
                            (rootButton.type.Equals("CLICK", StringComparison.OrdinalIgnoreCase)
                            && string.IsNullOrEmpty(rootButton.key)))
                        {
                            throw new WeixinMenuException("单击按钮的key不能为空！");
                        }

                        ButtonType buttontype = (ButtonType)Enum.Parse(typeof(ButtonType), rootButton.type);
                        if (buttontype == ButtonType.click)
                        {
                            //点击
                            bg.button.Add(new ClickButton()
                            {
                                name = rootButton.name,
                                key = rootButton.key,
                                type = buttontype
                            });
                        }
                        else if ( buttontype == ButtonType.view)
                        {
                            //URL
                            bg.button.Add(new ViewButton()
                            {
                                name = rootButton.name,
                                url = rootButton.url,
                                type = buttontype
                            });
                        }
                        //else if (rootButton.type.Equals("LOCATION_SELECT", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    //弹出地理位置选择器
                        //    bg.button.Add(new SingleLocationSelectButton()
                        //    {
                        //        name = rootButton.name,
                        //        key = rootButton.key,
                        //        type = rootButton.type
                        //    });
                        //}
                        //else if (rootButton.type.Equals("PIC_PHOTO_OR_ALBUM", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    //弹出拍照或者相册发图
                        //    bg.button.Add(new SinglePicPhotoOrAlbumButton()
                        //    {
                        //        name = rootButton.name,
                        //        key = rootButton.key,
                        //        type = rootButton.type
                        //    });
                        //}
                        //else if (rootButton.type.Equals("PIC_SYSPHOTO", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    //弹出系统拍照发图
                        //    bg.button.Add(new SinglePicSysphotoButton()
                        //    {
                        //        name = rootButton.name,
                        //        key = rootButton.key,
                        //        type = rootButton.type
                        //    });
                        //}
                        //else if (rootButton.type.Equals("PIC_WEIXIN", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    //弹出微信相册发图器
                        //    bg.button.Add(new SinglePicWeixinButton()
                        //    {
                        //        name = rootButton.name,
                        //        key = rootButton.key,
                        //        type = rootButton.type
                        //    });
                        //}
                        //else if (rootButton.type.Equals("SCANCODE_PUSH", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    //扫码推事件
                        //    bg.button.Add(new SingleScancodePushButton()
                        //    {
                        //        name = rootButton.name,
                        //        key = rootButton.key,
                        //        type = rootButton.type
                        //    });
                        //}
                        //else
                        //{
                        //    //扫码推事件且弹出“消息接收中”提示框
                        //    bg.button.Add(new SingleScancodeWaitmsgButton()
                        //    {
                        //        name = rootButton.name,
                        //        key = rootButton.key,
                        //        type = rootButton.type
                        //    });
                        //}
                    }
                    //else if (availableSubButton < 2)
                    //{
                    //    throw new WeixinMenuException("子菜单至少需要填写2个！");
                    //}
                    else
                    {
                        //底部二级菜单
                        var subButton = new BaseButton(rootButton.name);
                        bg.button.Add(subButton);

                        foreach (var subSubButton in rootButton.sub_button)
                        {
                            if (string.IsNullOrEmpty(subSubButton.name))
                            {
                                continue; //没有设置菜单
                            }

                            if (subSubButton.type.Equals("CLICK", StringComparison.OrdinalIgnoreCase)
                                && string.IsNullOrEmpty(subSubButton.key))
                            {
                                throw new WeixinMenuException("单击按钮的key不能为空！");
                            }

                            ButtonType buttontype = (ButtonType)Enum.Parse(typeof(ButtonType), subSubButton.type);
                            if (buttontype == ButtonType.click)
                            {
                                //点击
                                subButton.sub_button.Add(new ClickButton()
                                {
                                    name = subSubButton.name,
                                    key = subSubButton.key,
                                    type = buttontype
                                });
                            }
                            else if (buttontype == ButtonType.view)
                            {
                                //URL
                                subButton.sub_button.Add(new ViewButton()
                                {
                                    name = subSubButton.name,
                                    url = subSubButton.url,
                                    type = buttontype
                                });
                            }
                            //else if (subSubButton.type.Equals("LOCATION_SELECT", StringComparison.OrdinalIgnoreCase))
                            //{
                            //    //弹出地理位置选择器
                            //    subButton.sub_button.Add(new SingleLocationSelectButton()
                            //    {
                            //        name = subSubButton.name,
                            //        key = subSubButton.key,
                            //        type = subSubButton.type
                            //    });
                            //}
                            //else if (subSubButton.type.Equals("PIC_PHOTO_OR_ALBUM", StringComparison.OrdinalIgnoreCase))
                            //{
                            //    //弹出拍照或者相册发图
                            //    subButton.sub_button.Add(new SinglePicPhotoOrAlbumButton()
                            //    {
                            //        name = subSubButton.name,
                            //        key = subSubButton.key,
                            //        type = subSubButton.type
                            //    });
                            //}
                            //else if (subSubButton.type.Equals("PIC_SYSPHOTO", StringComparison.OrdinalIgnoreCase))
                            //{
                            //    //弹出系统拍照发图
                            //    subButton.sub_button.Add(new SinglePicSysphotoButton()
                            //    {
                            //        name = subSubButton.name,
                            //        key = subSubButton.key,
                            //        type = subSubButton.type
                            //    });
                            //}
                            //else if (subSubButton.type.Equals("PIC_WEIXIN", StringComparison.OrdinalIgnoreCase))
                            //{
                            //    //弹出微信相册发图器
                            //    subButton.sub_button.Add(new SinglePicWeixinButton()
                            //    {
                            //        name = subSubButton.name,
                            //        key = subSubButton.key,
                            //        type = subSubButton.type
                            //    });
                            //}
                            //else if (subSubButton.type.Equals("SCANCODE_PUSH", StringComparison.OrdinalIgnoreCase))
                            //{
                            //    //扫码推事件
                            //    subButton.sub_button.Add(new SingleScancodePushButton()
                            //    {
                            //        name = subSubButton.name,
                            //        key = subSubButton.key,
                            //        type = subSubButton.type
                            //    });
                            //}
                            //else
                            //{
                            //    //扫码推事件且弹出“消息接收中”提示框
                            //    subButton.sub_button.Add(new SingleScancodeWaitmsgButton()
                            //    {
                            //        name = subSubButton.name,
                            //        key = subSubButton.key,
                            //        type = subSubButton.type
                            //    });
                            //}
                        }
                    }
                }

                result = new MenuJsonResult()
                {
                    Buttons = bg
                };
            }
            catch (Exception ex)
            {
                throw new WeixinMenuException(ex.Message, ex);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static WxJsonResult DeleteMenu(string accessToken)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}", accessToken);
            var result = GetMethod.GetJson<WxJsonResult>(url);
            return result;
        }
    }
}