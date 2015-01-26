using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xxx.EntityLib;
using Xxx.DataAccessLib;
using System.Threading;
using System.IO;
using System.Security.Cryptography;

namespace Xxx.BusinessLib {
    /// <summary>系统管理类
    /// </summary>
    public class SystemManager {
        private static SystemManager instance = new SystemManager();
        private SystemManager() { }
        private byte[] key = { 0x10, 0x35, 0x28, 0x84, 0xc5, 0x02, 0x3a, 0x7f };
        private byte[] IV = { 0x51, 0x73, 0xa1, 0x1a, 0x07, 0x42, 0x86, 0xce };

        /// <summary>获取系统管理类型的菜单
        /// </summary>
        public static SystemManager Instance {
            get { return instance; }
        }

        /// <summary>初始化管理员数据库
        /// </summary>
        public void InitAdminAccount() {
            UserManager um=UserManager.Instance;

            if (um.TotalUserCount == 0) {
                User user = um.AddNewUser("xxx", "xxx", "xxx@xxx.com");
                um.ModifyUserRole(user.UserID, Role.SuperRole);
            }
        }

        /// <summary>对字符串进行加密操作
        /// </summary>
        /// <param name="planttext">要加密的明文</param>
        /// <returns>加密后的密文</returns>
        public string EncryptString(string planttext) {
            DESCryptoServiceProvider crypto = new DESCryptoServiceProvider();
            crypto.Key = key;
            crypto.IV = IV;

            byte[] plant_data = Encoding.UTF8.GetBytes(planttext);
            
            MemoryStream ms=new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, crypto.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(plant_data, 0, plant_data.Length);
            cs.FlushFinalBlock();

            StringBuilder ciphertext = new StringBuilder();
            byte[] cipher_data = ms.ToArray();
            foreach (byte cp in cipher_data)
                ciphertext.AppendFormat("{0:X2}", cp);

            return ciphertext.ToString();
        }

        /// <summary>对字符串进行解密
        /// </summary>
        /// <param name="ciphertext">要解密的密文</param>
        /// <returns>解密后的明文</returns>
        public string DecryptString(string ciphertext) {
            DESCryptoServiceProvider crypto = new DESCryptoServiceProvider();
            crypto.Key = key;
            crypto.IV = IV;

            //由于密文是16进制编码，所有字节长度是1/2
            int length = ciphertext.Length / 2;
            byte[] cipher_data = new byte[length];
            for (int i = 0; i < length; ++i) {
                cipher_data[i] = (byte)(Convert.ToInt32(ciphertext.Substring(i * 2, 2), 16));
            }

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, crypto.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipher_data, 0, length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }

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
