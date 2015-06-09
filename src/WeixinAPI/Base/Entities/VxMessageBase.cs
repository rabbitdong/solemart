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
    /// 所有Request和Response消息的基类
    /// </summary>
    public class VxMessageBase
    {
        private const DateTime BeginDateTime = new DateTime(1970, 1, 1);

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

        public static virtual VxMessageBase FromXml(XDocument xml)
        {
            VxMessageBase msg=new VxMessageBase();
            msg.MsgType = (VxMessageType)Enum.Parse(typeof(VxMessageType), xml.Root.Element("MsgType").Value, true);
            msg.ToUserName = xml.Root.Element("ToUserName").Value;
            msg.FromUserName = xml.Root.Element("FromUserName").Value;
            int sec = int.Parse(xml.Root.Element("CreateTime").Value);
            DateTime baseTime = new DateTime(1970, 1, 1);
            msg.CreateTime = baseTime.AddSeconds(sec);
            return msg;
        }
    }
}
