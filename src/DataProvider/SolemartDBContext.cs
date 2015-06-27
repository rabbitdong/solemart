using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;

namespace Solemart.DataProvider
{
    using Entity;
    using Solemart.SystemUtil;

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class SolemartDBContext : DbContext
    {
        public SolemartDBContext() : base(ConfigSettings.ConnectionString) { }

        #region DB tables
        public virtual DbSet<ProductCommentItem> ProductCommentItems { get; set; }
        public virtual DbSet<CategoryItem> CategoryItems { get; set; }
        public virtual DbSet<BrandItem> BrandItems { get; set; }
        public virtual DbSet<ProductImageItem> ProductImageItems { get; set; }
        public virtual DbSet<ProductItem> ProductItems { get; set; }
        public virtual DbSet<SaledProductItem> SaledProductItems { get; set; }
        public virtual DbSet<PriceHistoryItem> PriceHistoryItems { get; set; }
        public virtual DbSet<InStockItem> InStockItems { get; set; }
        public virtual DbSet<VendorItem> VendorItems { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderDetailItem> OrderDetailItems { get; set; }
        public virtual DbSet<SendAddressItem> SendAddressItems { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<UserItem> UserItems { get; set; }
        public virtual DbSet<UserAppendInfoItem> UserAppendInfoItems { get; set; }
        public virtual DbSet<FavoriteItem> FavoriteItems { get; set; }
        public virtual DbSet<AdviserItem> AdviserItems { get; set; }
        public virtual DbSet<BulletinItem> BulletinItems { get; set; }
        public virtual DbSet<UserPointItem> UserPointITems { get; set; }
        #endregion

        public bool ClearTableData(Type tableType)
        {
            string sql = string.Format("truncate table {0}s", tableType.Name);
            return this.Database.ExecuteSqlCommand(sql) > 0;
        }

        /// <summary>
        /// Validate the user's password
        /// </summary>
        /// <param name="username">The user name to validate</param>
        /// <param name="password">The password to validate</param>
        /// <returns></returns>
        public bool ValidateUserPassword(string username, string password)
        {
            password = EncryptUtil.GetHashPwd(password);
#if TEST
            string sql = "select count(1) from TestUserItems where username=@un and PASSWORD=@pwd";
#else
            string sql = "select count(1) from UserItems where username=@un and PASSWORD=@pwd";
#endif
            var q = this.Database.SqlQuery<int>(sql,
                new MySqlParameter("@un", username), new MySqlParameter("@pwd", password));
            return q.FirstOrDefault() > 0;
        }

        /// <summary>
        /// Insert a new local register user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="regTime"></param>
        /// <returns></returns>
        public int RegisterNewUser(string username, string email, string password, DateTime regTime)
        {
            password = EncryptUtil.GetHashPwd(password);
#if TEST
            string sql = "insert into TestUserItems(UserName, Email, Password, LoginType, RegTime, Roles) values(@UserName, @Email, @Password, 0, @RegTime, '2');select LAST_INSERT_ID();";
#else
            string sql = "insert into UserItems(UserName, Email, Password, LoginType, RegTime, Roles) values(@UserName, @Email, @Password, 0, @RegTime, '2');select LAST_INSERT_ID();";
#endif
            var q = this.Database.SqlQuery<int>(sql,
                new MySqlParameter("@UserName", username),
                new MySqlParameter("@Email", email), new MySqlParameter("@Password", password),
                new MySqlParameter("@RegTime", regTime));
            return q.FirstOrDefault();
        }

        /// <summary>
        /// Insert a new local register user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="regTime"></param>
        /// <returns></returns>
        public int RegisterNewUser(string email, string password, DateTime regTime)
        {
            password = EncryptUtil.GetHashPwd(password);
#if TEST
            string sql="insert into TestUserItems(UserName, Email, Password, LoginType, RegTime, Roles) values(@Email, @Email, @Password, 0, @RegTime, '2');select LAST_INSERT_ID();";
#else
            string sql="insert into UserItems(UserName, Email, Password, LoginType, RegTime, Roles) values(@Email, @Email, @Password, 0, @RegTime, '2');select LAST_INSERT_ID();";
#endif
            var q = this.Database.SqlQuery<int>(sql,
                new MySqlParameter("@Email", email), new MySqlParameter("@Password", password),
                new MySqlParameter("@RegTime", regTime));
            return q.FirstOrDefault();
        }

        /// <summary>
        /// Insert a new local register user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="regTime"></param>
        /// <returns></returns>
        public int RegisterNewWeixinUser(string username, DateTime regTime)
        {
#if TEST
            string sql="insert into TestUserItems(UserName, LoginType, RegTime, Roles) values(@UserName, 2, @RegTime, '2');select LAST_INSERT_ID();";
#else
            string sql = "insert into UserItems(UserName, LoginType, RegTime, Roles) values(@UserName, 2, @RegTime, '2');select LAST_INSERT_ID();";
#endif
            var q = this.Database.SqlQuery<int>(sql,
                new MySqlParameter("@UserName", username),
                new MySqlParameter("@RegTime", regTime));
            return q.FirstOrDefault();
        }

        /// <summary>
        /// Insert a new QQ register user.
        /// </summary>
        /// <param name="qqemail"></param>
        /// <param name="password"></param>
        /// <param name="regTime"></param>
        /// <returns></returns>
        public int RegisterNewQQUser(string username, string qqemail, string password, DateTime regTime)
        {
            password = EncryptUtil.GetHashPwd(password);
#if TEST
            string sql="insert into TestUserItems(UserName, Email, Password, LoginType, RegTime, Roles) values(@UserName, @Email, @Password, 0, @RegTime, '2');select LAST_INSERT_ID();";
#else
            string sql="insert into useritems(UserName, Email, Password, LoginType, RegTime, Roles) values(@UserName, @Email, @Password, 0, @RegTime, '2');select LAST_INSERT_ID();";
#endif
            var q = this.Database.SqlQuery<int>(sql,
                new MySqlParameter("@UserName", username),
                new MySqlParameter("@Email", qqemail), new MySqlParameter("@Password", password),
                new MySqlParameter("@RegTime", regTime));
            return q.FirstOrDefault();
        }

        /// <summary>
        /// Update the user password
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool UpdateUserPassword(int userID, string newPassword)
        {
            newPassword = EncryptUtil.GetHashPwd(newPassword);
#if TEST
            string sql = "update UserItems set PASSWORD=@pwd where userid=@userid";
#else
            string sql = "update UserItems set PASSWORD=@pwd where userid=@userid";
#endif
            return this.Database.ExecuteSqlCommand(sql,
                new MySqlParameter("@pwd", newPassword), new MySqlParameter("@userid", userID)) > 0;
        }

        /// <summary>
        /// Clear the user's cart
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool ClearCartForUser(int userID)
        {
#if TEST
            string sql="delete from TestCartItems where UserID=@userid";
#else
            string sql="delete from cartitems where UserID=@userid";
#endif
            return this.Database.ExecuteSqlCommand(sql, new MySqlParameter("@userid", userID)) > 0;
        }
    }
}
