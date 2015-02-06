using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Solemart.EntityLib;
using Solemart.DataAccessLib;

namespace Solemart.BusinessLib {
    public class Searcher {
        private SearcherDA sda = SearcherDA.Instance;
        private ProductManager pm = ProductManager.Instance;

        private static Searcher instance = new Searcher();

        private Thread index_t = null;  //生成索引的线程
        private Mutex mutex = null;

        private string[] preset_keywords = {"耳机", "蓝牙", "鼠标", "手机膜", "TF", "内存卡", "音箱", "耳麦", "线", "充电器" };

        private string[] keywords = null;

        /// <summary>LOG的缓存
        /// </summary>
        private Stack<string> log_cache = new Stack<string>();

        private Searcher() {
            mutex = new Mutex();
            index_t = new Thread(new ThreadStart(Thread_Run));
            //index_t.IsBackground = true;
            //index_t.Start();
        }

        /// <summary>运行的写线程
        /// </summary>
        /// <param name="o"></param>
        private void Thread_Run() {
            while (true) {
                mutex.WaitOne(); //等待队列慢


            }
        }

        /// <summary>按某个关键字搜索的产品ID
        /// </summary>
        /// <param name="keyword">要搜索的关键字</param>
        /// <returns>搜索到的产品ID列表</returns>
        private int[] GetProductIdsByKeyword(string keyword) {
            string key = keyword.ToLower();
            List<Product> sale_products = pm.SaleProducts;
            SortedSet<int> ids = new SortedSet<int>();

            foreach (Product prod in sale_products) {
                if (prod.Name.ToLower().IndexOf(key) != -1)
                    ids.Add(prod.ProductID);
                else if (prod.Desc.ToLower().IndexOf(key) != -1)
                    ids.Add(prod.ProductID);
                else if (prod.Spec.ToLower().IndexOf(key) != -1)
                    ids.Add(prod.ProductID);
                else if (prod.OwnedCategory.CateName.ToLower().IndexOf(key) != -1)
                    ids.Add(prod.ProductID);
            }

            return ids.ToArray<int>();
        }

        /// <summary>获取搜索对象的实例
        /// </summary>
        public static Searcher Instane {
            get { return instance; }
        }

        /// <summary>获取IDS表示的字符串
        /// </summary>
        /// <param name="ids">整数数组</param>
        /// <returns>获取的ID列表</returns>
        private string GetIdsStr(int[] ids) {
            StringBuilder sb = new StringBuilder(ids[0].ToString());
            for (int i = 0; i < ids.Length; ++i) {
                sb.AppendFormat(",{0}", ids[i]);
            }

            return sb.ToString();
        }

        /// <summary>从ID的字符串获取整数的ID列表
        /// </summary>
        /// <param name="ids_str">ID的字符串</param>
        /// <returns>ID的整数数组</returns>
        private int[] GetIdsFromIdsStr(string ids_str) {
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
        public IList<Product> GetPagedProductsByKey(string key) {
            int[] ids = GetProductIdsByKeyword(key);
            if (ids != null && ids.Length > 0) {                
                sda.SaveSearchResult(key, GetIdsStr(ids));
            }

            return pm.GetProductListByIds(ids);
        }
    }
}
