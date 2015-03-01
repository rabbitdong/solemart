using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.DataProvider;
using Solemart.DataProvider.Entity;

namespace Solemart.BusinessLib 
{
    public class VendorManager 
    {
        /// <summary>
        /// Add a new vendor item
        /// </summary>
        /// <param name="vendor">供应商名称</param>
        /// <returns>成功添加一个供应商，返回true，否则返回false</returns>
        public static bool AddNewVendor(VendorItem vendor) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                VendorItem oldVendor = context.VendorItems.FirstOrDefault(v => v.VendorName == vendor.VendorName);
                if (oldVendor != null)
                    return false;
                context.VendorItems.Add(vendor);
                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Delete a vendor item
        /// </summary>
        /// <param name="vendorID">要删除的供应商ID</param>
        /// <returns>是否删除成功</returns>
        public static bool DeleteVendor(int vendorID) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                VendorItem vendor = context.VendorItems.Find(vendorID);
                if (vendor != null)
                {
                    context.VendorItems.Remove(vendor);
                    return context.SaveChanges() > 0;
                }

                return false;
            }
        }

        /// <summary>
        /// Get the paged vendor list
        /// </summary>
        /// <param name="pageIndex">要获取的页索引，从0开始</param>
        /// <param name="pageSize"></param>
        /// <param name="totalPageCount"></param>
        /// <returns>获取的供应商信息的列表</returns>
        public static List<VendorItem> GetPagedVendorInfos(int pageIndex, int pageSize, out int totalPageCount) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                var q = from v in context.VendorItems
                        orderby v.RecordTime descending
                        select v;
                totalPageCount = (q.Count() - 1) / pageSize + 1;
                return q.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// Get the vendor info of the product
        /// </summary>
        /// <param name="productID">要获取供应商信息的商品</param>
        /// <returns>获取到的供应商信息，如果没有返回null</returns>
        public static VendorItem GetVendorInfoByProduct(int productID) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                ProductItem product = context.ProductItems.Include("Vendor").FirstOrDefault(p => p.ProductID == productID);
                if (product == null)
                    return null;

                return product.Vendor;
            }
        }
    }
}
