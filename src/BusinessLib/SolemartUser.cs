using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.DataProvider.Entity;
using System.Security.Principal;
using Solemart.DataProvider;
using Solemart.SystemUtil;

namespace Solemart.BusinessLib
{
    /// <summary>
    /// 系统的用户对象
    /// </summary>
    public class SolemartUser : IPrincipal
    {
        /// <summary>
        /// 内部用户表示匿名用户的UserID
        /// </summary>
        public static int AnonymousRoleID = 0;

        private UserItem userItem;
        private Cart cart;

        public SolemartUser() { }

        /// <summary>
        /// Create the user object by the userid.
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="anonymousUserID">if the user id is not exist, and create user with this id</param>
        public SolemartUser(int userid, int anonymousUserID) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem useritem = context.UserItems.Find(userid);
                //如果useritem为空，说明不是注册用户，就返回一个匿名用户（UserID代分配）
                if (useritem != null)
                {
                    this.userItem = useritem;
                    //不管是不是匿名用户，都用购物车
                    cart = GetUserCart();
                }
                else
                {
                    string anonymousUserName = string.Format("Anonymous_{0}", anonymousUserID);
                    this.userItem = new UserItem { UserID = anonymousUserID, Roles = Role.Anonymous.RoleID.ToString(), UserName = anonymousUserName };
                    cart = new Cart();
                }
            }
        }

        /// <summary>
        /// Create the user object by the useritem data.
        /// </summary>
        /// <param name="user"></param>
        public SolemartUser(UserItem user)
        {
            this.userItem = user;
            cart = GetUserCart();
        }

        public string UserName
        {
            get { return userItem.UserName; }
        }

        public int UserID
        {
            get { return userItem.UserID; }
        }

        public bool IsLoginQQ
        {
            get { return userItem.LoginType == LoginType.QQ; }
        }

        public string[] RoleNames
        {
            get { return Role.GetRoleNames(Role.GetRoles(userItem.Roles)); }
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
            return Identity != null && Identity.IsAuthenticated &&
                !string.IsNullOrWhiteSpace(role) && (RoleNames.Contains(role));
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
