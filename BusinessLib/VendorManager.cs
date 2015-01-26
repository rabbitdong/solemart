using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xxx.EntityLib;
using Xxx.DataAccessLib;

namespace Xxx.BusinessLib {
    public class VendorManager {
        /// <summary>默认的返回每一页的供应商的数量
        /// </summary>
        private const int DEFAULT_EACH_PAGE_COUNT = 10;

        private static VendorManager instance = new VendorManager();

        private int each_page_count = DEFAULT_EACH_PAGE_COUNT;
        private VendorDA vda = VendorDA.Instance;

        private VendorManager(){}

        //存储供应商名称信息的缓存
        private List<VendorInfoCache> vendor_name_cache = null;

        /// <summary>获取供应商的缓存列表
        /// </summary>
        public IList<VendorInfoCache> VendorNameCache {
            get { 
                if(vendor_name_cache == null)
                    vendor_name_cache = vda.GetMostFrequentVendorNameList();
                return vendor_name_cache; 
            }
        }

        /// <summary>获取货品管理对象
        /// </summary>
        public static VendorManager Instance {
            get { return instance; }
        }

        /// <summary>获取或设置获取的每页的供应商的信息的记录数目
        /// </summary>
        public int EACH_PAGE_COUNT {
            get { return each_page_count; }

            set {
                if (each_page_count <= 0)
                    this.each_page_count = value;
            }
        }

        /// <summary>添加一个新的供应商目录
        /// </summary>
        /// <param name="name">供应商名称</param>
        /// <param name="address">供应商地址</param>
        /// <param name="contact_name">联系人姓名</param>
        /// <param name="phone1">联系人电话号码</param>
        /// <returns>成功添加一个供应商，返回true，否则返回false</returns>
        public bool AddNewVendor(string name, string address, string contact_name, string phone1, string url) {
            VendorInfo vinfo = vda.AddNewVendor(name, address, contact_name, phone1, url);
            if (vinfo != null) {
                VendorNameCache.Add(new VendorInfoCache() { VendorName = name, VendorID = vinfo.VendorID });
                return true;
            }

            return false;
        }

        /// <summary>删除一个供应商的项
        /// </summary>
        /// <param name="vendor_id">要删除的供应商ID</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteVendor(int vendor_id) {
            VendorInfoCache vi = VendorNameCache.First(v => v.VendorID == vendor_id);
            if (vi == null)
                return false;

            if (vda.DeleteVendor(vendor_id)) {
                VendorNameCache.Remove(vi);
                return true;
            }

            return false;
        }

        /// <summary>获取供应商的数量
        /// </summary>
        public int VendorTotalCount {
            get { return VendorNameCache.Count; }
        }

        /// <summary>获取供应商分页的页数
        /// </summary>
        public int VendorPageCount {
            get { return (VendorNameCache.Count + each_page_count - 1) / each_page_count; }
        }

        /// <summary>获取供应商的列表
        /// </summary>
        /// <param name="page_index">要获取的页索引，从0开始</param>
        /// <returns>获取的供应商信息的列表</returns>
        /// <remarks>每页的数量通过VendorManager对象的EACH_PAGE_COUNT进行设置</remarks>
        public List<VendorInfo> GetPagedVendorInfos(int page_index) {
            List<VendorInfo> vendors = vda.GetPagedVendorInfoList(page_index, EACH_PAGE_COUNT);

            return vendors;
        }

        /// <summary>获取某个商品的供应商信息
        /// </summary>
        /// <param name="product">要获取供应商信息的商品</param>
        /// <returns>获取到的供应商信息，如果没有返回null</returns>
        public VendorInfo GetVendorInfoByProduct(Product product) {
            return vda.GetVendorInfoByProductID(product.ProductID);
        }
    }
}
