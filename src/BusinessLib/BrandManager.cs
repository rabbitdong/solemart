using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.DataProvider;
using Solemart.DataProvider.Entity;

namespace Solemart.BusinessLib
{
    /// <summary>品牌管理类
    /// </summary>
    public class BrandManager
    {
        /// <summary>取得目前使用到的品牌列表
        /// </summary>
        /// <returns>获取到的品牌列表</returns>
        public static List<BrandItem> GetAllUsedBrands()
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.BrandItems.ToList();
            }
        }

        /// <summary>
        /// Get the brand info by the brand id
        /// </summary>
        /// <param name="brandID">要获取的品牌的ID</param>
        /// <returns>返回该ID的品牌，如果没有，返回null</returns>
        public static BrandItem GetBrandByID(int brandID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.BrandItems.Find(brandID);
            }
        }

        /// <summary>
        /// 根据brand信息，决定该brand的存储的LOGO文件名
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public static string GetLogoFileName(BrandItem brand, string mimetype)
        {
            string ext = mimetype.Substring(mimetype.IndexOf('/') + 1);

            return string.Format("{0}.{1}", brand.EnName, ext);
        }

        /// <summary>
        /// Add new brand info
        /// </summary>
        /// <param name="newBrand">品牌的名称</param>
        /// <returns>新创建的品牌信息对象</returns>
        public static BrandItem AddNewBrand(BrandItem newBrand)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                context.BrandItems.Add(newBrand);
                if (context.SaveChanges() > 0)
                    return newBrand;

                return null;
            }
        }

        /// <summary>
        /// 判断某个品牌的名称是否已经存在
        /// </summary>
        /// <param name="brand_name">品牌的名称</param>
        /// <returns>该品牌是否已经存在</returns>
        public static bool IsBrandExist(string zhName, string enName)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.BrandItems.First(b => (b.ZhName == zhName || b.EnName == enName)) != null;
            }
        }

        /// <summary>更新了品牌的LOGO图片
        /// </summary>
        /// <param name="newBrand">要更新的品牌</param>
        /// <returns>是否更新成功</returns>
        public static bool UpdateBrandItem(BrandItem newBrand)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                BrandItem oldBrand = context.BrandItems.Find(newBrand.BrandID);
                if (oldBrand != null)
                {
                    oldBrand.ZhName = newBrand.ZhName;
                    oldBrand.EnName = newBrand.EnName;
                    oldBrand.BrandLogo = newBrand.BrandLogo;
                    oldBrand.BrandUrl = newBrand.BrandUrl;
                    oldBrand.Description = newBrand.Description;
                    return context.SaveChanges() > 0;
                }

                return false;
            }
        }
    }
}
