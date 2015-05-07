using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Solemart.DataProvider;
using Solemart.DataProvider.Entity;

namespace Solemart.BusinessLib
{
    /// <summary>用户管理类对象，是singleton，通过Instance访问实例
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">用户密码，明文</param>
        /// <param name="email">用户名的email地址</param>
        /// <returns>成功注册返回新注册的用户对象，否则返回null</returns>
        public static UserItem AddNewUser(string name, string pwd, string email)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                int userid = context.RegisterNewUser(name, email, pwd, DateTime.Now);
                if (userid > 0)
                {
                    context.UserAppendInfoItems.Add(new UserAppendInfoItem { UserID = userid });
                    return new UserItem { UserID = userid, UserName = name, Email = email, Roles = Role.NormalUser.ToString(), LoginType = SystemUtil.LoginType.Local };
                }

                return null;
            }
        }

        /// <summary>
        /// Add a new qq user
        /// </summary>
        /// <param name="openid">用户名</param>
        /// <returns>成功注册返回新注册的用户对象，否则返回null</returns>
        public static UserItem AddNewQQUser(string openid, string nickname)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                int userid = context.RegisterNewQQUser(nickname, "", "", DateTime.Now);
                if (userid > 0)
                {
                    context.UserAppendInfoItems.Add(new UserAppendInfoItem { UserID = userid });
                    return new UserItem { UserID = userid, UserName = nickname, Email = "", Roles = Role.NormalUser.ToString(), LoginType = SystemUtil.LoginType.QQ };
                }

                return null;
            }
        }

        /// <summary>
        /// Is the user name registed.
        /// </summary>
        /// <param name="userName">需要判断的用户名</param>
        /// <returns>如果被注册，返回true，否则返回false</returns>
        public static bool IsUserNameDuplicate(string userName)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem user = context.UserItems.FirstOrDefault(u => u.UserName == userName);
                return user != null;
            }
        }

        /// <summary>
        /// Is the email registed
        /// </summary>
        /// <param name="email">需要判断的email地址</param>
        /// <returns>如果被注册，返回true，否则返回false</returns>
        public static bool IsEmailDuplicate(string email)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem user = context.UserItems.FirstOrDefault(u => u.Email == email);
                return user != null;
            }
        }

        /// <summary>获取当前中用户数
        /// </summary>
        public static int TotalUserCount
        {
            get
            {
                using (SolemartDBContext context = new SolemartDBContext())
                {
                    return context.UserItems.Count();
                }
            }
        }

        /// <summary>
        /// 用户登录的处理
        /// </summary>
        /// <param name="name">登录的用户名</param>
        /// <param name="pwd">登录的用户的密码，明文</param>
        /// <returns>登录成功，返回一个用户对象代表当前登录的用户</returns>
        public static SolemartUser OnLogin(string name, string pwd)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                if (context.ValidateUserPassword(name, pwd))
                {
                    UserItem useritem = context.UserItems.FirstOrDefault(u => u.UserName == name);
                    return new SolemartUser(useritem);
                }

                return null;
            }
        }

        /// <summary>用户在QQ登录后的处理
        /// </summary>
        /// <param name="openid">QQ登录使用的openid</param>
        /// <param name="nickname">用户QQ登录的昵称</param>
        /// <returns>登录成功，返回一个用户对象代表当前登录的用户</returns>
        public static UserItem OnQQLogin(string openid, string nickname)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem user = context.UserItems.FirstOrDefault(u => u.OpenID == openid);
                if (user != null)
                    return user;
                user = new UserItem();
                user.OpenID = openid;
                user.UserName = nickname;
                user.LoginType = SystemUtil.LoginType.QQ;
                user.RegTime = DateTime.Now;
                user.Email = "123@qq.com";
                context.UserItems.Add(user);
                if (context.SaveChanges() > 0)
                    return user;

                return null;
            }
        }

        /// <summary>
        /// Get the user by user name.
        /// </summary>
        /// <param name="userName">要获取的用户的用户名</param>
        /// <returns>获取到的用户对象，如果没有，返回null</returns>
        public static UserItem GetUserByName(string userName)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem user = context.UserItems.FirstOrDefault(u => u.UserName == userName);
                return user;
            }
        }

        /// <summary>
        /// Get the user by the openid
        /// </summary>
        /// <param name="openID">要获取的用户的OpenId</param>
        /// <returns></returns>
        public static UserItem GetUserByOpenId(string openID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.UserItems.FirstOrDefault(u => u.OpenID == openID);
            }
        }

        /// <summary>
        /// Get the user by id
        /// </summary>
        /// <param name="userID">要获取的用户的ID</param>
        /// <returns>返回获取到的用户对象，如果没有该ID的用户，返回null</returns>
        public static UserItem GetUserByID(int userID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.UserItems.Find(userID);
            }
        }

        /// <summary>获取某个用户的送货地址信息
        /// </summary>
        /// <param name="user_id">要获取的用户的ID</param>
        /// <returns>如果获取到，返回该地址信息，否则返回null</returns>
        public static SendAddressItem GetSendAddressInfo(int userID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.SendAddressItems.Find(userID);
            }
        }

        /// <summary>保存用户的送货地址信息
        /// </summary>
        /// <param name="addressItem">保存的送货地址信息</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public static bool SaveSendAddressInfoForUser(SendAddressItem addressItem)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                SendAddressItem address = context.SendAddressItems.Find(addressItem.UserID);
                if (address != null)
                {
                    address.Phone = addressItem.Phone;
                    address.PostCode = addressItem.PostCode;
                    address.Receiver = addressItem.Receiver;
                    address.PaymentType = addressItem.PaymentType;
                    address.Address = addressItem.Address;
                    address.DeliverWay = addressItem.DeliverWay;
                }
                else
                {
                    context.SendAddressItems.Add(addressItem);
                }

                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Get the paged user list
        /// </summary>
        /// <param name="pageIndex">要获取的页索引(从0开始)</param>
        /// <param name="pageSize">要获取的页大小</param>
        /// <param name="totalPageCount"></param>
        /// <returns>获取到的用户列表</returns>
        public static List<UserItem> GetUserList(int pageIndex, int pageSize, out int totalPageCount)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                var q = from u in context.UserItems
                        orderby u.UserID
                        select u;
                totalPageCount = (q.Count() - 1) / pageSize + 1;
                return q.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// Modify the user's role
        /// </summary>
        /// <param name="userID">要修改的用户ID</param>
        /// <param name="roleIDS">修改后的新角色</param>
        /// <returns>是否成功修改，成功修改返回true，否则返回false</returns>
        public static bool ModifyUserRole(int userID, string roleIDS)
        {
            return true;
        }

        /// <summary>
        /// Get the user favorite list
        /// </summary>
        /// <param name="userID">要获取收藏夹的用户ID</param>
        /// <returns>用户的收藏夹列表信息，如果没有内容，返回null</returns>
        public static IList<FavoriteItem> GetPagedUserFavoriteList(int userID, int pageIndex, int pageSize, out int totalPageCount)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                var q = from f in context.FavoriteItems.Include("Product")
                        orderby f.FavoriteTime descending
                        where f.UserID == userID
                        select f;
                totalPageCount = (q.Count() - 1) / pageSize + 1;
                return q.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// Add a new favorite for user
        /// </summary>
        /// <param name="userID">添加收藏夹项的用户</param>
        /// <param name="productID">添加的商品</param>
        /// <returns>成功收藏返回true，否则返回false</returns>
        public static bool AddNewFavorite(int userID, int productID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                FavoriteItem favorite = context.FavoriteItems.FirstOrDefault(f => (f.UserID == userID && f.ProductID == productID));
                if (favorite != null)
                {
                    favorite.FavoriteTime = DateTime.Now;
                }
                else
                {
                    context.FavoriteItems.Add(favorite);
                }

                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Get the user append info
        /// </summary>
        /// <param name="userID">要获取的用户的ID</param>
        /// <returns>返回获取到的其它信息内容</returns>
        public static UserAppendInfoItem GetUserAppendInfo(int userID)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.UserAppendInfoItems.Find(userID);
            }
        }

        /// <summary>
        /// Update user info
        /// </summary>
        /// <param name="user">要更新的用户的ID</param>
        /// <returns>是否成功更新, true：更新成功, false:更新失败</returns>
        public static bool UpdateUserInfo(UserItem user)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem old = context.UserItems.Find(user.UserID);
                if (old != null)
                {
                    old.UserName = user.UserName;
                    old.Email = user.Email;
                    return context.SaveChanges() > 0;
                }

                return false;
            }
        }

        /// <summary>
        /// Update the user password
        /// </summary>
        /// <param name="userID">要更新密码的用户ID</param>
        /// <param name="newPassword">更新的新密码</param>
        /// <returns>是否更新成功</returns>
        public static bool UpdateUserPwd(int userID, string newPassword)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                return context.UpdateUserPassword(userID, newPassword);
            }
        }

        /// <summary>
        /// Update birthday of the user
        /// </summary>
        /// <param name="userID">要更新的用户的ID</param>
        /// <param name="newBirthDay">新的用户的生日</param>
        /// <returns>是否成功更新, true：更新成功, false:更新失败</returns>
        public static bool UpdateUserBirthDay(int userID, DateTime newBirthDay)
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserAppendInfoItem userAppendInfo = context.UserAppendInfoItems.Find(userID);
                if (userAppendInfo != null)
                {
                    userAppendInfo.BirthDay = newBirthDay;
                    return context.SaveChanges() > 0;
                }

                return false;
            }
        }
    }
}
