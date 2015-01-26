using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xxx.EntityLib;
using Xxx.DataAccessLib;

namespace Xxx.BusinessLib {
    /// <summary>表示购物车对象，它是一个会话对象
    /// </summary>
    public class Cart {
        /// <summary>保存购物车中商品项列表
        /// </summary>
        private List<ProductItem> products { get; set; }

        private static OrderDA oda = OrderDA.Instance;

        public Cart() {
            products = new List<ProductItem>();
        }

        /// <summary>添加一个商品项到购物车中
        /// </summary>
        /// <param name="product">该商品对象</param>
        /// <param name="count">该商品的数量</param>
        public void AddToCart(Product product, int count) {
            ProductItem proditem = products.Find(p => p.Product.ProductID == product.ProductID);
            if (proditem != null)
                proditem.Amount += count;
            else {
                ProductItem pi = new ProductItem(product, product.SalePrice * product.Discount / 100, count);
                products.Add(pi);
            }
        }

        /// <summary>添加一个商品项到购物车中
        /// </summary>
        /// <param name="pid">该商品项的ID</param>
        /// <param name="count">该商品的数量</param>
        public void AddToCart(int pid, int count) {
            ProductItem proditem = products.Find(p => p.Product.ProductID == pid);
            if (proditem != null) {
                proditem.Amount += count;
            }
            else {
                Product product=ProductManager.Instance.GetProductByID(pid);
                if (product == null)
                    throw new ArgumentException(string.Format("该ID={0}的商品不存在", pid));
                ProductItem pi = new ProductItem(product, product.SalePrice * product.Discount / 100, count);
                products.Add(pi);
            }
        }

        /// <summary>清除购物车中的所有商品项
        /// </summary>
        public void Clear() {
            products.Clear();
        }

        /// <summary>清除购物车中的所有商品并保存入库
        /// <param name="user">要清除的购物车的用户ID</param>
        /// </summary>
        /// <returns>执行成功返回true，否则返回false</returns>
        public bool ClearAndSave(User user) {
            if (oda.ClearCartProductList(user.UserID)) {
                Clear();
                return true;
            }

            return false;
        }

        /// <summary>删除购物车中某件商品
        /// </summary>
        /// <param name="product">要删除的商品</param>
        public void ExcludeProduct(Product product) {
            ExcludeProduct(product.ProductID);
        }

        /// <summary>删除购物车中的某件商品
        /// </summary>
        /// <param name="pid">要删除的商品</param>
        public void ExcludeProduct(int pid) {
            ProductItem proditem = products.Find(p => p.Product.ProductID == pid);
            if (proditem != null)
                Products.Remove(proditem);
        }

        /// <summary>返回购物车的商品总价
        /// </summary>
        public decimal TotalPrice{
            get {
                decimal price=0.0m;
                foreach (ProductItem pi in products)
                    price += pi.Product.SalePrice * pi.Product.Discount / 100 * pi.Amount;

                return price;
            }
        }

        /// <summary>获取购物车中的商品列表
        /// </summary>
        public List<ProductItem> Products {
            get { return products; }
        }

        /// <summary>修改购物车中商品项的数量
        /// </summary>
        /// <param name="pid">要修改的购物车中的商品项的商品ID</param>
        /// <param name="amount">商品的最后数量</param>
        /// <returns>修改成功返回true，否则返回false</returns>
        public bool ModifyCartProductItem(int pid, int amount) {
            ProductItem pi = products.Find(p => p.Product.ProductID == pid);
            if (pi == null)
                return false;

            if (amount <= 0)
                products.Remove(pi);
            else
                pi.Amount = amount;

            return true;
        }

        /// <summary>获取某个用户的购物车
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>该用户的购物车</returns>
        /// <remarks>该方法用于用户登录时，获取前次登录的购物车对象，表示为未完成的购物行为</remarks>
        public static Cart GetUserCart(User user) {
            List<TmpProductItem> items = oda.GetLastCart(user.UserID);

            Cart cart = new Cart();
            List<ProductItem> prod_items = new List<ProductItem>();
            foreach (TmpProductItem tpi in items) {
                Product prod = ProductManager.Instance.GetProductByID(tpi.ProductID);
                prod_items.Add(new ProductItem(prod, prod.SalePrice * prod.Discount / 100, tpi.Amount));
            }
            cart.products = prod_items;
            return cart;
        }

        /// <summary>保存购物车
        /// </summary>
        /// <param name="user">要保存购物车的用户</param>
        /// <returns>是否成功保存</returns>
        public bool Save(User user) {
            IEnumerable<TmpProductItem> prod_list = from p in products 
                                             select new TmpProductItem() { OrderID=0, ProductID=p.Product.ProductID, Amount=p.Amount, UnitPrice=0.0m };
            return oda.SaveLastCart(user.UserID, prod_list);
        }
    }
}
