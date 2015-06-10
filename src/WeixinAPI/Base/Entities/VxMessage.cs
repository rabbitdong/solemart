using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeixinAPI.Base.Entities
{
    /// <summary>
    /// Weixin文本消息
    /// </summary>
    public class VxMessageText : VxMessageBase
    {
        public VxMessageText()
        {
            MsgType = VxMessageType.Text;
        }

        public string Content { get; set; }

        public long MsgId { get; set; }
    }

    /// <summary>
    /// Weixin图片消息
    /// </summary>
    public class VxMessageImage : VxMessageBase
    {
        public VxMessageImage()
        {
            MsgType = VxMessageType.Image;
        }

        public string PicUrl { get; set; }

        public string MediaId { get; set; }

        public long MsgId { get; set; }
    }

    /// <summary>
    /// Weixin语音消息
    /// </summary>
    public class VxMessageVoice : VxMessageBase
    {
        public VxMessageVoice()
        {
            MsgType = VxMessageType.Voice;
        }

        public string MediaId { get; set; }

        public string Format { get; set; }

        public long MsgId { get; set; }
    }

    /// <summary>
    /// Weixin视频消息
    /// </summary>
    public class VxMessageVideo : VxMessageBase
    {
        public VxMessageVideo()
        {
            MsgType = VxMessageType.Video;
        }

        public string MediaId { get; set; }

        public string ThumbMediaId { get; set; }

        public long MsgId { get; set; }
    }


    /// <summary>
    /// Weixin小视频消息
    /// </summary>
    public class VxMessageShortVideo : VxMessageBase
    {
        public VxMessageShortVideo()
        {
            MsgType = VxMessageType.ShortVideo;
        }

        public string MediaId { get; set; }

        public string ThumbMediaId { get; set; }

        public long MsgId { get; set; }
    }

    /// <summary>
    /// Weixin地理位置消息
    /// </summary>
    public class VxMessageLocation : VxMessageBase
    {
        public VxMessageLocation()
        {
            MsgType = VxMessageType.Location;
        }

        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public double Location_X { get; set; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Location_Y { get; set; }

        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get; set; }

        public long MsgId { get; set; }
    }

    /// <summary>
    /// Wiexin链接消息
    /// </summary>
    public class VxMessageLink : VxMessageBase
    {
        public VxMessageLink()
        {
            MsgType = VxMessageType.Link;
        }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 消息链接
        /// </summary>
        public string Url { get; set; }

        public long MsgId { get; set; }
    }

    /// <summary>
    /// Weixin事件的基类
    /// </summary>
    public class VxMessageEvent : VxMessageBase
    {
        public VxMessageEvent()
        {
            MsgType = VxMessageType.Event;
        }

        public VxEventType Event { get; set; } 
    }

    /// <summary>
    /// Weixin订阅事件
    /// </summary>
    public class VxMessageEventSubscribe : VxMessageEvent
    {
        public VxMessageEventSubscribe()
        {
            Event = VxEventType.subscribe;
        }
    }

    /// <summary>
    /// Weixin的取消订阅事件
    /// </summary>
    public class VxMessageEventUnsubscribe : VxMessageEvent
    {
        public VxMessageEventUnsubscribe()
        {
            Event = VxEventType.unsubscribe;
        }
    }

    /// <summary>
    /// Weixin扫描二维码关注
    /// </summary>
    public class VxMessageEventSubscribeQR : VxMessageEvent
    {
        public VxMessageEventSubscribeQR()
        {
            Event = VxEventType.subscribe;
        }

        public string EventKey { get; set; }

        public string Ticket { get; set; }
    }

    /// <summary>
    /// Weixin扫描二维码（已经关注）
    /// </summary>
    public class VxMessageEventScan : VxMessageEvent
    {
        public VxMessageEventScan()
        {
            Event = VxEventType.SCAN;
        }

        public string EventKey { get; set; }

        public string Ticket { get; set; }
    }

    /// <summary>
    /// Weixin上报地理位置
    /// 获取用户地理位置（高级接口下才能用）
    /// 获取用户地理位置的方式有两种，一种是仅在进入会话时上报一次，一种是进入会话后每隔5秒上报一次。公众号可以在公众平台网站中设置。
    /// 用户同意上报地理位置后，每次进入公众号会话时，都会在进入时上报地理位置，或在进入会话后每5秒上报一次地理位置，上报地理位置以推送XML数据包到开发者填写的URL来实现。
    /// </summary>
    public class VxMessageEventLocation : VxMessageEvent
    {
        public VxMessageEventLocation()
        {
            Event = VxEventType.LOCATION;
        }

        /// <summary>
        /// 地理位置维度，事件类型为LOCATION的时存在
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 地理位置经度，事件类型为LOCATION的时存在
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 地理位置精度，事件类型为LOCATION的时存在
        /// </summary>
        public double Precision { get; set; }
    }

    /// <summary>
    /// Weixin的自定义菜单选择（动作类）
    /// </summary>
    public class VxMessageEventClick : VxMessageEvent
    {
        public VxMessageEventClick()
        {
            Event = VxEventType.CLICK;
        }

        /// <summary>
        /// 事件KEY值，与自定义菜单接口中KEY值对应
        /// </summary>
        public string EventKey { get; set; }
    }

    /// <summary>
    /// Weixin的自定义菜单选择（转跳类）
    /// </summary>
    public class VxMessageEventView : VxMessageEvent
    {
        public VxMessageEventView()
        {
            Event = VxEventType.VIEW;
        }

        /// <summary>
        /// 事件KEY值，设置的跳转URL
        /// </summary>
        public string EventKey { get; set; }
    }
}
