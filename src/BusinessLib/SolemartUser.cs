using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.DataProvider.Entity;
using System.Security.Principal;
using Solemart.DataProvider;

namespace Solemart.BusinessLib
{
    public class SolemartUser : IPrincipal
    {
        private UserItem userItem;
        private Cart cart;

        public SolemartUser() { }

        public SolemartUser(UserItem user)
        {
            this.userItem = user;
            cart = GetUserCart();
        }

        /// <summary>
        /// The system anonymous user.
        /// </summary>
        public static SolemartUser Anonymous = new SolemartUser { userItem = new UserItem { UserName = "Anonymous", UserID = 0 } };

        public string UserName
        {
            get { return userItem.UserName; }
        }

        public int UserID
        {
            get { return userItem.UserID; }
        }

        /// <summary>
        /// Get the user identity
        /// </summary>
        public IIdentity Identity
        {
            get 
            {
                return new GenericIdentity(userItem.UserName);
            }
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the cart of the user
        /// </summary>
        /// <returns>该用户的购物车</returns>
        /// <remarks>该方法用于用户登录时，获取前次登录的购物车对象，表示为未完成的购物行为</remarks>
        private Cart GetUserCart()
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                Cart cart = new Cart();
                cart.CartItems = context.CartItems.Include("Product").Where(c => c.UserID == UserID).ToList();
                return cart;
            }
        }

        public void SaveCart()
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return;
            }
        }

        /// <summary>
        /// Get the user's cart
        /// </summary>
        public Cart Cart
        {
            get { return cart; }
        }
    }
}
