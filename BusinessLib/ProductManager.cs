using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xxx.EntityLib;
using Xxx.DataAccessLib;
using Xxx.SystemUtil;

namespace Xxx.BusinessLib {
    /// <summary>货品管理类
    /// </summary>
    public class ProductManager {
        /// <summary>默认的返回每一页的产品的数量
        /// </summary>
        private const int DEFAULT_EACH_PAGE_COUNT = 10;

        /// <summary>默认的销售页面的每一页销售产品的数量
        /// </summary>
        private const int DEFAULT_SALE_EACH_PAGE_COUNT = 12;

        private static ProductManager instance = new ProductManager();

        /// <summary>所有商品的缓存
        /// </summary>
        private List<Product> products_cache = null;

        /// <summary>最受欢迎的产品的缓存，用于首页显示
        /// </summary>
        private List<Product> most_popular_products_cache = new List<Product>();

        /// <summary>销售的商品的缓存
        /// </summary>
        private List<Product> sale_product_cache = null;

        private int each_page_count = DEFAULT_EACH_PAGE_COUNT;
        private int each_sale_page_count = DEFAULT_SALE_EACH_PAGE_COUNT;

        private ProductDA prod_da = ProductDA.Instance;

        /// <summary>私有构造函数，单件
        /// </summary>
        private ProductManager(){
            InitProductCache();
            InitSaledProductCache();
        }

        /// <summary>初始化商品信息列表
        /// </summary>
        private void InitProductCache() {
            products_cache = prod_da.GetAllProduct();
            foreach (Product product in products_cache) {
                product.OwnedCategory = CategoryManager.Instance.GetCateById(product.OwnedCategory.CateID);
                product.Images = prod_da.GetProductImage(product.ProductID);
            }
        }

        /// <summary>获取最流行的产品列表(近一个月销售的产品)
        /// </summary>
        /// <returns></returns>
        public List<Product> GetMostPopularProducts() {
            if (most_popular_products_cache.Count > 0)
                return most_popular_products_cache;

            OrderDA oda=OrderDA.Instance;
            List<KeyValuePair<int, int>> sale_counts = oda.GetMostPopularProducts(DateTime.Now.AddMonths(-1));

            most_popular_products_cache.Clear();
            foreach (KeyValuePair<int, int> item in sale_counts) {
                most_popular_products_cache.Add(AllProduct.Find(p => p.ProductID == item.Key));
            }

            foreach (Product prod in SaleProductCache) {
                if (!most_popular_products_cache.Contains(prod))
                    most_popular_products_cache.Add(prod);
            }

            return most_popular_products_cache;
        }

        /// <summary>刷新最流行商品的列表
        /// </summary>
        public void RefleshMostPopularProducts() {
            most_popular_products_cache.Clear();
        }

        /// <summary>初始化在售的商品列表
        /// </summary>
        private void InitSaledProductCache() {
            List<TempSaleProduct> tmp_saleproduct_list = prod_da.GetAllSaleProductList();
            sale_product_cache = new List<Product>();

            foreach (TempSaleProduct tsp in tmp_saleproduct_list) {
                //先在缓存全部商品表中查找是否有该商品
                Product product = AllProduct.Find(p => p.ProductID == tsp.ProductID);
                if (product != null) {
                    sale_product_cache.Add(product);
                }
                else {      //如果没有，访问数据库是否有
                    product = prod_da.GetProductByID(tsp.ProductID);
                    if (product != null) {
                        AllProduct.Add(product);
                    }
                    else {
                        LogManager.Instance.Error(String.Format("商品销售的[pid:{0}]和库存信息不一致，系统有问题！", tsp.ProductID));
                    }
                }

                if (product != null) {
                    product.IsSaling = true;
                    product.SalePrice = tsp.Price;
                    product.IsSpec = tsp.IsSpecFlag;
                    product.Discount = tsp.Discount;
                }
            }
        }

        /// <summary>获取货品管理对象
        /// </summary>
        public static ProductManager Instance {
            get { return instance; }
        }

        /// <summary>在售商品列表，按照该商品的销售受欢迎程度排序
        /// </summary>
        private List<Product> SaleProductCache {
            get {
                if (sale_product_cache == null) {
                    InitSaledProductCache();
                }

                return sale_product_cache;
            }
        }

        /// <summary>获取所有的商品列表
        /// </summary>
        private List<Product> AllProduct {
            get {
                if (products_cache == null) {
                    InitProductCache();
                }

                return products_cache;
            }
        }

        /// <summary>获取所有的销售商品列表
        /// </summary>
        public List<Product> SaleProducts {
            get { return sale_product_cache; }
        }

        /// <summary>获取目前中的货品样数
        /// </summary>
        public int TotalProductCount {
            get {
                return AllProduct.Count;
            }
        }

        /// <summary>获取目前销售的商品的数量
        /// </summary>
        public int SaleProductCount {
            get { return sale_product_cache.Count; }
        }

        /// <summary>获取或设置产品每一页的数量
        /// </summary>
        public int EachPageCount {
            get { return each_page_count; }
            set { each_page_count = value; }
        }

        /// <summary>获取或设置销售产品的每一页的数量
        /// </summary>
        public int EachSalePageCount{
            get{ return each_sale_page_count;}
            set{ each_sale_page_count = value;}
        }

        /// <summary>获取产品的分页页数(总产品的页数)
        /// </summary>
        public int ProductPagedCount {
            get { return (TotalProductCount + EachPageCount - 1) / EachPageCount; }
        }

        /// <summary>获取销售产品的分页页数（已经上架的产品）
        /// </summary>
        public int SaleProductPagedCount {
            get {
                int sale_product_count = sale_product_cache.Count;
                return (sale_product_count + EachSalePageCount - 1) / EachSalePageCount;
            }
        }

        /// <summary>获取第几页的产品列表
        /// </summary>
        /// <param name="page_index">页索引，从0开始</param>
        /// <returns>返回产品的列表</returns>
        public List<Product> GetPagedProduct(int page_index) {
            int begin_index = page_index * EachPageCount;
            int count = EachPageCount;

            if (begin_index > TotalProductCount) {
                begin_index = 0;
            }

            if (begin_index + EachPageCount > TotalProductCount)
                count = TotalProductCount - begin_index;

            return AllProduct.GetRange(begin_index, count);
        }

        /// <summary>获取最新的产品列表
        /// </summary>
        /// <returns></returns>
        public List<Product> GetNewestSaleProducts() {
            return SaleProductCache.GetRange(0, SaleProductCache.Count > 9 ? 9 : SaleProductCache.Count);
        }

        /// <summary>获取分页的上架产品的列表
        /// </summary>
        /// <param name="page_index">要获取的产品的页索引，从0开始</param>
        /// <returns>获取到的产品列表</returns>
        public List<Product> GetSalePagedProducts(int page_index) {
            if (page_index < 0)
                page_index = 0;

            int begin_index = page_index * EachSalePageCount;
            int count = EachSalePageCount;

            if (begin_index > SaleProductCount) {
                begin_index = 0;
            }

            if (begin_index + EachSalePageCount > SaleProductCount)
                count = SaleProductCount - begin_index;

            return GetMostPopularProducts().GetRange(begin_index, count);
        }

        /// <summary>给定产品的ID列表，返回产品的对象列表
        /// </summary>
        /// <param name="ids">产品的ID列表</param>
        /// <returns>获取到的产品的列表</returns>
        internal List<Product> GetProductListByIds(int[] ids) {
            List<Product> results = new List<Product>();
            foreach(Product prod in sale_product_cache){
                for (int i = 0; i < ids.Length; ++i) {
                    if (prod.ProductID == ids[i])
                        results.Add(prod);
                }
            }

            return results;
        }

        /// <summary>根据PID获取某个商品的基本信息
        /// </summary>
        /// <param name="pid">要获取信息的产品PID</param>
        /// <returns>商品对象, 如果该ID的对象不存在，返回null</returns>
        public Product GetProductByID(int pid) {
            Product prod = AllProduct.Find(p => p.ProductID == pid);
            if (prod == null) {
                //先查看销售表中是否有该商品存在
                Product saled_prod = SaleProductCache.Find(p => p.ProductID == pid);
                if (saled_prod != null)
                    AllProduct.Add(saled_prod);
                else {  //没有只能从数据库中获取
                    prod = prod_da.GetProductByID(pid);
                    prod.Images = prod_da.GetProductImage(prod.ProductID);
                    if (prod == null)
                        LogManager.Instance.Error(string.Format("[pid:{0}]的商品不存在", pid));
                    else {
                        AllProduct.Add(prod);
                        prod.IsSaling = false;
                    }
                }
            }

            return prod;
        }

        #region 上架/下架处理
        /// <summary>对商品进行上架操作
        /// </summary>
        /// <param name="prod_id">要上架的商品ID</param>
        /// <param name="sale_price">销售价格</param>
        /// <param name="discount">商品的折扣</param>
        /// <param name="spec_price_flag">是否是特价的标志</param>
        /// <returns>如果执行成功，返回true，否则返回false</returns>
        public bool PutToSaling(int prod_id, decimal sale_price, int discount, bool spec_price_flag) {
            Product tmp_prod = AllProduct.Find(p => p.ProductID == prod_id);
            tmp_prod.SalePrice = sale_price;
            tmp_prod.IsSpec = spec_price_flag;
            tmp_prod.Discount = discount;
            tmp_prod.IsSaling = true;

            if (SaleProductCache.Contains(tmp_prod)) {
                prod_da.UpdateSaling(prod_id, sale_price, discount, spec_price_flag);
            }
            else{
                prod_da.PutToSaling(prod_id, sale_price, discount, spec_price_flag);
                SaleProductCache.Add(tmp_prod);
            }
            //清空缓存
            most_popular_products_cache.Clear();

            return true;
        }

        /// <summary>对商品prod_id进行下架处理
        /// </summary>
        /// <param name="prod_id">要下架的商品的ID</param>
        /// <returns>是否下架成功</returns>
        public bool GetBackSaling(int prod_id) {
            if (prod_da.GetBackSaling(prod_id)) {
                SaleProductCache.RemoveAll(p => p.ProductID == prod_id);
                Product prod = AllProduct.First(p => p.ProductID == prod_id);
                if (prod != null)
                    prod.IsSaling = false;

                return true;
            }
            //清空缓存
            most_popular_products_cache.Clear();

            return false;
        }

        /// <summary>获取商品的最后的入库价格
        /// </summary>
        /// <param name="product_id">要获取的商品的ID</param>
        /// <returns>商品的最后的入库价格</returns>
        public decimal GetLastStockPrice(int product_id) {
            return prod_da.GetLastStockPrice(product_id);
        }

        #endregion

        #region 商品的图片处理
        /// <summary>添加一个图像到产品
        /// </summary>
        /// <param name="pid">要添加的产品的PID</param>
        /// <param name="datas">图像数据</param>
        /// <returns>是否添加成功</returns>
        public bool AddNewImageToProduct(int pid, string mimetype, string file_name) {
            int iid = prod_da.AddNewProductImage(pid, mimetype, file_name);
            //添加成功后，刷新缓存
            if (iid > 0) {
                Product product = AllProduct.Find(p => p.ProductID == pid);
                product.Images = prod_da.GetProductImage(pid);
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
                product.OwnedCategory = CategoryManager.Instance.GetCateById(cate_id);
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
