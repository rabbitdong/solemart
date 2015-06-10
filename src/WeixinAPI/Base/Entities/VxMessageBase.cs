using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WeixinAPI.Base.Entities
{
    /// <summary>
    /// 接收消息类型
    /// </summary>
    public enum VxMessageType
    {
        Text, //文本
        Location, //地理位置
        Image, //图片
        Voice, //语音
        Video, //视频
        Link, //连接信息
        ShortVideo,//小视频
        Event, //事件推送
    }

    /// <summary>
    /// Weixin事件的类型
    /// </summary>
    public enum VxEventType
    {
        /// <summary>
        /// 订阅
        /// </summary>
        subscribe,

        /// <summary>
        /// 取消订阅
        /// </summary>
        unsubscribe,

        /// <summary>
        /// 自定义菜单点击事件
        /// </summary>
        CLICK,

        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        LOCATION,

        /// <summary>
        /// 二维码扫描
        /// </summary>
        SCAN,

        /// <summary>
        /// URL跳转
        /// </summary>
        VIEW
    }

    /// <summary>
    /// 所有Request和Response消息的基类
    /// </summary>
    public class VxMessageBase
    {
        private static DateTime BeginDateTime = new DateTime(1970, 1, 1);

        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public DateTime CreateTime { get; set; }
        public VxMessageType MsgType { get; set; }

        public virtual XDocument ToXml()
        {
            XDocument doc = new XDocument(new XElement("xml",
                new XElement("ToUserName", ToUserName),
                new XElement("FromUserName", FromUserName),
                new XElement("CreateTime", (CreateTime - BeginDateTime).Seconds),
                new XElement("MsgType", MsgType.ToString("g").ToLower())));

            return doc;
        }

        /// <summary>
        /// 从XDocument解析消息实体
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static VxMessageBase FromXml(XDocument xml)
        {
            VxMessageBase msg = null;
            VxMessageType msgType = (VxMessageType)Enum.Parse(typeof(VxMessageType), xml.Root.Element("MsgType").Value, true);

            //进行消息的解析
            switch (msgType)
            {
                case VxMessageType.Text:
                    msg = new VxMessageText();
                    break;
                case VxMessageType.Image:
                    msg = new VxMessageImage();
                    break;
                case VxMessageType.Location:
                    msg = new VxMessageLocation();
                    break;
                case VxMessageType.Link:
                    msg = new VxMessageLink();
                    break;
                case VxMessageType.ShortVideo:
                    msg = new VxMessageShortVideo();
                    break;
                case VxMessageType.Video:
                    msg = new VxMessageVideo();
                    break;
                case VxMessageType.Voice:
                    msg = new VxMessageVoice();
                    break;
                case VxMessageType.Event:
                    
                    break;
            }

            msg.ToUserName = xml.Root.Element("ToUserName").Value;
            msg.FromUserName = xml.Root.Element("FromUserName").Value;
            int sec = int.Parse(xml.Root.Element("CreateTime").Value);
            DateTime baseTime = new DateTime(1970, 1, 1);
            msg.CreateTime = baseTime.AddSeconds(sec);
            return msg;
        }
    }
}
