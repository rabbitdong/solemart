﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.SystemUtil;
using Solemart.DataProvider;
using Solemart.DataProvider.Entity;

namespace Solemart.BusinessLib
{
    /// <summary>货品管理类
    /// </summary>
    public class ProductManager
    {
        private static List<SaledProductItem> allSaledProducts;
        private static List<ProductImageItem> allProductLogoImages;

        #region 商品的查询处理
        /// <summary>
        /// Get the total amount of the saled products.
        /// </summary>
        public static int TotalProductCount
        {
            get
            {
                using (SolemartDBContext context = new SolemartDBContext())
                {
                    return context.SaledProductItems.Count();
                }
            }
        }

        /// <summary>
        /// Get the product on saled by the product id
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static SaledProductItem GetSaledProductByID(int productID)
        {
            return allSaledProducts.FirstOrDefault(p => p.ProductID == productID);
        }

        /// <summary>
        /// Get the product by the product id.
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static ProductItem GetProductByID(int productID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.ProductItems.Find(productID);
            }
        }

        /// <summary>
        /// Get the product (with brand information) by the product id.
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static ProductItem GetProductWithBrandByID(int productID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.ProductItems.Include("Brand").FirstOrDefault(p => p.ProductID == productID);
            }
        }

        /// <summary>
        /// Get the paged all product
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<ProductItem> GetPagedAllProducts(int pageIndex, int pageSize, out int totalCount)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                var q = from p in context.ProductItems.Include("SaledProduct").Include("Category")
                        orderby p.ProductID
                        select p;
                totalCount = q.Count();
                return q.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// Get the paged product list on saling.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<SaledProductItem> GetPagedSaledProducts(int pageIndex, int pageSize, out int totalCount)
        {
            if (allSaledProducts == null || allSaledProducts.Count == 0)
            {
                using (SolemartDBContext context = new SolemartDBContext())
                {
                    var q = from p in context.SaledProductItems.Include("Product")
                            orderby p.ProductID
                            select p;
                    totalCount = q.Count();
                    allSaledProducts = q.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                }
            }

            totalCount = allSaledProducts.Count;
            return allSaledProducts;
        }
        #endregion

        #region 上架/下架处理
        /// <summary>
        /// Put on the product for saling.
        /// </summary>
        /// <param name="newSaledProductItem">The product put to saling.</param>
        /// <returns>return true if success, or return false</returns>
        public static bool PutToSaling(SaledProductItem newSaledProductItem)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                SaledProductItem oldSaledProductItem = context.SaledProductItems.FirstOrDefault(spi => (spi.ProductID == newSaledProductItem.ProductID));
                if (oldSaledProductItem != null)
                {
                    oldSaledProductItem.Price = newSaledProductItem.Price;
                    oldSaledProductItem.Discount = newSaledProductItem.Discount;
                    oldSaledProductItem.SpecialFlag = newSaledProductItem.SpecialFlag;
                }
                else
                    context.SaledProductItems.Add(newSaledProductItem);

                if(context.SaveChanges() > 0)
                {
                    if(allSaledProducts != null)
                        allSaledProducts.Clear();
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Take off the product for saling
        /// </summary>
        /// <param name="productID">要下架的商品的ID</param>
        /// <returns>是否下架成功</returns>
        public static bool TakeOffSaling(int productID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                SaledProductItem saledProductItem = context.SaledProductItems.Find(productID);
                if (saledProductItem != null)
                {
                    context.SaledProductItems.Remove(saledProductItem);
                    if (context.SaveChanges() > 0)
                    {
                        if (allSaledProducts != null)
                            allSaledProducts.Clear();
                        return true;
                    }                    
                }

                return false;
            }
        }

        /// <summary>
        /// Get the last price of the product in stock
        /// </summary>
        /// <param name="productID">要获取的商品的ID</param>
        /// <returns>商品的最后的入库价格</returns>
        public static decimal GetLastStockPrice(int productID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                var query = from p in context.InStockItems
                            where p.ProductID == productID
                            orderby p.InStockTime descending
                            select p;
                return query.First().Price;
            }
        }

        /// <summary>
        /// Set the saled product to Top.(recommend the product to user)
        /// </summary>
        /// <param name="productID">The product id</param>
        /// <returns></returns>
        public static bool SetTop(int productID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                SaledProductItem saledProductItem = context.SaledProductItems.Find(productID);
                if (saledProductItem != null)
                {
                    saledProductItem.SetTop = true;
                    if (context.SaveChanges() > 0)
                    {
                        if (allSaledProducts != null)
                            allSaledProducts.Clear();
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Drawback the recommend product.
        /// </summary>
        /// <param name="productID">The product id</param>
        /// <returns></returns>
        public static bool DrawbackTop(int productID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                SaledProductItem saledProductItem = context.SaledProductItems.Find(productID);
                if (saledProductItem != null)
                {
                    saledProductItem.SetTop = false;
                    if (context.SaveChanges() > 0)
                    {
                        if (allSaledProducts != null)
                            allSaledProducts.Clear();
                        return true;
                    }
                }

                return false;
            }
        }
        #endregion

        #region 商品的图片处理
        /// <summary>
        /// Add an image to the product
        /// </summary>
        /// <param name="image"></param>
        /// <returns>是否添加成功</returns>
        public static bool AddNewImageToProduct(ProductImageItem image)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                image.AddTime = DateTime.Now;
                context.ProductImageItems.Add(image);
                if (context.SaveChanges() > 0)
                {
                    if (allProductLogoImages != null)
                        allProductLogoImages.Clear();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Get the file extend name from the mimetype.
        /// </summary>
        /// <param name="mimeType">MIME类型，如"image/jpeg"等</param>
        /// <returns></returns>
        public static string FromMimeTypeGetExtendName(string mimeType)
        {
            string[,] mapping = { { "image/jpeg", "jpg" }, { "image/png", "png" }, { "image/gif", "gif" } };

            int len = mapping.Length;
            for (int i = 0; i < len; ++i)
            {
                if (mimeType == mapping[i, 0])
                    return mapping[i, 1];
            }

            return "jpg";
        }

        /// <summary>
        /// Generate the image filename for the product.
        /// </summary>
        /// <param name="productID">商品ID</param>
        /// <param name="extName">图片的扩展名, 如jpg, png等</param>
        /// <returns>图像名称的字符串</returns>
        /// <remarks>图像名称定为 pid_+编号.xxx</remarks>
        public static string GenerateProductImageFileName(int productID, string extName)
        {
#if TEST            
            string pathFmt = "test{0}_{1}.{2}";
#else
            string pathFmt = "{0}_{1}.{2}";
#endif
            using (SolemartDBContext context = new SolemartDBContext())
            {
                ProductItem product = context.ProductItems.Find(productID);
                if (product == null)
                    return "";

                int startIndex = 0;
                int endIndex = 0;

                List<ProductImageItem> imageList = context.ProductImageItems.Where(i => (i.ProductID == productID)).ToList();
                if (imageList == null || imageList.Count == 0)
                    return string.Format(pathFmt, product.ProductID, 0, extName);

                int count = imageList.Count;
                int[] imgSequence = new int[count];

                for (int idx = 0; idx < imageList.Count; ++idx)
                {
                    string url = imageList[idx].ImageUrl;
                    startIndex = url.IndexOf('_') + 1;
                    endIndex = url.LastIndexOf('.');
                    if (!int.TryParse(url.Substring(startIndex, endIndex - startIndex), out imgSequence[idx]))
                        continue;
                }

                Array.Sort<int>(imgSequence);
                int seq = 0;    //序号从0开始
                for (int idx = 0; idx < count; ++idx, ++seq)
                {
                    if (seq < imgSequence[idx])
                        break;
                }

                return string.Format(pathFmt, product.ProductID, seq, extName);
            }

        }

        /// <summary>
        /// Delete a image of the product
        /// </summary>
        /// <param name="pid">要删除图片的商品ID</param>
        /// <param name="imageID">删除的图片的ID</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public static bool DeleteProductImage(int productID, int imageID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                ProductImageItem image = context.ProductImageItems.FirstOrDefault(i => (i.ProductID == productID && i.ImageID == imageID));
                if (image != null)
                {
                    context.ProductImageItems.Remove(image);
                    if (context.SaveChanges() > 0)
                    {
                        if (allProductLogoImages != null)
                            allProductLogoImages.Clear();
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Get the image list of the product
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductImageItem> GetProductNoLogoImage(int productID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.ProductImageItems.Where(p => (p.ProductID == productID && !p.ForLogo)).OrderBy(p=>p.ImageID).ToList();
            }
        }

        /// <summary>
        /// Get the product image of the product
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="imageID"></param>
        /// <returns></returns>
        public static ProductImageItem GetProductImage(int productID, int imageID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.ProductImageItems.FirstOrDefault(p => (p.ProductID == productID && p.ImageID == imageID));
            }
        }

        /// <summary>
        /// Get the product image of the product
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="imageID"></param>
        /// <returns></returns>
        public static IList<ProductImageItem> GetProductAllImage(int productID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.ProductImageItems.Where(p => (p.ProductID == productID)).OrderBy(p => p.ImageID).ToList();
            }
        }

        /// <summary>
        /// Get the product image of the product
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="imageID"></param>
        /// <returns></returns>
        public static ProductImageItem GetProductLogoImage(int productID)
        {
            if (allProductLogoImages == null || allProductLogoImages.Count == 0)
            {
                //using (SolemartDBContext context = new SolemartDBContext())
                //{
                //    return context.ProductImageItems.FirstOrDefault(p => (p.ProductID == productID && p.ForLogo));
                //}
                GetAllProductLogoImage();
            }

            return allProductLogoImages.Find(i => i.ProductID == productID);
        }

        /// <summary>
        /// 获取所有商品的图像（缓存使用）
        /// </summary>
        /// <returns></returns>
        public static List<ProductImageItem> GetAllProductLogoImage()
        {
            if (allProductLogoImages == null || allProductLogoImages.Count == 0)
            {
                using (SolemartDBContext context = new SolemartDBContext())
                {
                    allProductLogoImages = context.ProductImageItems.Where(p => (p.ForLogo == true)).ToList();
                }
            }

            return allProductLogoImages;
        }

        /// <summary>
        /// Set the logo image for product.
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="imageID"></param>
        /// <returns></returns>
        public static bool SetProductLogImage(int productID, int imageID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                ProductImageItem imageItem = context.ProductImageItems.FirstOrDefault(p => (p.ProductID == productID && p.ImageID == imageID));

                ProductImageItem logoImageItem = context.ProductImageItems.FirstOrDefault(p => (p.ProductID == productID && p.ForLogo));
                //如果已经是LOGO的图片
                if (imageItem != null && imageItem.ForLogo)
                    return true;

                //设置的图片不是原来的LOGO图片
                if (imageItem != null)
                {
                    imageItem.ForLogo = true;
                    if (logoImageItem != null)
                        logoImageItem.ForLogo = false;

                    if (context.SaveChanges() > 0)
                    {
                        if (allProductLogoImages != null)
                            allProductLogoImages.Clear();
                        return true;
                    }
                }

                return false;
            }
        }
        #endregion

        #region 商品的库存处理
        /// <summary>
        /// In stock a new product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="price"></param>
        /// <param name="amount"></param>
        /// <param name="remark"></param>
        /// <returns>入库成功，返回true，否则返回false</returns>
        public static bool InStockProduct(ProductItem product, decimal price, int amount, string remark)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                /* 在增加新产品的时候，先不增加库存数量，设置该库存为0，后面入库操作再填写库存数量 */
                ProductItem existProduct = context.ProductItems.Find(product.ProductID);
                if (existProduct == null)
                {
                    //the count of product's stock is the amount for the first time in stock.
                    product.StockCount = amount;
                    context.ProductItems.Add(product);
                }
                else
                {
                    existProduct.StockCount += amount;
                }

                InStockItem stockItem = new InStockItem { ProductID = product.ProductID, Price = price, Amount = amount, InStockTime = DateTime.Now, Remark = remark };
                context.InStockItems.Add(stockItem);
                if (context.SaveChanges() > 0)
                {
                    if (allSaledProducts != null)
                        allSaledProducts.Clear();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Reserve some product in shopping cart.
        /// </summary>
        /// <param name="productID">the id of product want to reserve</param>
        /// <returns>是否保留成功，如果该产品的库存数量大于amount，应该要能保留成功。成功返回true，否则返回false</returns>
        /// <remarks>该接口在用户下订单的时候使用</remarks>
        public static bool ReserveProduct(int productID, int count)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                ProductItem product = context.ProductItems.Find(productID);
                if (product != null)
                {
                    product.ReserveCount += count;
                    return context.SaveChanges() > 0;
                }

                return false;
            }
        }

        /// <summary>
        /// unreserve the product
        /// </summary>
        /// <param name="productID">要去除的商品保留数量列表</param>
        /// <param name="count"></param>
        /// <returns>成功返回true，否则返回false</returns>
        public static bool UnReserveProducts(int productID, int count)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                ProductItem product = context.ProductItems.Find(productID);
                if (product != null && product.ReserveCount >= count)
                {
                    product.ReserveCount += count;
                    return context.SaveChanges() > 0;
                }

                return false;
            }
        }

        /// <summary>
        /// Shipping the product
        /// </summary>
        /// <param name="productIDs">要出货的商品列表</param>
        /// <returns>出货成功返回true，否则返回false</returns>
        public static bool ShippingProducts(IEnumerable<OrderDetailItem> items)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                foreach (OrderDetailItem item in items)
                {
                    ProductItem product = context.ProductItems.Find(item.ProductID);
                    if (product != null && product.ReserveCount >= item.Amount)
                    {
                        product.ReserveCount -= item.Amount;
                        product.StockCount -= item.Amount;
                    }
                }

                return context.SaveChanges() > 0;
            }
        }
        #endregion

        #region 商品的评论处理
        /// <summary>
        /// Make a comment for a product
        /// </summary>
        /// <param name="userID">进行评论的用户</param>
        /// <param name="product">要评论的商品对象</param>
        /// <param name="level">评价的星级</param>
        /// <param name="content">评论的内容</param>
        /// <returns>是否评论成功，成功返回true，否则返回false</returns>
        public static bool CommentProduct(int userID, int productID, EvaluteGrade level, string content)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                ProductCommentItem productComment = new ProductCommentItem();
                productComment.UserID = userID;
                productComment.ProductID = productID;
                productComment.Grade = level;
                productComment.Content = content;
                productComment.CommentTime = DateTime.Now;
                context.ProductCommentItems.Add(productComment);

                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Get a list of the comments for the product
        /// </summary>
        /// <param name="productID">要获取评论的产品对象</param>
        /// <param name="pageIndex">The page index of comment list, index is from 0</param>
        /// <param name="pageSize">获取的每页评论的数量</param>
        /// <param name="totalCount">The total page count of the comments</param>
        /// <returns>获取的评论的列表, 按时间倒序排列</returns>
        public static IList<ProductCommentItem> GetProductComment(int productID, int pageIndex, int pageSize, out int totalCount)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                totalCount = 0;
                var query = from pc in context.ProductCommentItems
                            where pc.ProductID == productID
                            orderby pc.CommentTime descending
                            select pc;
                totalCount = query.Count();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// Get the total comment count of the product
        /// </summary>
        /// <returns>Get the total comment count of the product</returns>
        public static int GetProductCommentCount(int productID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.ProductCommentItems.Count(p=>p.ProductID==productID);
            }
        }
        #endregion

        /// <summary>
        /// Get the list of the product of the category
        /// </summary>
        /// <param name="categoryID">要获取的产品的类别</param>
        /// <param name="pageIndex">要获取的产品的页索引</param>
        /// <param name="pageSize">每页的产品数量</param>
        /// <param name="totalCount"></param>
        /// <returns>获取到的产品的列表</returns>
        public static IList<ProductItem> GetPagedProductsByCategory(int categoryID, int pageIndex, int pageSize, out int totalCount)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                totalCount = 0;
                var query = from p in context.ProductItems
                            where p.CategoryID == categoryID
                            orderby p.ProductID descending
                            select p;
                totalCount = query.Count();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// Modify the the product information
        /// </summary>
        /// <param name="product">The modified product item</param>
        /// <returns>是否修改成功</returns>
        public static bool ModifyProductInfo(ProductItem product)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                ProductItem oldProduct = context.ProductItems.Find(product.ProductID);
                if (oldProduct == null)
                    return false;

                if(!string.IsNullOrWhiteSpace(product.ProductName))
                    oldProduct.ProductName = product.ProductName;
                if(!string.IsNullOrWhiteSpace(product.Unit))
                    oldProduct.Unit = product.Unit;
                if(!string.IsNullOrWhiteSpace(product.Specification))
                    oldProduct.Specification = product.Specification;
                if(!string.IsNullOrWhiteSpace(product.Description))
                    oldProduct.Description = product.Description;
                if (!string.IsNullOrWhiteSpace(product.ProducingArea))
                    oldProduct.ProducingArea = product.ProducingArea;
                oldProduct.CategoryID = product.CategoryID;

                if (context.SaveChanges() > 0)
                {
                    if (allSaledProducts != null)
                        allSaledProducts.Clear();
                    return true;
                }
                return false;
            }
        }
    }
}
