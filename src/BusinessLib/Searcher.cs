using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Solemart.DataProvider;
using Solemart.DataProvider.Entity;

namespace Solemart.BusinessLib
{
    public class Searcher
    {
        private Thread index_t = null;  //生成索引的线程
        private Mutex mutex = null;

        private string[] preset_keywords = { "耳机", "蓝牙", "鼠标", "手机膜", "TF", "内存卡", "音箱", "耳麦", "线", "充电器" };

        /// <summary>LOG的缓存
        /// </summary>
        private Stack<string> log_cache = new Stack<string>();

        /// <summary>运行的写线程
        /// </summary>
        /// <param name="o"></param>
        private void Thread_Run()
        {
            while (true)
            {
                mutex.WaitOne(); //等待队列慢


            }
        }

        /// <summary>获取IDS表示的字符串
        /// </summary>
        /// <param name="ids">整数数组</param>
        /// <returns>获取的ID列表</returns>
        private string GetIdsStr(int[] ids)
        {
            StringBuilder sb = new StringBuilder(ids[0].ToString());
            for (int i = 0; i < ids.Length; ++i)
            {
                sb.AppendFormat(",{0}", ids[i]);
            }

            return sb.ToString();
        }

        /// <summary>从ID的字符串获取整数的ID列表
        /// </summary>
        /// <param name="ids_str">ID的字符串</param>
        /// <returns>ID的整数数组</returns>
        private int[] GetIdsFromIdsStr(string ids_str)
        {
            string[] ids_arr = ids_str.Split(',');
            int len = ids_arr.Length;
            int[] ids = new int[len];

            for (int i = 0; i < len; ++i)
                int.TryParse(ids_arr[i], out ids[i]);

            return ids;
        }

        /// <summary>按关键字搜索后返回的产品列表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IList<ProductItem> GetPagedProductsByKey(string key)
        {
            return null;
        }
    }
}
