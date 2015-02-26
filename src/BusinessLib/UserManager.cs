using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Solemart.DataProvider;
using Solemart.DataProvider.Entity;

namespace Solemart.BusinessLib {
    /// <summary>用户管理类对象，是singleton，通过Instance访问实例
    /// </summary>
    public class UserManager {
        private List<Role> roleList = null;

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">用户密码，明文</param>
        /// <param name="email">用户名的email地址</param>
        /// <returns>成功注册返回新注册的用户对象，否则返回null</returns>
        public UserItem AddNewUser(string name, string pwd, string email){
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem user = new UserItem();
                user.UserName = name;
                user.Email = email;
                user.RegTime = DateTime.Now;
                user.LoginType = SystemUtil.LoginType.Local;
                context.UserItems.Add(user);
                if (context.SaveChanges() > 0)
                    return user;

                return null;
            }
        }

        /// <summary>
        /// Add a new qq user
        /// </summary>
        /// <param name="openid">用户名</param>
        /// <returns>成功注册返回新注册的用户对象，否则返回null</returns>
        public UserItem AddNewQQUser(string openid, string nickname) {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem user = new UserItem();
                user.OpenID = openid;
                user.UserName = nickname;
                user.RegTime = DateTime.Now;
                user.LoginType = SystemUtil.LoginType.QQ;
                context.UserItems.Add(user);
                if (context.SaveChanges() > 0)
                    return user;

                return null;
            }
        }

        /// <summary>
        /// Is the user name registed.
        /// </summary>
        /// <param name="userName">需要判断的用户名</param>
        /// <returns>如果被注册，返回true，否则返回false</returns>
        public bool IsUserNameDuplicate(string userName) {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem user = context.UserItems.FirstOrDefault(u => u.UserName == userName);
                return user == null;
            }
        }

        /// <summary>
        /// Is the email registed
        /// </summary>
        /// <param name="email">需要判断的email地址</param>
        /// <returns>如果被注册，返回true，否则返回false</returns>
        public bool IsEmailDuplicate(string email) {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                UserItem user = context.UserItems.FirstOrDefault(u => u.Email == email);
                return user == null;
            }
        }

        /// <summary>获取当前中用户数
        /// </summary>
        public int TotalUserCount 
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
        public UserItem OnLogin(string name, string pwd) {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                if (context.ValidateUserPassword(name, pwd))
                {
                    return context.UserItems.FirstOrDefault(u => u.UserName == name);
                }

                return null;
            }
        }

        /// <summary>用户在QQ登录后的处理
        /// </summary>
        /// <param name="openid">QQ登录使用的openid</param>
        /// <param name="nickname">用户QQ登录的昵称</param>
        /// <returns>登录成功，返回一个用户对象代表当前登录的用户</returns>
        public UserItem OnQQLogin(string openid, string nickname) {
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
        /// 通过用户名获取用户对象
        /// </summary>
        /// <param name="userName">要获取的用户的用户名</param>
        /// <returns>获取到的用户对象，如果没有，返回null</returns>
        public UserItem GetUserByName(string userName) {
            UserItem UserItem = uda.GetUserByName(userName);
            if (UserItem != null) {
                Role[] roles = GetRoleByUserId(UserItem.UserID);
                if (roles != null && roles.Length > 0) {
                    UserItem.Roles = roles;
                }
                else
                    UserItem.Roles = new Role[] { Role.NormalRole };

                return UserItem;
            }
            return null;
        }

        /// <summary>通过OpenId获取用户对象
        /// </summary>
        /// <param name="openid">要获取的用户的OpenId</param>
        /// <returns></returns>
        public UserItem GetUserByOpenId(string openid) {
            UserItem UserItem = uda.GetUserByOpenId(openid);
            if (UserItem != null && UserItem.Name.Trim() == "") {
                UserItem.Name = uda.GetNickNameByUserID(UserItem.UserID);
                Role[] roles = GetRoleByUserId(UserItem.UserID);
                if (roles != null && roles.Length > 0) {
                    UserItem.Roles = roles;
                }
                else
                    UserItem.Roles = new Role[] { Role.NormalRole };

                return UserItem;
            }

            return null;
        }

        /// <summary>通过用户ID获取用户对象
        /// </summary>
        /// <param name="user_id">要获取的用户的ID</param>
        /// <returns>返回获取到的用户对象，如果没有该ID的用户，返回null</returns>
        public UserItem GetUserByID(int user_id) {
            UserItem UserItem = uda.GetUserByID(user_id);

            return UserItem;
        }

        /// <summary>获取某个用户所代码的角色信息
        /// </summary>
        /// <param name="user_id">用户ID</param>
        /// <returns>获取的用户角色信息</returns>
        public Role[] GetRoleByUserId(int user_id) {
            int[] role_ids = urda.GetRoleIdsByUserId(user_id);

            int role_num = role_ids.Length;
            Role[] roles = new Role[role_num];
            for (int i = 0; i < role_num; ++i) {
                roles[i] = role_list.Find(r => r.RoleID == role_ids[i]);
            }

            return roles;
        }

        /// <summary>获取某个用户的送货地址信息
        /// </summary>
        /// <param name="user_id">要获取的用户的ID</param>
        /// <returns>如果获取到，返回该地址信息，否则返回null</returns>
        public SendAddressInfo GetSendAddressInfo(int user_id) {
            return urda.GetSendAddressInfo(user_id);
        }

        /// <summary>保存用户的送货地址信息
        /// </summary>
        /// <param name="addr_info">保存的送货地址信息</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public bool SaveSendAddressInfoForUser(SendAddressInfo addr_info) {
            if (urda.HasSendAddressInfo(addr_info.UserID)) {
                return urda.UpdateSendAddressInfo(addr_info);
            }
            else
                return urda.AddNewSendAddressInfo(addr_info);
        }

        /// <summary>获取当前注册的用户的列表
        /// </summary>
        /// <param name="page_index">要获取的页索引(从0开始)</param>
        /// <param name="page_size">要获取的页大小</param>
        /// <returns>获取到的用户列表</returns>
        public List<UserItem> GetUserList(int page_index, int page_size) {
            List<UserItem> users = uda.GetUserList(page_index, page_size);
            if (users != null) {
                IEnumerable<KeyValuePair<int, int>> user_roles = urda.GetUsersRole(from UserItem in users select UserItem.UserID);
                foreach (UserItem UserItem in users) {
                    IEnumerable<int> roleids = from ur in user_roles 
                                               where ur.Key == UserItem.UserID select ur.Value;
                    UserItem.Roles = (from r in role_list where roleids.Contains(r.RoleID) 
                                  select r).ToArray();
                }
            }

            return users;
        }

        /// <summary>获取角色的字符串表达式
        /// </summary>
        /// <param name="roles">角色的列表</param>
        /// <returns>字符串表达式</returns>
        public string GetRolesString(Role[] roles) {
            StringBuilder sb = new StringBuilder();
            int roles_count = roles.Length;

            if (roles_count >= 1)
                sb.Append(roles[0].ToString());

            for (int i = 1; i < roles_count; ++i) {
                sb.AppendFormat(",{0}", roles[i]);
            }

            return sb.ToString();
        }

        /// <summary>修改用户角色信息
        /// </summary>
        /// <param name="uid">要修改的用户ID</param>
        /// <param name="new_role">修改后的新角色</param>
        /// <returns>是否成功修改，成功修改返回true，否则返回false</returns>
        public bool ModifyUserRole(int uid, Role new_role) {
            if (new_role == null)
                return false;

            return urda.ModifyUserRole(uid, new_role.RoleID);
        }

        /// <summary>获取用户的收藏夹列表
        /// </summary>
        /// <param name="uid">要获取收藏夹的用户ID</param>
        /// <returns>用户的收藏夹列表信息，如果没有内容，返回null</returns>
        public IList<FavoriteInfo> GetUserFavoriteList(int uid) {
            IList<TmpFavoriteItem> fav_items = urda.GetUserFavoriteList(uid);

            if (fav_items != null && fav_items.Count > 0) {
                List<FavoriteInfo> favorites = new List<FavoriteInfo>();
                foreach (TmpFavoriteItem fav in fav_items) {
                    FavoriteInfo favorite = new FavoriteInfo();
                    favorite.Product = ProductManager.Instance.GetProductByID(fav.ProductID);
                    favorite.FavoriteTime = fav.FavoriteTime;
                    favorites.Add(favorite);
                }
                return favorites;
            }

            return null;
        }

        /// <summary>用户uid新加一个收藏夹项
        /// </summary>
        /// <param name="uid">添加收藏夹项的用户</param>
        /// <param name="product">添加的商品</param>
        /// <returns>成功收藏返回true，否则返回false</returns>
        public bool AddNewFavorite(int uid, Product product) {
            if (urda.HasFavoriteProduct(uid, product.ProductID)) {
                return urda.UpdateFavorite(uid, product.ProductID);
            }
            else {
                return urda.AddNewFavorite(uid, product.ProductID);
            }
        }

        /// <summary>获取用户的其它信息内容
        /// </summary>
        /// <param name="uid">要获取的用户的ID</param>
        /// <returns>返回获取到的其它信息内容</returns>
        public UserAppendInfo GetUserAppendInfo(int uid) {
            return uda.GetUserAppendInfo(uid);
        }

        /// <summary>更新用户信息
        /// </summary>
        /// <param name="uid">要更新的用户的ID</param>
        /// <param name="field">要更新的用户字段</param>
        /// <param name="new_value">新的用户的值</param>
        /// <returns>是否成功更新, true：更新成功, false:更新失败</returns>
        public bool UpdateUserInfo(int uid, string field, string new_value) {
            return uda.UpdateUserInfo(uid, field, new_value);
        }

        /// <summary>更新用户的密码
        /// </summary>
        /// <param name="uid">要更新密码的用户ID</param>
        /// <param name="new_pwd">更新的新密码</param>
        /// <returns>是否更新成功</returns>
        public bool UpdateUserPwd(int uid, string new_pwd) {
            return uda.UpdateUserPwd(uid, GetHashPwd(new_pwd));
        }

        /// <summary>更新用户生日信息
        /// </summary>
        /// <param name="uid">要更新的用户的ID</param>
        /// <param name="new_value">新的用户的生日</param>
        /// <returns>是否成功更新, true：更新成功, false:更新失败</returns>
        public bool UpdateUserBirthDay(int uid, DateTime new_value) {
            return uda.UpdateUserBirthDay(uid, new_value);
        }        
    }
}
