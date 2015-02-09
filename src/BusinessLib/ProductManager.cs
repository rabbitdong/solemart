using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.SystemUtil;
using Solemart.DataProvider;
using Solemart.DataProvider.Entity;

namespace Solemart.BusinessLib {
    /// <summary>货品管理类
    /// </summary>
    public class ProductManager {
        private static ProductManager instance = new ProductManager();

        /// <summary>
        /// The private constructor
        /// </summary>
        private ProductManager(){ }

        /// <summary>
        /// Get the product manager object
        /// </summary>
        public static ProductManager Instance {
            get { return instance; }
        }

        /// <summary>
        /// Get the total amount of the saled products.
        /// </summary>
        public int TotalProductCount {
            get
            {
                using (SolemartDBContext context = new SolemartDBContext())
                {
                    return context.SaledProductItems.Count();
                }
            }
        }

        #region 上架/下架处理
        /// <summary>
        /// Put on the product for saling.
        /// </summary>
        /// <param name="newSaledProductItem">The product put to saling.</param>
        /// <returns>return true if success, or return false</returns>
        public bool PutToSaling(SaledProductItem newSaledProductItem) {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                SaledProductItem oldSaledProductItem = context.SaledProductItems.First(spi => (spi.ProductID == newSaledProductItem.ProductID));
                if (oldSaledProductItem != null)
                {
                    oldSaledProductItem.Price = newSaledProductItem.Price;
                    oldSaledProductItem.Discount = newSaledProductItem.Discount;
                    oldSaledProductItem.SpecialFlag = newSaledProductItem.SpecialFlag;
                }
                else
                    context.SaledProductItems.Add(newSaledProductItem);

                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Take off the product for saling
        /// </summary>
        /// <param name="productID">要下架的商品的ID</param>
        /// <returns>是否下架成功</returns>
        public bool TakeOffSaling(int productID) {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                SaledProductItem saledProductItem = context.SaledProductItems.Find(productID);
                if (saledProductItem != null)
                {
                    context.SaledProductItems.Remove(saledProductItem);
                    return context.SaveChanges() > 0;
                }

                return false;
            }
        }

        /// <summary>
        /// Get the last price of the product in stock
        /// </summary>
        /// <param name="productID">要获取的商品的ID</param>
        /// <returns>商品的最后的入库价格</returns>
        public decimal GetLastStockPrice(int productID) {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                var query = from p in context.InStockItems
                            where p.ProductID == productID
                            orderby p.InStockTime descending
                            select p;
                return query.First().Price;
            }
        }

        #endregion

        #region 商品的图片处理
        /// <summary>
        /// Add a new image for the product
        /// </summary>
        /// <param name="productID">要添加的产品的PID</param>
        /// <param name="datas">图像数据</param>
        /// <returns>是否添加成功</returns>
        public bool AddNewImageToProduct(int productID, string mimetype, string fileName) {
            int iid = prod_da.AddNewProductImage(productID, mimetype, fileName);
            //添加成功后，刷新缓存
            if (iid > 0) {
                Product product = AllProduct.Find(p => p.ProductID == productID);
                product.Images = prod_da.GetProductImage(productID);
            }

            return iid > 0;
        }

        /// <summary>从MIME类型获取文件的扩展名
        /// </summary>
        /// <param name="mimetype">MIME类型，如"image/jpeg"等</param>
        /// <returns></returns>
        public string FromMimeTypeGetExtendName(string mimetype) {
            string[,] mapping = { { "image/jpeg", "jpg" }, { "image/png", "png" }, { "image/gif", "gif" } };

            int len = mapping.Length;
            for (int i = 0; i < len; ++i) {
                if (mimetype == mapping[i, 0])
                    return mapping[i, 1];
            }

            return "jpg";
        }

        /// <summary>生成一个商品的图像名称
        /// </summary>
        /// <param name="pid">商品ID</param>
        /// <param name="ext_name">图片的扩展名, 如jpg, png等</param>
        /// <returns>图像名称的字符串</returns>
        /// <remarks>图像名称定为 pid_+编号.xxx</remarks>
        public string GenerateProductImageFileName(int pid, string ext_name) {
            Product prod = GetProductByID(pid);
            if(prod == null)
                return "";

            int start_idx = 0;
            int end_idx = 0;
            int img_count = 0;

            if (prod.Images == null || prod.Images.Length == 0)
                return string.Format("{0}_{1}.{2}", prod.ProductID, 0, ext_name);

            img_count = prod.Images.Length;
            int[] img_seqs = new int[img_count];

            for (int idx = 0; idx < img_count; ++idx) {
                string url = prod.Images[idx].Url;
                start_idx = url.IndexOf('_') + 1;
                end_idx = url.LastIndexOf('.');
                if (!int.TryParse(url.Substring(start_idx, end_idx - start_idx), out img_seqs[idx]))
                    break;
            }

            Array.Sort<int>(img_seqs);
            int seq = 0;    //序号从0开始
            for (int idx = 0; idx < img_count; ++idx, ++seq) {
                if (seq < img_seqs[idx])
                    break;
            }

            return string.Format("{0}_{1}.{2}", prod.ProductID, seq, ext_name);
        }

        /// <summary>删除一个商品使用的图片
        /// </summary>
        /// <param name="pid">要删除图片的商品ID</param>
        /// <param name="iid">删除的图片的ID</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public bool DeleteProductImage(Product product, int iid) {
            bool is_my_img = false;
            foreach (ProductImage img in product.Images)
                if (img.ImgID == iid)
                    is_my_img = true;
            //操作成功后，刷新缓存
            if (is_my_img && prod_da.DeleteProductImage(iid)) {
                Product prod = AllProduct.Find(p => p.ProductID == product.ProductID);
                prod.Images = prod_da.GetProductImage(product.ProductID);
                return true;
            }

            return false;
        }
        #endregion

        #region 商品的库存处理
        /// <summary>入库一个货品
        /// </summary>
        /// <param name="prod_name">货品名称</param>
        /// <param name="cate_id">货品所属的类别ID</param>
        /// <param name="fact_id">生产商品的厂家ID</param>
        /// <param name="prod_spec">商品的规格信息</param>
        /// <param name="brand_id">商品的品牌ID</param>
        /// <param name="inprice">进货的价格</param>
        /// <param name="amount">进货数量</param>
        /// <returns>入库成功，返回true，否则返回false</returns>
        public bool InStockNewProduct(string prod_name, int cate_id, int fact_id,
            string prod_spec, int brand_id, decimal inprice, int amount, string unit) {

            /* 在增加新产品的时候，先不增加库存数量，设置该库存为0，后面入库操作再填写库存数量 */
            int new_product_id = prod_da.AddNewProduct(prod_name, cate_id, fact_id,
                prod_spec, brand_id, 0, unit);

            //更新缓存
            if (new_product_id != -1) {
                prod_da.InStockProduct(new_product_id, inprice, amount);
                Product product = new Product();
                product.ProductID = new_product_id;
                product.Name = prod_name;
                product.OwnedCategory = CategoryManager.Instance.GetCategoryById(cate_id);
                product.VendorID = fact_id;
                product.Spec = prod_spec;
                product.BrandID = brand_id;
                product.StockCount = amount;
                product.Unit = unit;
                product.IsSaling = false;
                AllProduct.Add(product);
                return true;
            }

            return false;
        }

        /// <summary>入库一个库存中已经有的商品
        /// </summary>
        /// <param name="prod_id">要入库的商品ID</param>
        /// <param name="inprice">入库的价格</param>
        /// <param name="amount">入库的数量</param>
        /// <param name="vendor_id">该商品的供应商ID</param>
        /// <returns></returns>
        public bool InStockProduct(int prod_id, decimal inprice, int amount) {
            if (prod_da.InStockProduct(prod_id, inprice, amount)) {
                Product product = AllProduct.First(p => p.ProductID == prod_id);
                if (product == null) {
                    product = prod_da.GetProductByID(prod_id);
                    AllProduct.Add(product);
                }

                product.StockCount += amount;
                return true;
            }

            return false;
        }

        /// <summary>对库存的商品保留某些数量
        /// </summary>
        /// <param name="pi">要保留的商品项</param>
        /// <returns>是否保留成功，如果该产品的库存数量大于amount，应该要能保留成功。成功返回true，否则返回false</returns>
        /// <remarks>该接口在用户下订单的时候使用</remarks>
        public bool ReserveProduct(ProductItem pi) {
            if (prod_da.ReserveProduct(pi.Product.ProductID, pi.Amount)) {
                pi.Product.ReserceCount += pi.Amount;
                return true;
            }

            return false;
        }

        /// <summary>对库存的商品保留某些数量
        /// </summary>
        /// <param name="items">要保留的商品列表</param>
        /// <returns>是否保留成功，如果该产品的库存数量大于amount，应该要能保留成功。成功返回true，否则返回false</returns>
        /// <remarks>该接口在用户下订单的时候使用</remarks>
        public bool ReserveProducts(IEnumerable<ProductItem> items) {
            if (prod_da.ReserveProducts(items)) {
                foreach (ProductItem pi in items) {
                    pi.Product.ReserceCount += pi.Amount;
                }
                return true;
            }

            return false;
        }

        /// <summary>对库存的商品去除保留商品数量
        /// </summary>
        /// <param name="items">要去除的商品保留数量列表</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool UnReserveProducts(IEnumerable<ProductItem> items) {
            if (prod_da.UnReserveProducts(items)) {
                foreach (ProductItem pi in items) {
                    pi.Product.ReserceCount -= pi.Amount;
                }
                return true;
            }

            return false;
        }

        /// <summary>对商品进行出货处理
        /// </summary>
        /// <param name="items">要出货的商品列表</param>
        /// <returns>出货成功返回true，否则返回false</returns>
        public bool ShippingProducts(IEnumerable<ProductItem> items) {
            if (items.Count() <= 0)
                return false;

            //成功出货要更新缓存
            if (prod_da.ShippingProducts(items)) {
                foreach (ProductItem pi in items) {
                    pi.Product.StockCount -= pi.Amount;
                    //保留的商品项可以去除（已经在库存中减去该部分的商品的数量）
                    pi.Product.ReserceCount -= pi.Amount;
                }
                
                return true;
            }

            return false;
        }
        #endregion

        #region 商品的评论处理
        /// <summary>对一个商品进行评论
        /// </summary>
        /// <param name="user">进行评论的用户</param>
        /// <param name="product">要评论的商品对象</param>
        /// <param name="level">评价的星级</param>
        /// <param name="content">评论的内容</param>
        /// <returns>是否评论成功，成功返回true，否则返回false</returns>
        public bool CommentProduct(User user, Product product, EvaluteGrade level, string content) {
            return prod_da.CommentProduct(user.UserID, product.ProductID, level, content);
        }

        /// <summary>获取某个产品的评论的数量
        /// </summary>
        /// <param name="prod">要获取评论的产品</param>
        /// <returns>评论的数量</returns>
        public int GetProductCommentCount(Product prod, out int grade) {
            int total_grade = 0;
            int total_comment = prod_da.GetProductCommentCountAndGrade(prod.ProductID, out total_grade);
            if (total_comment > 0) {
                grade = (total_comment / 2 + total_grade) / total_comment;
            }
            else
                grade = 5;

            return total_comment;
        }

        /// <summary>获取产品的评论列表
        /// </summary>
        /// <param name="prod">要获取评论的产品对象</param>
        /// <param name="page_index">获取评论的页索引，从0开始</param>
        /// <param name="page_size">获取的每页评论的数量</param>
        /// <returns>获取的评论的列表, 按时间倒序排列</returns>
        public IList<ProductComment> GetProductComment(Product prod, int page_index, int page_size) {
            List<TmpProductComment> comment_list = prod_da.GetProductComment(prod.ProductID, page_index, page_size);
            List<ProductComment> comments = new List<ProductComment>();
            foreach (TmpProductComment tpc in comment_list) {
                ProductComment pc = new ProductComment();
                pc.Product = prod;
                pc.User = UserManager.Instance.GetUserByID(tpc.UserID);
                pc.Grade = tpc.Grade;
                pc.Content = tpc.Content;
                pc.CommentTime = tpc.CommentTime;
                comments.Add(pc);
            }

            return comments;
        }
        #endregion

        /// <summary>获取某个类别的产品的列表
        /// </summary>
        /// <param name="cate">要获取的产品的类别</param>
        /// <param name="page_index">要获取的产品的页索引</param>
        /// <param name="page_size">每页的产品数量</param>
        /// <returns>获取到的产品的列表</returns>
        public IList<Product> GetPagedProductsByCategory(Category cate, int page_index, int page_size, out int total_page_count) {
            List<Product> cate_products = SaleProductCache.FindAll(p => p.OwnedCategory == cate);
            int item_count = cate_products.Count;
            total_page_count = (item_count + page_size - 1) / page_size;

            int begin_index = page_index * page_size;
            int count = page_size;

            if (begin_index > item_count) {
                begin_index = 0;
            }

            if (begin_index + page_size > item_count)
                count = item_count - begin_index;

            return cate_products.GetRange(begin_index, count);
        }

        /// <summary>修改商品的信息
        /// </summary>
        /// <param name="product">新的商品对象</param>
        /// <returns>是否修改成功</returns>
        public bool ModifyProductInfo(Product product) {
            if (prod_da.ModifyProductInfo(product)) {
                return true;
            }

            return false;
        }
    }
}
