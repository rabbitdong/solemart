using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Solemart.DataProvider.Entity
{
    #region 用户建议项的实体  AdviserItem
    /// <summary>
    /// 用户建议项的实体
    /// </summary>
    public class AdviserItem {
        /// <summary>
        /// The advise unique id.
        /// </summary>
        [Key]
        public int AdviseID { get; set; }

        /// <summary>
        /// 获取或设置用户的ID（-1表示匿名用户)
        /// </summary>
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual UserItem User { get; set; }
        /// <summary>
        /// 用户的建议内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 获取或设置提建议的时间
        /// </summary>
        public DateTime AdviseTime { get; set; }

        /// <summary>
        /// 该建议是否被采纳
        /// </summary>
        public bool IsViewed { get; set; }
    }
    #endregion

    #region 系统的公告项的实体 BulletinItem
    /// <summary>
    /// 系统的公告项的实体
    /// </summary>
    public class BulletinItem {
        /// <summary>
        /// 公告ID
        /// </summary>
        [Key]
        public int BulletinID { get; set; }

        /// <summary>
        /// 公告的内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 公告的发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }

        /// <summary>
        /// 公告的有效截至日期
        /// </summary>
        public DateTime AbortTime { get; set; }
    }
    #endregion
}
