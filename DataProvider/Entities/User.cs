using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xxx.EntityLib {
    /// <summary>表示用户角色的对象
    /// </summary>
    public class Role {
        /// <summary>表示匿名帐号的对象，不能编辑
        /// </summary>
        public static Role Anonymous = new Role{ RoleID=1, RoleName="Anonymous" };

        /// <summary>表示普通帐号的对象，不能编辑
        /// </summary>
        public static Role NormalUser = new Role { RoleID = 2, RoleName = "NormalUser" };

        /// <summary>表示操作员帐号的对象，不能编辑
        /// </summary>
        public static Role Operator = new Role { RoleID = 3, RoleName = "Operator" };

        /// <summary>表示超级管理员帐号对象，不能编辑
        /// </summary>
        public static Role Super = new Role { RoleID = 4, RoleName = "Super" };

        public Role() { }

        /// <summary>构造一个角色对象
        /// </summary>
        /// <param name="role_id">角色的ID</param>
        /// <param name="role_name">角色的名称</param>
        public Role(int role_id, string role_name) {
            RoleID = role_id;
            RoleName = role_name;
        }

        /// <summary>获取或设置角色的ID
        /// </summary>
        public int RoleID{set; get;}

        /// <summary>获取或设置角色的名称
        /// </summary>
        public string RoleName{set; get;}

        /// <summary>获取或设置角色的说明
        /// </summary>
        public string Description { set; get; }

        /// <summary>获取角色列表的名称列表
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

        /// <summary>获取角色对象的字符串表达式
        /// </summary>
        /// <returns>返回角色对象的字符串表达式</returns>
        public override string ToString() {
            if (this == Role.NormalRole)
                return "普通用户";
            else if (this == Role.OperatorRole)
                return "操作员";
            else if (this == Role.SuperRole)
                return "超级管理员";
            else if (this == Role.AnonymousRole)
                return "匿名用户";
            else
                return "用户";
        }
    }

    /// <summary>系统中的用户信息对象，它表示基本信息
    /// </summary>
    public class User {
        /// <summary>表示匿名用户，它在获取完角色列表时进行初始化
        /// </summary>
        public static User Anonymous = null;

        /// <summary>获取或设置用户的ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>获取或设置用户的名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>用户或设置用户的登录类型
        /// </summary>
        public LoginType LoginType { get; set; }

        /// <summary>外部网站使用时使用的登录ID
        /// </summary>
        public string OpenID { get; set; }

        /// <summary>获取或设置用户的角色
        /// </summary>
        public Role[] Roles { get; set; }

        /// <summary>注册时间
        /// </summary>
        public DateTime RegisterTime { get; set; }

        /// <summary>获取或设置用户的收藏夹内容
        /// </summary>
        public List<FavoriteInfo> Favorites { get; set; }

        /// <summary>获取或设置用户的Email信息
        /// </summary>
        public string Email { get; set; }

        /// <summary>获取或设置用户的附加信息
        /// </summary>
        public UserAppendInfo AppendInfo { get; set; }

        /// <summary>用户的朋友组列表
        /// </summary>
        public IList<FriendGroup> Groups { get; set; }
    }

    /// <summary>用户的朋友组项
    /// </summary>
    public class FriendGroup {
        public int GroupID { get; set; }

        public string GroupName { get; set; }

        public string Remark { get; set; }

        public IList<FriendInfo> Friends { get; set; }
    }

    /// <summary>用户的朋友项
    /// </summary>
    public class FriendInfo {
        /// <summary>朋友的用户
        /// </summary>
        public User FriendUser { get; set; }

        /// <summary>该朋友的备注信息
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>表示用户性别信息的枚举
    /// </summary>
    public enum Sex{
        /// <summary>表示该用户的性别未知，（没有填写)
        /// </summary>
        [StringValue("未知")]
        Unknown,
        /// <summary>男性
        /// </summary>
        [StringValue("男")]
        Male,
        /// <summary>女性
        /// </summary>
        [StringValue("女")]
        Female
    }

    /// <summary>用户的登录类型（可以从其它网站的认证登录）
    /// </summary>
    public enum LoginType {
        /// <summary>用户从本地网站登录
        /// </summary>
        [StringValue("本地用户")]
        Local = 0,

        /// <summary>用户从QQ登录
        /// </summary>
        [StringValue("QQ用户")]
        QQ = 1
    }

    /// <summary>用户的附加信息，通常系统在运行是不获取，除非用户修改或获取这些信息时才获取
    /// </summary>
    public class UserAppendInfo {
        /// <summary>获取或设置用户的地址信息
        /// </summary>
        public string Address { get; set; }

        /// <summary>获取或设置用户的联系电话1
        /// </summary>
        public string Phone1 { get; set; }

        /// <summary>获取或设置用户的联系电话2
        /// </summary>
        public string Phone2 { get; set; }

        /// <summary>获取或设置用户的爱好信息
        /// </summary>
        public string Interest { get; set; }

        /// <summary>获取或设置用户的职业信息
        /// </summary>
        public string Profession { get; set; }

        /// <summary>获取或设置用户的性别信息
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>获取或设置用户的出生日期
        /// </summary>
        public DateTime BirthDate { get; set; }
    }

    /// <summary>表示用户的收藏项
    /// </summary>
    public class FavoriteInfo {
        /// <summary>获取或设置用户收藏的产品
        /// </summary>
        public Product Product { get; set; }

        /// <summary>获取或设置用户收藏的时间
        /// </summary>
        public DateTime FavoriteTime { get; set; }
    }

    /// <summary>表示用户收藏项，用于数据访问时与业务的交付使用
    /// </summary>
    public class TmpFavoriteItem {
        /// <summary>获取或设置用户的ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>获取或设置商品的ID
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>获取或设置用户收藏的时间信息
        /// </summary>
        public DateTime FavoriteTime { get; set; }
    }

    /// <summary>表示用户信息的字段的类型
    /// </summary>
    public enum UserFieldType{
        /// <summary>表示用户的地址信息内容
        /// </summary>
        Address,
        /// <summary>表示用户的性别信息
        /// </summary>
        Sex,
        /// <summary>表示用户的电话信息
        /// </summary>
        Phone,

        /// <summary>表示用户的生日
        /// </summary>
        BirthDay,
        /// <summary>表示用户的兴趣爱好信息
        /// </summary>
        Interest
    }

    /// <summary>表示用户字段的信息
    /// </summary>
    public class UserField {
        private static UserField[] field_type_names = {new UserField(UserFieldType.Address, "address"), 
                                                       new UserField(UserFieldType.BirthDay, "birthday"),
                                                       new UserField(UserFieldType.Interest, "hobbit"),
                                                       new UserField(UserFieldType.Phone, "phone1"),
                                                       new UserField(UserFieldType.Sex, "sex")};

        /// <summary>生成字段类型名称对应关系表
        /// </summary>
        /// <param name="type">字段的类型</param>
        /// <param name="field_name">字段名称</param>
        public UserField(UserFieldType type, string field_name) {
            FieldType = type;
            FieldName = field_name;
        }

        /// <summary>获取或设置用户信息的字段的类型
        /// </summary>
        public UserFieldType FieldType { get; set; }
        /// <summary>获取或设置用户信息的字段
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>获取用户的某个字段名对于的字段类型
        /// </summary>
        /// <param name="field_name">要获取的字段的字段名</param>
        /// <returns>获取到的字段的类型</returns>
        public static UserFieldType GetFieldTypeByName(string field_name) {
            return field_type_names.First(u => u.FieldName == field_name).FieldType;
        }

        /// <summary>获取用户的某个字段类型对应的字段名
        /// </summary>
        /// <param name="field_type">字段的类型</param>
        /// <returns>获取到的字段名</returns>
        public static string GetFileNameByType(UserFieldType field_type) {
            return field_type_names.First(u => u.FieldType == field_type).FieldName;
        }
    }
}
