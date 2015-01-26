using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xxx.EntityLib {

    /// <summary>电子书类型的预设值
    /// </summary>
    public enum EBookMimeTypeValue {
        [StringValue("PDF文档")]
        Pdf,
        [StringValue("Word文档")]
        MsWord,
        [StringValue("PPT文档")]
        MsPPT
    }

    /// <summary>电子书的文件类型
    /// </summary>
    public class EBookMimeType {
        /// <summary>获取或设置电子书类型的ID
        /// </summary>
        public EBookMimeTypeValue TypeValue { get; set; }

        /// <summary>获取或设置该类型的扩展名
        /// </summary>
        public string TypeExt { get; set; }

        /// <summary>获取或设置该类型的MIME类型
        /// </summary>
        public string TypeMime { get; set; }
    }

    public class BookCategory { }

    /// <summary>电子书的实体类
    /// </summary>
    public class EBook {
        public int EBookID { get; set; }

        public string EBookName { get; set; }

        public EBookMimeType MimeType { get; set; }

        public BookCategory Category { get; set; }

        public string BookPath { get; set; }

        public int ViewTimes { get; set; }

        public int DownloadTimes { get; set; }

        public string Synopsis { get; set; }
    }
}
