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
        private UserItem userItem;
        private Cart cart;

        public SolemartUser() { }

        /// <summary>
        /// Create the user object by the userid.
        /// </summary>
        /// <param name="userid"></param>
        public SolemartUser(int userid) 
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem useritem = context.UserItems.Find(userid);
                if (useritem == null)
                {
                    throw new KeyNotFoundException("The user of the userid doesn't exist!");
                }

                this.userItem = useritem;
                cart = GetUserCart();
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

        /// <summary>
        /// The system anonymous user.
        /// </summary>
        public static SolemartUser Anonymous = new SolemartUser { userItem = new UserItem { UserName = "Anonymous", UserID = 0, Roles = Role.Anonymous.RoleName } };

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

    /// <summary>
    /// 系统用户缓存类
    /// </summary>
    public class SolemartUserCache
    {
        private static List<SolemartUser> _userCache = new List<SolemartUser>();

        /// <summary>
        /// Get the user object from the cache.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static SolemartUser GetUser(int userid)
        {
            SolemartUser user = _userCache.Find(u => u.UserID == userid);
            if (user == null)
            {
                user = new SolemartUser(userid);
                _userCache.Add(user);
            }

            return user;
        }

        /// <summary>
        /// Drop the user object in cache by manual.
        /// </summary>
        /// <param name="userid"></param>
        public static void DropUserInCache(int userid)
        {
            SolemartUser user = _userCache.Find(u => u.UserID == userid);
            if (user != null)
                _userCache.Remove(user);
        }
    }
}
