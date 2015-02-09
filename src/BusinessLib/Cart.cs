using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.DataProvider;
using Solemart.DataProvider.Entity;

namespace Solemart.BusinessLib {
    /// <summary>
    /// The shopping cart object
    /// </summary>
    public class Cart {
        /// <summary>
        /// the product list in the shopping cart.
        /// </summary>
        private List<CartItem> cartItems { get; set; }

        public Cart() {
            cartItems = new List<CartItem>();
        }

        /// <summary>
        /// Add a product to shopping cart
        /// </summary>
        /// <param name="cartItem">The shopping cart item.</param>
        public void AddToCart(CartItem cartItem) {
            CartItem addedCartItem = cartItems.Find(p => p.ProductID == cartItem.ProductID);
            if (addedCartItem != null)
                addedCartItem.Amount += cartItem.Amount;
            else {
                //, cartItem.SalePrice * cartItem.Discount / 100, count);
                cartItems.Add(cartItem);
            }
        }

        /// <summary>
        /// Add a product to shopping cart
        /// </summary>
        /// <param name="productID">The product want to add to shopping cart</param>
        /// <param name="count">该商品的数量</param>
        public void AddToCart(int productID, int count) {
            CartItem cartItem = cartItems.Find(p => p.ProductID == productID);
            if (cartItem != null) {
                cartItem.Amount += count;
            }
            else {
                SaledProductItem product=ProductManager.Instance.GetProductByID(productID);
                if (product == null)
                    throw new ArgumentException(string.Format("该ID={0}的商品不存在", productID));
                CartItem pi = new CartItem { ProductID = product.ProductID, Amount = count, UnitPrice = product.Price * product.Discount };
                cartItems.Add(pi);
            }
        }

        /// <summary>清除购物车中的所有商品项
        /// </summary>
        public void Clear() {
            cartItems.Clear();
        }

        /// <summary>清除购物车中的所有商品并保存入库
        /// <param name="user">要清除的购物车的用户ID</param>
        /// </summary>
        /// <returns>执行成功返回true，否则返回false</returns>
        public bool ClearAndSave(UserItem user) {
            if (oda.ClearCartProductList(user.UserID)) {
                Clear();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete the product in the shopping cart
        /// </summary>
        /// <param name="product">要删除的商品</param>
        public void RemoveProduct(ProductItem product) {
            RemoveProduct(product.ProductID);
        }

        /// <summary>删除购物车中的某件商品
        /// </summary>
        /// <param name="productID">要删除的商品</param>
        public void RemoveProduct(int productID) {
            CartItem cartItem = cartItems.Find(p => p.ProductID == productID);
            if (cartItem != null)
                Products.Remove(cartItem);
        }

        /// <summary>返回购物车的商品总价
        /// </summary>
        public decimal TotalPrice{
            get {
                decimal price=0.0m;
                foreach (CartItem cartItem in cartItems)
                    price += cartItem.UnitPrice * cartItem.Amount;

                return price;
            }
        }

        /// <summary>
        /// Modify the product count in the shopping cart
        /// </summary>
        /// <param name="productID">要修改的购物车中的商品项的商品ID</param>
        /// <param name="amount">商品的最后数量</param>
        /// <returns>修改成功返回true，否则返回false</returns>
        public bool ModifyCartItem(int productID, int amount) {
            CartItem cartItem = cartItems.Find(p => p.ProductID == productID);
            if (cartItem == null)
                return false;

            if (amount <= 0)
                cartItems.Remove(cartItem);
            else
                cartItem.Amount = amount;

            return true;
        }

        /// <summary>获取某个用户的购物车
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>该用户的购物车</returns>
        /// <remarks>该方法用于用户登录时，获取前次登录的购物车对象，表示为未完成的购物行为</remarks>
        public static Cart GetUserCart(UserItem user) {
            List<TmpProductItem> items = oda.GetLastCart(user.UserID);

            Cart cart = new Cart();
            List<ProductItem> prod_items = new List<ProductItem>();
            foreach (TmpProductItem tpi in items) {
                Product prod = ProductManager.Instance.GetProductByID(tpi.ProductID);
                prod_items.Add(new ProductItem(prod, prod.SalePrice * prod.Discount / 100, tpi.Amount));
            }
            cart.cartItems = prod_items;
            return cart;
        }

        /// <summary>保存购物车
        /// </summary>
        /// <param name="user">要保存购物车的用户</param>
        /// <returns>是否成功保存</returns>
        public bool Save(UserItem user) {
            IEnumerable<TmpProductItem> prod_list = from p in cartItems 
                                             select new TmpProductItem() { OrderID=0, ProductID=p.Product.ProductID, Amount=p.Amount, UnitPrice=0.0m };
            return oda.SaveLastCart(user.UserID, prod_list);
        }
    }
}
