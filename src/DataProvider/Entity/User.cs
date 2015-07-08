using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Solemart.SystemUtil;

namespace Solemart.DataProvider.Entity
{
    #region 用户角色的对象  Role
    /// <summary>
    /// 表示用户角色的对象
    /// </summary>
    public class Role 
    {
        /// <summary>
        /// 匿名帐号的对象，不能编辑
        /// </summary>
        public static Role Anonymous = new Role { RoleID = 1, RoleName = "Anonymous" };

        /// <summary>
        /// 普通帐号的对象，不能编辑
        /// </summary>
        public static Role NormalUser = new Role { RoleID = 2, RoleName = "NormalUser" };

        /// <summary>
        /// 操作员帐号的对象，不能编辑
        /// </summary>
        public static Role Operator = new Role { RoleID = 3, RoleName = "Operator" };

        /// <summary>
        /// 超级管理员帐号对象，不能编辑
        /// </summary>
        public static Role Super = new Role { RoleID = 4, RoleName = "Super" };

        private static List<Role> RoleList = new List<Role> { Anonymous, NormalUser, Operator, Super };

        public Role() { }

        /// <summary>
        /// 构造一个角色对象
        /// </summary>
        /// <param name="roleID">角色的ID</param>
        /// <param name="roleName">角色的名称</param>
        public Role(int roleID, string roleName) {
            RoleID = roleID;
            RoleName = roleName;
        }

        /// <summary>
        /// 获取或设置角色的ID
        /// </summary>
        public int RoleID{set; get;}

        /// <summary>
        /// 获取或设置角色的名称
        /// </summary>
        public string RoleName{set; get;}

        /// <summary>
        /// 获取或设置角色的说明
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// 获取角色列表的名称列表
        /// </summary>
        /// <param name="roles">角色类别</param>
        /// <returns>字符串数组表示其名称列表</returns>
        public static string[] GetRoleNames(Role[] roles) {
            int len = roles.Length;
            string[] role_names = new string[len];
            for (int i = 0; i < len; ++i)
                role_names[i] = roles[i].RoleName;

            return role_names;
        }

        /// <summary>
        /// Get the role list for the role id list.
        /// </summary>
        /// <param name="roleIDS"></param>
        /// <returns></returns>
        public static Role[] GetRoles(string roleIDS)
        {
            var rolelist = roleIDS.Split(',');
            return RoleList.Where(p => rolelist.Contains(p.RoleID.ToString())).ToArray();
        }

        /// <summary>
        /// Get the role id string for the role list.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static string GetRoleIDString(Role[] roles)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Role role in roles)
                sb.AppendFormat("{0},", role.RoleID);

            string rolestr = sb.ToString();
            if (!string.IsNullOrEmpty(rolestr))
                rolestr = rolestr.TrimEnd(',');

            return rolestr;
        }

        /// <summary>
        /// 获取角色对象的字符串表达式
        /// </summary>
        /// <returns>返回角色对象的字符串表达式</returns>
        public override string ToString() {
            if (this == Role.NormalUser)
                return "普通用户";
            else if (this == Role.Operator)
                return "操作员";
            else if (this == Role.Super)
                return "超级管理员";
            else if (this == Role.Anonymous)
                return "匿名用户";
            else
                return "用户";
        }
    }
    #endregion

    #region 系统中的用户项实体  UserItem
    /// <summary>
    /// 系统中的用户信息对象，它表示基本信息
    /// </summary>
    [Table("useritems")]
    public class UserItem
    {
        /// <summary>获取或设置用户的ID
        /// </summary>
        [Key]
        public int UserID { get; set; }

        /// <summary>获取或设置用户的名字
        /// </summary>
        public string UserName { get; set; }

        /// <summary>用户或设置用户的登录类型
        /// </summary>
        public LoginType LoginType { get; set; }

        /// <summary>外部网站使用时使用的登录ID
        /// </summary>
        public string OpenID { get; set; }

        /// <summary>获取或设置用户的角色
        /// </summary>
        public string Roles { get; set; }

        /// <summary>注册时间
        /// </summary>
        public DateTime RegTime { get; set; }

        /// <summary>
        /// 获取或设置用户的Email信息
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 最后一次登陆时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 获取或设置用户的附加信息
        /// </summary>
        public UserAppendInfoItem AppendInfo { get; set; }

        public virtual ICollection<FavoriteItem> Favovrites { get; set; }
        public virtual ICollection<OrderItem> Orders { get; set; }
        public virtual ICollection<CartItem> Cart { get; set; }
        public virtual ICollection<SendAddressItem> SendAddress { get; set; }
    }
    #endregion

    #region 用户的附加信息，通常系统在运行是不获取，除非用户修改或获取这些信息时才获取  UserAppendInfoItem
    /// <summary>
    /// 用户的附加信息，通常系统在运行是不获取，除非用户修改或获取这些信息时才获取
    /// </summary>
    [Table("userappendinfoitems")]
    public class UserAppendInfoItem
    {
        /// <summary>
        /// UserID for the user.
        /// </summary>
        [Key, ForeignKey("UserOf")]
        public int UserID { get; set; }

        public UserItem UserOf { get; set; }

        /// <summary>
        /// NickName for the user.
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// The point of user having.
        /// </summary>
        public int PointAmount { get; set; }

        /// <summary>
        /// The question for losing the password.
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// The answer for losing the password.
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// The user's real name.
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// The user's sex.
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// The user's head images url.
        /// </summary>
        public string HeadImageUrl { get; set; }

        /// <summary>
        /// The user's birthday.
        /// </summary>
        public DateTime BirthDay { get; set; }

        /// <summary>
        /// The user's URL for blog etc.
        /// </summary>
        public string SpaceURL { get; set; }

        /// <summary>
        /// The hobit of the user
        /// </summary>
        public string Hobits { get; set; }

        /// <summary>
        /// The country of the user from.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// The province of the user from.
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// The city of the user from.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The address for the user.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The user's phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// The extent content for the user(JSON)
        /// </summary>
        public string ExtContent { get; set; }
    }
    #endregion

    #region 用户积分表
    /// <summary>
    /// The user point transaction logging table
    /// </summary>
    [Table("userpointitems")]
    public class UserPointItem
    {
        [Key]
        public long AutoID { get; set; }

        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual UserItem UserItem { get; set; }

        public int PointAmount { get; set; }

        public PointType PointType { get; set; }

        public DateTime TransTime { get; set; }

        public string Remark { get; set; }
    }
    #endregion

    #region 用户收藏项  FavoriteItem
    /// <summary>
    /// User favorite item
    /// </summary>
    [Table("favoriteitems")]
    public class FavoriteItem
    {
        /// <summary>
        /// 用户的ID
        /// </summary>
        [Key, Column(Order = 0)]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual UserItem OwnedUser { get; set; }
        /// <summary>
        /// 商品的ID
        /// </summary>
        [Key, Column(Order = 1)]
        public int ProductID { get; set; }

        /// <summary>
        /// 用户收藏的时间信息
        /// </summary>
        public DateTime FavoriteTime { get; set; }

        /// <summary>
        /// The favorite item description.
        /// </summary>
        public string Description { get; set; }
    }
    #endregion
}
