using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using Solemart.DataProvider.Entity;
using Solemart.DataProvider;

namespace Solemart.BusinessLib {
    /// <summary>
    /// Bulletin Manager
    /// </summary>
    public class BillboardManager 
    {
        /// <summary>
        /// Publish a new bulletin
        /// </summary>
        /// <param name="bulletin">The bulletin information</param>
        /// <returns></returns>
        public bool CreateNewBillBoard(BulletinItem bulletin) {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                bulletin.PublishTime = DateTime.Now;
                context.BulletinItems.Add(bulletin);
                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Get the valid bulletin list
        /// </summary>
        /// <returns>获取到的有效的公告的列表</returns>
        /// <remarks>有效的公告列表表示时间未超期的公告</remarks>
        public List<BulletinItem> GetValidBillBoardList() {
            return null;
        }
    }

    /// <summary>
    /// The manager class of the advise
    /// </summary>
    public class AdviseManager {
        /// <summary>
        /// Get the advise list
        /// </summary>
        /// <param name="pageIndex">页索引，从0开始</param>
        /// <param name="pageSize">The page size</param>
        /// <param name="totalPageCount">The total page count</param>
        /// <returns>获取到的建议列表</returns>
        public List<AdviserItem> GetPagedAdvises(int pageIndex, int pageSize, out int totalPageCount)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                var q = from a in context.AdviserItems
                        orderby a.AdviseTime descending
                        select a;
                totalPageCount = (q.Count() - 1) / pageSize + 1;
                return q.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// The user publish a advise
        /// </summary>
        /// <param name="user">发表建议的用户ID</param>
        /// <param name="content">建议的内容</param>
        /// <returns>是否成功发表</returns>
        public bool NewAdvise(UserItem user, string content) {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                AdviserItem advise = new AdviserItem();
                advise.UserID = user.UserID;
                advise.Content = content;
                advise.AdviseTime = DateTime.Now;
                advise.IsViewed = false;
                context.AdviserItems.Add(advise);

                return context.SaveChanges() > 0;
            }
        }
    }
}
