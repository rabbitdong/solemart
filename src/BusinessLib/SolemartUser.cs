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
        /// 内部用户表示匿名用户的UserID (用1表示匿名用户ID)
        /// </summary>
        public static int DefaultAnonymousUserID = 1;

        private SendAddressItem sendAddress;

        private UserItem userItem;
        private Cart cart;
        private bool isAnonymous;

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

                //如果useritem为空，或者非匿名用户，说明不是注册用户，就返回一个匿名用户（UserID代分配）
                if (useritem != null && string.Compare(useritem.Roles, Role.Anonymous.RoleID.ToString()) != 0)
                {
                    this.userItem = useritem;
                    //不管是不是匿名用户，都用购物车
                    cart = GetUserCart();
                    isAnonymous = false;
                }
                else
                {
                    string anonymousUserName = string.Format("Anonymous_{0}", anonymousUserID);
                    this.userItem = new UserItem { UserID = anonymousUserID, Roles = Role.Anonymous.RoleID.ToString(), UserName = anonymousUserName };
                    cart = new Cart(this);
                    isAnonymous = true;
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
            isAnonymous = user.Roles.Contains(Role.Anonymous.RoleID.ToString());
        }

        public string UserName
        {
            get { return userItem.UserName; }
        }

        public int UserID
        {
            get { return userItem.UserID; }
        }

        /// <summary>
        /// Indicate is anonymous or not.
        /// </summary>
        public bool IsAnonymous
        {
            get { return isAnonymous; }
        }

        public bool IsLoginQQ
        {
            get { return userItem.LoginType == LoginType.QQ; }
        }

        public string Email
        {
            get { return userItem.Email; }
        }

        public string Address
        {
            get
            {
                if (userItem.AppendInfo == null)
                {
                    LoadAppendInfo();
                }

                return "";
            }
        }

        public string Phone
        {
            get
            {
                if (userItem.AppendInfo == null)
                {
                    LoadAppendInfo();
                }

                return "";
            }
        }

        public DateTime BirthDay
        {
            get
            {
                if (userItem.AppendInfo == null)
                {
                    LoadAppendInfo();
                }
                return DateTime.Now;
            }
        }

        public Sex Sex
        {
            get
            {
                if (userItem.AppendInfo == null)
                {
                    LoadAppendInfo();
                }

                return SystemUtil.Sex.Unknown;
            }
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
                Cart cart = new Cart(this);
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

        /// <summary>
        /// Get or set the user send address information.
        /// </summary>
        public SendAddressItem SendAddressInfo
        {
            get
            {
                //如果是非匿名用户，从用户注册信息中获取
                if (sendAddress == null && !IsAnonymous)
                {
                    sendAddress = UserManager.GetSendAddressInfo(UserID);
                    //如果发货地址还是为空，就使用用户的附加信息字段中的信息
                    if (sendAddress == null)
                    {
                        UserAppendInfoItem uai = UserManager.GetUserAppendInfo(UserID);
                        if (uai != null && string.IsNullOrEmpty(uai.Address))
                            sendAddress.Address = uai.Address;
                        if (uai != null && string.IsNullOrEmpty(uai.Phone))
                            sendAddress.Phone = uai.Phone;
                    }
                }

                if (sendAddress == null)
                {
                    sendAddress = new SendAddressItem();
                }
                //最终都返回送货地址信息
                return sendAddress;
            }
            set
            {
                //如果不是匿名用户，需要保存到数据库中
                if (!IsAnonymous)
                    UserManager.SaveSendAddressInfoForUser(value);

                //都赋值给内存的用户对象
                sendAddress = value;
            }
        }

        private void LoadAppendInfo()
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserAppendInfoItem appendInfo = context.UserAppendInfoItems.Find(UserID);
                if (appendInfo != null)
                {
                    userItem.AppendInfo = appendInfo;
                }
            }
        }
    }
}
