using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.DataProvider.Entity;
using System.Security.Principal;
using Solemart.DataProvider;
using Solemart.SystemUtil;
using SimLogLib;

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

        /// <summary>
        /// 使用openID的用户，如微信用户等
        /// </summary>
        /// <param name="openID">The user's openid of third part</param>
        /// <param name="type">The login type</param>
        public SolemartUser(string openID, LoginType type)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem useritem = context.UserItems.FirstOrDefault(u=>(u.LoginType == type && u.OpenID == openID));

                //如果useritem为空，用户曾经登陆过，直接取数据库中的数据
                if (useritem != null)
                {
                    Log.Instance.WriteLog(string.Format("the user[openid:[{0}], type:[{1}]] has login before.", openID, type));
                    this.userItem = useritem;
                    //不管是不是匿名用户，都用购物车
                    cart = GetUserCart();
                }
                else
                {
                    Log.Instance.WriteLog(string.Format("the user[openid:[{0}], type:[{1}]] is first login.", openID, type));
                    this.userItem = new UserItem { OpenID = openID, LoginType = type, Roles = Role.NormalUser.RoleID.ToString(), RegTime=DateTime.Now };
                    context.UserItems.Add(this.userItem);
                    context.UserAppendInfoItems.Add(new UserAppendInfoItem { UserID = this.userItem.UserID, BirthDay = new DateTime(1970, 1, 1), Address = "", Phone = "", Sex = SystemUtil.Sex.Unknown });
                    cart = new Cart(this);
                    //写入数据库
                    if (context.SaveChanges() <= 0)
                    {
                        Log.Instance.WriteWarn(string.Format("SolemartUser save failed. userid[{0}]", this.userItem.UserID));
                    }
                }
                isAnonymous = false;
            }
        }

        public string UserName
        {
            get { return userItem.UserName; }
        }

        /// <summary>
        /// Set the username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool SetUserName(string username)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem tempUserItem = context.UserItems.Find(userItem.UserID);
                if(tempUserItem != null)
                    tempUserItem.UserName = username;

                if (context.SaveChanges() > 0)
                {
                    userItem.UserName = username;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Set the username
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool SetEmail(string email)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem tempUserItem = context.UserItems.Find(userItem.UserID);
                if (tempUserItem != null)
                    tempUserItem.Email = email;

                if (context.SaveChanges() > 0)
                {
                    userItem.Email = email;
                    return true;
                }

                return false;
            }
        }

        public bool SetBirthDay(DateTime birthDay)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserAppendInfoItem tempUserItem = context.UserAppendInfoItems.FirstOrDefault(u=>u.UserID == userItem.UserID);
                if (tempUserItem != null)
                    tempUserItem.BirthDay = birthDay;

                if (context.SaveChanges() > 0)
                {
                    userItem.AppendInfo.BirthDay = birthDay;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Set the user head Image
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <returns></returns>
        public bool SetHeadImage(string imgUrl)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserAppendInfoItem tempUserItem = context.UserAppendInfoItems.FirstOrDefault(u => u.UserID == userItem.UserID);
                if (tempUserItem != null)
                    tempUserItem.HeadImageUrl = imgUrl;

                if (context.SaveChanges() > 0)
                {
                    userItem.AppendInfo.HeadImageUrl = imgUrl;
                    return true;
                }

                return false;
            }
        }


        /// <summary>
        /// Change the user's password
        /// </summary>
        /// <param name="newPassword">the new password</param>
        /// <param name="oldPassword">the old password</param>
        /// <returns></returns>
        public bool ChangePassword(string newPassword, string oldPassword)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                if (context.ValidateUserPassword(userItem.UserName, oldPassword))
                {
                    return context.UpdateUserPassword(userItem.UserID, newPassword);
                }
            }
            return true;
        }

        /// <summary>
        /// Set the user's new address
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public bool SetAddress(string address)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserAppendInfoItem tempUserItem = context.UserAppendInfoItems.FirstOrDefault(u => u.UserID == userItem.UserID);
                if (tempUserItem != null)
                    tempUserItem.Address = address;

                if (context.SaveChanges() > 0)
                {
                    if(userItem.AppendInfo != null)
                        userItem.AppendInfo.Address = address;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Set the user's phone
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public bool SetPhone(string phone)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserAppendInfoItem tempUserItem = context.UserAppendInfoItems.FirstOrDefault(u => u.UserID == userItem.UserID);
                if (tempUserItem != null)
                    tempUserItem.Phone = phone;

                if (context.SaveChanges() > 0)
                {
                    if(userItem.AppendInfo != null)
                        userItem.AppendInfo.Phone = phone;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Set the user's new address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool SetSex(Sex sex)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserAppendInfoItem tempUserItem = context.UserAppendInfoItems.FirstOrDefault(u => u.UserID == userItem.UserID);
                if (tempUserItem != null)
                    tempUserItem.Sex = sex;

                if (context.SaveChanges() > 0)
                {
                    if(userItem.AppendInfo != null)
                        userItem.AppendInfo.Sex = sex;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// The user add the point(for buy the goods).
        /// </summary>
        /// <param name="pointAmount"></param>
        /// <returns></returns>
        public bool AddPoint(int pointAmount)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserAppendInfoItem tempUserItem = context.UserAppendInfoItems.FirstOrDefault(u => u.UserID == userItem.UserID);
                if (tempUserItem != null)
                    tempUserItem.PointAmount += pointAmount;

                context.UserPointITems.Add(new UserPointItem { UserID = UserID, PointAmount = pointAmount, PointType = PointType.BuyGoods, TransTime = DateTime.Now, Remark = "" });
                if (context.SaveChanges() > 0)
                {
                    if(userItem.AppendInfo != null)
                        userItem.AppendInfo.PointAmount += pointAmount;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// The user consume the point.(for buy the goods)
        /// </summary>
        /// <param name="pointAmount"></param>
        /// <returns></returns>
        public bool ConsumePoint(int pointAmount)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserAppendInfoItem tempUserItem = context.UserAppendInfoItems.FirstOrDefault(u => u.UserID == userItem.UserID);
                if (tempUserItem != null && tempUserItem.PointAmount >= pointAmount)
                    tempUserItem.PointAmount -= pointAmount;

                //记录积分的变更
                context.UserPointITems.Add(new UserPointItem { UserID = UserID, PointAmount = -pointAmount, PointType = PointType.ConsumeGoods, TransTime = DateTime.Now, Remark = "" });
                if (context.SaveChanges() > 0)
                {
                    if (userItem.AppendInfo != null)
                        userItem.AppendInfo.PointAmount -= pointAmount;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// The user take off the point.(for return the goods)
        /// </summary>
        /// <param name="pointAmount"></param>
        /// <returns></returns>
        public bool TakeOffPoint(int pointAmount)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserAppendInfoItem tempUserItem = context.UserAppendInfoItems.FirstOrDefault(u => u.UserID == userItem.UserID);
                if (tempUserItem != null && tempUserItem.PointAmount >= pointAmount)
                    tempUserItem.PointAmount -= pointAmount;

                //记录积分的变更
                context.UserPointITems.Add(new UserPointItem { UserID = UserID, PointAmount = -pointAmount, PointType = PointType.ReturnGoods, TransTime = DateTime.Now, Remark = "" });
                if (context.SaveChanges() > 0)
                {
                    if (userItem.AppendInfo != null)
                        userItem.AppendInfo.PointAmount -= pointAmount;
                    return true;
                }

                return false;
            }
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

        /// <summary>
        /// Get the user's email address.
        /// </summary>
        public string Email
        {
            get { return userItem.Email; }
        }

        /// <summary>
        /// Get the user's address
        /// </summary>
        public string Address
        {
            get
            {
                if (userItem.AppendInfo == null)
                {
                    LoadAppendInfo();
                }

                return userItem.AppendInfo.Address;
            }
        }

        public string HeaderImageUrl
        {
            get
            {
                if (userItem.AppendInfo == null)
                    LoadAppendInfo();
                return userItem.AppendInfo.HeadImageUrl;
            }
        }

        /// <summary>
        /// Get the user's point
        /// </summary>
        public int PointAmount
        {
            get
            {
                if (userItem.AppendInfo == null)
                    LoadAppendInfo();

                return userItem.AppendInfo.PointAmount;
            }
        }

        /// <summary>
        /// Get the user's phone number.
        /// </summary>
        public string Phone
        {
            get
            {
                if (userItem.AppendInfo == null)
                {
                    LoadAppendInfo();
                }

                return userItem.AppendInfo.Phone;
            }
        }

        /// <summary>
        /// Get the user's birthday.
        /// </summary>
        public DateTime BirthDay
        {
            get
            {
                if (userItem.AppendInfo == null)
                {
                    LoadAppendInfo();
                }

                return userItem.AppendInfo.BirthDay;
            }
        }

        /// <summary>
        /// Get the user's sex.
        /// </summary>
        public Sex Sex
        {
            get
            {
                if (userItem.AppendInfo == null)
                {
                    LoadAppendInfo();
                }

                return userItem.AppendInfo.Sex;
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
        /// Clear the user's shopping cart.
        /// </summary>
        public void ClearCart()
        {
            cart.CartItems.Clear();
        }

        /// <summary>
        /// Set the cart to User. it happen the user login.
        /// </summary>
        /// <param name="cart">the new cart</param>
        public void SetCart(Cart cart)
        {
            this.cart = cart;            
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
                        if (userItem.AppendInfo == null)
                            LoadAppendInfo();

                        sendAddress = new SendAddressItem { Address = string.Empty, Phone = string.Empty, PostCode = string.Empty };
                        if (!string.IsNullOrEmpty(userItem.AppendInfo.Address))
                            sendAddress.Address = userItem.AppendInfo.Address;
                        if (!string.IsNullOrEmpty(userItem.AppendInfo.Phone))
                            sendAddress.Phone = userItem.AppendInfo.Phone;
                    }
                }

                if (sendAddress == null)
                {
                    sendAddress = new SendAddressItem { Address="", Phone="", PostCode="" };
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
