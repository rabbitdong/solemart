using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.Cryptography;

namespace Solemart.BusinessLib {
    /// <summary>公告管理类
    /// </summary>
    public class BillboardManager {
        private static BillboardManager instance = new BillboardManager();

        private BillBoardDA bda = BillBoardDA.Instance;

        /// <summary>目前有效的公告的缓存
        /// </summary>
        private List<BillBoard> valid_billboards_cache = new List<BillBoard>();

        private BillboardManager() {
            valid_billboards_cache = bda.GetValidBillBoardList();
        }

        /// <summary>获取建议管理类实例
        /// </summary>
        public static BillboardManager Instance {
            get { return instance; }
        }

        /// <summary>发布一条新的公告
        /// </summary>
        /// <param name="content">公告的内容</param>
        /// <param name="AbortTime">公告的终止时间</param>
        /// <returns></returns>
        public BillBoard CreateNewBillBoard(string content, DateTime abort_time) {
            int id = bda.CreateNewBillBoard(content, abort_time);
            if (id > 0) {
                BillBoard bb = new BillBoard();
                bb.BillBoardID = id;
                bb.Content = content;
                bb.PublishTime = DateTime.Now;
                bb.AbortTime = abort_time;

                valid_billboards_cache.Add(bb);

                return bb;
            }

            return null;
        }

        /// <summary>获取有效的公告的列表
        /// </summary>
        /// <returns>获取到的有效的公告的列表</returns>
        /// <remarks>有效的公告列表表示时间未超期的公告</remarks>
        public List<BillBoard> GetValidBillBoardList() {
            return valid_billboards_cache;
        }
    }

    /// <summary>建议的管理类
    /// </summary>
    public class AdviseManager {
        private static AdviseManager instance = new AdviseManager();

        private AdviserDA ada = AdviserDA.Instance;

        private AdviseManager() { }

        /// <summary>获取建议管理类实例
        /// </summary>
        public static AdviseManager Instance {
            get { return instance; }
        }

        /// <summary>获取分页的建议列表
        /// </summary>
        /// <param name="page_index">页索引，从0开始</param>
        /// <returns>获取到的建议列表</returns>
        public List<Adviser> GetPagedAdvise(int page_index) {
            return ada.GetPagedAdvise(page_index, 10);
        }

        /// <summary>用户发表一个建议
        /// </summary>
        /// <param name="user">发表建议的用户ID</param>
        /// <param name="content">建议的内容</param>
        /// <returns>是否成功发表</returns>
        public bool NewAdvise(User user, string content) {
            return ada.AddNewAdvise(user.UserID, content);
        }
    }
}
