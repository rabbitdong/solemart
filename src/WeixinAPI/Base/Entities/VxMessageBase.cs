using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WeixinAPI.Base.Entities
{
    #region Weixin的消息类型
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
    #endregion

    #region Weixin的推送事件类型
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
    #endregion

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

        #region 从微信消息中解析实体
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
                    (msg as VxMessageText).MsgId = long.Parse(xml.Root.Element("MsgId").Value);
                    (msg as VxMessageText).Content = xml.Root.Element("Content").Value;
                    break;
                case VxMessageType.Image:
                    msg = new VxMessageImage();
                    (msg as VxMessageImage).MsgId = long.Parse(xml.Root.Element("MsgId").Value);
                    (msg as VxMessageImage).MediaId = xml.Root.Element("MediaId").Value;
                    (msg as VxMessageImage).PicUrl = xml.Root.Element("PicUrl").Value;
                    break;
                case VxMessageType.Location:
                    msg = new VxMessageLocation();
                    (msg as VxMessageLocation).MsgId = long.Parse(xml.Root.Element("MsgId").Value);
                    (msg as VxMessageLocation).Label = xml.Root.Element("Label").Value;
                    (msg as VxMessageLocation).Location_X = double.Parse(xml.Root.Element("Location_X").Value);
                    (msg as VxMessageLocation).Location_Y = double.Parse(xml.Root.Element("Location_Y").Value);
                    (msg as VxMessageLocation).Scale = int.Parse(xml.Root.Element("Scale").Value);
                    break;
                case VxMessageType.Link:
                    msg = new VxMessageLink();
                    (msg as VxMessageLink).MsgId = long.Parse(xml.Root.Element("MsgId").Value);
                    (msg as VxMessageLink).Description = xml.Root.Element("Description").Value;
                    (msg as VxMessageLink).Title = xml.Root.Element("Title").Value;
                    (msg as VxMessageLink).Url = xml.Root.Element("Url").Value;
                    break;
                case VxMessageType.ShortVideo:
                    msg = new VxMessageShortVideo();
                    (msg as VxMessageShortVideo).MsgId = long.Parse(xml.Root.Element("MsgId").Value);
                    (msg as VxMessageShortVideo).MediaId = xml.Root.Element("MediaId").Value;
                    (msg as VxMessageShortVideo).ThumbMediaId = xml.Root.Element("ThumbMediaId").Value;
                    break;
                case VxMessageType.Video:
                    msg = new VxMessageVideo();
                    (msg as VxMessageVideo).MsgId = long.Parse(xml.Root.Element("MsgId").Value);
                    (msg as VxMessageVideo).MediaId = xml.Root.Element("MediaId").Value;
                    (msg as VxMessageVideo).ThumbMediaId = xml.Root.Element("ThumbMediaId").Value;
                    break;
                case VxMessageType.Voice:
                    msg = new VxMessageVoice();
                    (msg as VxMessageVoice).MsgId = long.Parse(xml.Root.Element("MsgId").Value);
                    (msg as VxMessageVoice).MediaId = xml.Root.Element("MediaId").Value;
                    (msg as VxMessageVoice).Format = xml.Root.Element("Format").Value;
                    break;
                case VxMessageType.Event:
                    VxEventType eventType = (VxEventType)Enum.Parse(typeof(VxEventType), xml.Root.Element("Event").Value, true);
                    switch (eventType)
                    {
                        case VxEventType.CLICK:
                            msg = new VxMessageEventClick();
                            (msg as VxMessageEventClick).EventKey = xml.Root.Element("EventKey").Value;
                            break;
                        case VxEventType.LOCATION:
                            msg = new VxMessageEventLocation();
                            (msg as VxMessageEventLocation).Latitude = double.Parse(xml.Root.Element("Latitude").Value);
                            (msg as VxMessageEventLocation).Longitude = double.Parse(xml.Root.Element("Longitude").Value);
                            (msg as VxMessageEventLocation).Precision = double.Parse(xml.Root.Element("Precision").Value);
                            break;
                        case VxEventType.SCAN:
                            msg = new VxMessageEventScan();
                            (msg as VxMessageEventScan).EventKey = xml.Root.Element("EventKey").Value;
                            (msg as VxMessageEventScan).Ticket = xml.Root.Element("Ticket").Value;
                            break;
                        case VxEventType.subscribe:
                            msg = new VxMessageEventSubscribe();
                            break;
                        case VxEventType.unsubscribe:
                            msg = new VxMessageEventUnsubscribe();
                            break;
                        case VxEventType.VIEW:
                            msg = new VxMessageEventView();
                            (msg as VxMessageEventView).EventKey = xml.Root.Element("EventKey").Value;
                            break;
                    }
                    break;
            }

            msg.ToUserName = xml.Root.Element("ToUserName").Value;
            msg.FromUserName = xml.Root.Element("FromUserName").Value;
            int sec = int.Parse(xml.Root.Element("CreateTime").Value);
            DateTime baseTime = new DateTime(1970, 1, 1);
            msg.CreateTime = baseTime.AddSeconds(sec);
            return msg;
        }
        #endregion
    }
}
