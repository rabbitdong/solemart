using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xxx.EntityLib;
using Xxx.DataAccessLib;
using System.Security.Cryptography;

namespace Xxx.BusinessLib {
    /// <summary>用户管理类对象，是singleton，通过Instance访问实例
    /// </summary>
    public class UserManager {
        private UserDA uda = UserDA.Instance;
        private UserInfoDA urda = UserInfoDA.Instance;

        private int user_count = 0;

        private static UserManager instance = new UserManager();

        private List<Role> role_list = null;

        private const int PWD_CHAR_LEN = 62; //数字+小写+大写 = 62个字符
        private byte[] pwdchars = null;
        private Random rand = new Random();

        private UserManager() {
            user_count = uda.GetTotalUserCount();

            InitRoles();
            InitUsers();
            InitializePwdChars();
        }

        /// <summary>获取用户管理对象的实例
        /// </summary>
        public static UserManager Instance {
            get {
                return instance;
            }
        }

        /// <summary>新增一个注册用户信息
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">用户密码，明文</param>
        /// <param name="email">用户名的email地址</param>
        /// <returns>成功注册返回新注册的用户对象，否则返回null</returns>
        public User AddNewUser(string name, string pwd, string email){
            int tmpuid = TotalUserCount + 1;
            
            if (uda.RegisterNewUser(tmpuid, name, GetHashPwd(pwd), email)) {
                user_count++;
                User current_user = new User();
                current_user.UserID = tmpuid;
                current_user.Name = name;
                current_user.Email = email;
                current_user.Roles = new Role[] { Role.NormalRole };

                uda.AndUserAppendInfo(tmpuid);
                urda.SetUserRole(tmpuid, Role.NormalRole.RoleID);
                return current_user;
            }

            return null;
        }

        /// <summary>新增一个QQ注册用户信息
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns>成功注册返回新注册的用户对象，否则返回null</returns>
        public User AddNewQQUser(string openid, string nickname) {
            int tmpuid = TotalUserCount + 1;

            if (uda.RegisterNewQQUser(tmpuid, openid)) {
                user_count++;
                User current_user = new User();
                current_user.UserID = tmpuid;
                current_user.Name = nickname;
                current_user.OpenID = openid;
                current_user.Roles = new Role[] { Role.NormalRole };

                uda.AndUserAppendInfo(tmpuid, nickname);
                urda.SetUserRole(tmpuid, Role.NormalRole.RoleID);
                return current_user;
            }

            return null;
        }

        /// <summary>判断用户名user_name是否被注册过
        /// </summary>
        /// <param name="user_name">需要判断的用户名</param>
        /// <returns>如果被注册，返回true，否则返回false</returns>
        public bool IsUserNameDuplicate(string user_name) {
            return uda.IsUserNameDuplicate(user_name);
        }

        /// <summary>判断Email地址是否被注册过
        /// </summary>
        /// <param name="email">需要判断的email地址</param>
        /// <returns>如果被注册，返回true，否则返回false</returns>
        public bool IsEmailDuplicate(string email) {
            return uda.IsEmailDuplicate(email);
        }
        /// <summary>初始化角色列表
        /// </summary>
        private void InitRoles() {
            role_list = urda.GetRoleList();

            if (Role.AnonymousRole == null ||
                Role.NormalRole == null ||
                Role.OperatorRole == null ||
                Role.SuperRole == null) {
                Role.AnonymousRole = role_list.Find(r => r.RoleID == -1); 
                Role.NormalRole = role_list.Find(r => r.RoleID == 1);
                Role.OperatorRole = role_list.Find(r => r.RoleID == 2);
                Role.SuperRole = role_list.Find(r => r.RoleID == 3);
            }
        }

        /// <summary>初始化用户(某些特殊用户，如匿名用户等)
        /// </summary>
        private void InitUsers() {
            User.Anonymous = new User() { UserID = -1, Name = "anonymous", Roles = new Role[]{ Role.AnonymousRole }, Email = "" };
        }

        /// <summary>获取当前系统的所有角色列表
        /// </summary>
        public Role[] AllRoles {
            get {
                return role_list.ToArray();
            }
        }

        /// <summary>获取当前中用户数
        /// </summary>
        public int TotalUserCount {
            get{
                if (user_count == 0)
                    user_count = uda.GetTotalUserCount();

                return user_count;
            }
        }

        /// <summary>获取Hash后的密码字符串
        /// </summary>
        /// <param name="pwd">原始字符串</param>
        /// <returns></returns>
        private string GetHashPwd(string pwd) {
            MD5 md5 = MD5.Create();
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(pwd);
            byte[] hashed_bytes = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();

            foreach (byte b in hashed_bytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        /// <summary>用户登录的处理
        /// </summary>
        /// <param name="name">登录的用户名</param>
        /// <param name="pwd">登录的用户的密码，明文</param>
        /// <returns>登录成功，返回一个用户对象代表当前登录的用户</returns>
        public User OnLogin(string name, string pwd) {
            int uid = 0;
            if (pwd == "")
                uid = uda.Login(name, pwd);
            else
                uid = uda.Login(name, GetHashPwd(pwd));

            User user = new User();
            if (uid > 0) {
                return uda.GetUserByID(uid);
            }

            return null;
        }

        /// <summary>用户在QQ登录后的处理
        /// </summary>
        /// <param name="openid">QQ登录使用的openid</param>
        /// <param name="nickname">用户QQ登录的昵称</param>
        /// <returns>登录成功，返回一个用户对象代表当前登录的用户</returns>
        public User OnQQLogin(string openid, string nickname) {
            int uid = uda.LoginQQ(openid);
            if (uid > 0) {
                User user = uda.GetUserByID(uid);
                user.Name = nickname;
                return user;
            }
            else {
                return AddNewQQUser(openid, nickname);
            }
        }

        /// <summary>通过用户名获取用户对象
        /// </summary>
        /// <param name="user_name">要获取的用户的用户名</param>
        /// <returns>获取到的用户对象，如果没有，返回null</returns>
        public User GetUserByName(string user_name) {
            User user = uda.GetUserByName(user_name);
            if (user != null) {
                Role[] roles = GetRoleByUserId(user.UserID);
                if (roles != null && roles.Length > 0) {
                    user.Roles = roles;
                }
                else
                    user.Roles = new Role[] { Role.NormalRole };

                return user;
            }
            return null;
        }

        /// <summary>通过OpenId获取用户对象
        /// </summary>
        /// <param name="openid">要获取的用户的OpenId</param>
        /// <returns></returns>
        public User GetUserByOpenId(string openid) {
            User user = uda.GetUserByOpenId(openid);
            if (user != null && user.Name.Trim() == "") {
                user.Name = uda.GetNickNameByUserID(user.UserID);
                Role[] roles = GetRoleByUserId(user.UserID);
                if (roles != null && roles.Length > 0) {
                    user.Roles = roles;
                }
                else
                    user.Roles = new Role[] { Role.NormalRole };

                return user;
            }

            return null;
        }

        /// <summary>通过用户ID获取用户对象
        /// </summary>
        /// <param name="user_id">要获取的用户的ID</param>
        /// <returns>返回获取到的用户对象，如果没有该ID的用户，返回null</returns>
        public User GetUserByID(int user_id) {
            User user = uda.GetUserByID(user_id);

            return user;
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
        public List<User> GetUserList(int page_index, int page_size) {
            List<User> users = uda.GetUserList(page_index, page_size);
            if (users != null) {
                IEnumerable<KeyValuePair<int, int>> user_roles = urda.GetUsersRole(from user in users select user.UserID);
                foreach (User user in users) {
                    IEnumerable<int> roleids = from ur in user_roles 
                                               where ur.Key == user.UserID select ur.Value;
                    user.Roles = (from r in role_list where roleids.Contains(r.RoleID) 
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

        /// <summary>初始化生成密码用的字符
        /// </summary>
        /// <remarks>目前只是有数字、大写字母、小写字母构成</remarks>
        private void InitializePwdChars() {
            pwdchars = new byte[PWD_CHAR_LEN];

            int num = 0;
            for (int i = 0; i < 10; ++i)
                pwdchars[num++] = (byte)('0' + i);

            for (int j = 'a'; j <= 'z'; ++j)
                pwdchars[num++] = (byte)j;

            for (int j = 'A'; j <= 'Z'; ++j)
                pwdchars[num++] = (byte)j;
        }

        /// <summary>生成一个8位随机的密码
        /// </summary>
        /// <returns></returns>
        /// <remarks>生成的8位密码由10个数字、32个小写字母、32个大写字母组成</remarks>
        public string GenerateRandomPwd() {
            int pwd_len = 8;
            byte[] pwd = new byte[pwd_len];

            for (int i = 0; i < pwd_len; ++i) {
                pwd[i] = pwdchars[rand.Next(PWD_CHAR_LEN)];
            }

            return Encoding.ASCII.GetString(pwd);
        }
    }
}
