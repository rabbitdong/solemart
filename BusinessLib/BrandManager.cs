using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xxx.EntityLib;
using Xxx.DataAccessLib;

namespace Xxx.BusinessLib {
    /// <summary>品牌管理类
    /// </summary>
    public class BrandManager {
        private static BrandManager instance = new BrandManager();

        private ProductDA prod_da = ProductDA.Instance;
        private BrandDA brand_da = BrandDA.Instance;

        private List<BrandInfo> brands_cache = new List<BrandInfo>();        

        private BrandManager() {
            GetAllUsedBrands();
        }

        /// <summary>获取品牌管理对象
        /// </summary>
        public static BrandManager Instance {
            get { return instance; }
        }

        /// <summary>取得目前使用到的品牌列表
        /// </summary>
        /// <returns>获取到的品牌列表</returns>
        public List<BrandInfo> GetAllUsedBrands() {
            if (brands_cache.Count == 0)
                brands_cache = brand_da.GetAllBrandInfo();

            return brands_cache;
        }

        /// <summary>获取某个ID的品牌信息
        /// </summary>
        /// <param name="brand_id">要获取的品牌的ID</param>
        /// <returns>返回该ID的品牌，如果没有，返回null</returns>
        public BrandInfo GetBrandByID(int brand_id) {
            return brands_cache.Find(b => b.BrandID == brand_id);
        }

        /// <summary>根据brand信息，决定该brand的存储的LOGO文件名
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public string GetLogoFileName(BrandInfo brand, string mimetype) {
            string ext = mimetype.Substring(mimetype.IndexOf('/') + 1);

            return string.Format("{0}.{1}", brand.Name, ext);
        }

        /// <summary>添加一个新的品牌信息
        /// </summary>
        /// <param name="name">品牌的名称</param>
        /// <param name="description">品牌的描述信息</param>
        /// <returns>新创建的品牌信息对象</returns>
        public BrandInfo AddNewBrand(string name, string name_en, string url, string description) {
            BrandInfo brand = brand_da.AddNewBrand(name, name_en, url, description);
            //添加成功后要更新缓存
            if (brand != null) 
                brands_cache.Add(brand);            

            return brand;
        }

        /// <summary>判断某个品牌的名称是否已经存在
        /// </summary>
        /// <param name="brand_name">品牌的名称</param>
        /// <returns>该品牌是否已经存在</returns>
        public bool IsBrandExist(string brand_name, string name_en) {
            return brands_cache.Exists(b => (b.Name == brand_name) || (b.EnName == name_en));
        }

        /// <summary>更新了品牌的LOGO图片
        /// </summary>
        /// <param name="brand">要更新的品牌</param>
        /// <returns>是否更新成功</returns>
        public bool UpdateBrandImage(BrandInfo brand, string image) {
            if (brand_da.UpdateBrandImage(brand.BrandID, image)) {
                brand.Image = image;
                return true;
            }

            return false;
        }
    }
}
