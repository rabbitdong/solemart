using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xxx.EntityLib {
    /// <summary>表示用户的诉求和建议的实体
    /// </summary>
    public class Adviser {
        /// <summary>获取或设置用户的ID（-1表示匿名用户)
        /// </summary>
        public int UserID { get; set; }

        /// <summary>用户的建议内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>获取或设置提建议的时间
        /// </summary>
        public DateTime AdviseTime { get; set; }

        /// <summary>该建议是否被采纳
        /// </summary>
        public bool IsViewed { get; set; }
    }

    /// <summary>表示系统的公告
    /// </summary>
    public class BillBoard {
        /// <summary>公告ID
        /// </summary>
        public int BillBoardID { get; set; }

        /// <summary>公告的内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>公告的发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }

        /// <summary>公告的有效截至日期
        /// </summary>
        public DateTime AbortTime { get; set; }
    }
}
