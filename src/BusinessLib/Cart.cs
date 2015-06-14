using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.DataProvider;
using Solemart.DataProvider.Entity;
using SimLogLib;

namespace Solemart.BusinessLib
{
    /// <summary>
    /// The shopping cart object
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// The cart user.
        /// </summary>
        public SolemartUser User { get; set; }

        /// <summary>
        /// the product list in the shopping cart.
        /// </summary>
        private List<CartItem> cartItems { get; set; }

        /// <summary>
        /// The user own the cart.
        /// </summary>
        /// <param name="user"></param>
        public Cart(SolemartUser user)
        {
            this.User = user;
            cartItems = new List<CartItem>();
        }

        /// <summary>
        /// Add a product to shopping cart
        /// </summary>
        /// <param name="cartItem">The shopping cart item.</param>
        public void AddToCart(CartItem cartItem)
        {
            CartItem addedCartItem = cartItems.Find(p => p.ProductID == cartItem.ProductID);
            if (addedCartItem != null)
                addedCartItem.Amount += cartItem.Amount;
            else
            {
                cartItems.Add(cartItem);
            }
        }

        public List<CartItem> CartItems
        {
            get { return cartItems; }
            set { cartItems = value; }
        }

        /// <summary>
        /// Add a product to shopping cart
        /// </summary>
        /// <param name="productID">The product want to add to shopping cart</param>
        /// <param name="count">该商品的数量</param>
        public void AddToCart(ProductItem product, int count)
        {
            CartItem cartItem = cartItems.Find(p => p.ProductID == product.ProductID);
            if (cartItem != null)
            {
                cartItem.Amount += count;
            }
            else
            {
                SaledProductItem saledProduct = ProductManager.GetSaledProductByID(product.ProductID);
                if (product == null)
                    throw new ArgumentException(string.Format("该ID={0}的商品不存在", product.ProductID));
                CartItem pi = new CartItem { ProductID = product.ProductID, Product = product, Amount = count, UnitPrice = saledProduct.Price * saledProduct.Discount /100 };
                cartItems.Add(pi);
            }
        }

        /// <summary>
        /// Clear all the cart items.
        /// </summary>
        private void Clear()
        {
            cartItems.Clear();
            Log.Instance.WriteLog(string.Format("Clear the all cart items for user[{0}]", User.UserID));
        }

        /// <summary>清除购物车中的所有商品并保存入库
        /// <param name="userID">要清除的购物车的用户ID</param>
        /// </summary>
        /// <returns>执行成功返回true，否则返回false</returns>
        public bool ClearAndSave(int userID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                ///可能临时库中没有产品，会返回false
                context.ClearCartForUser(userID);
                Clear();
                return true;
            }
        }

        /// <summary>
        /// Delete the product in the shopping cart
        /// </summary>
        /// <param name="product">要删除的商品</param>
        public void RemoveProduct(ProductItem product)
        {
            RemoveProduct(product.ProductID);
        }

        /// <summary>删除购物车中的某件商品
        /// </summary>
        /// <param name="productID">要删除的商品</param>
        public void RemoveProduct(int productID)
        {
            CartItem cartItem = cartItems.Find(p => p.ProductID == productID);
            if (cartItem != null)
                cartItems.Remove(cartItem);
        }

        /// <summary>返回购物车的商品总价
        /// </summary>
        public decimal TotalPrice
        {
            get
            {
                decimal price = 0.0m;
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
        public bool ModifyCartItem(int productID, decimal amount)
        {
            CartItem cartItem = cartItems.Find(p => p.ProductID == productID);
            if (cartItem == null)
                return false;

            if ((cartItem.Product.Unit == "个" || cartItem.Product.Unit == "粒") && decimal.Round(amount) != amount)
                return false;

            if (amount <= 0)
                cartItems.Remove(cartItem);
            else
                cartItem.Amount = amount;

            return true;
        }
    }
}
