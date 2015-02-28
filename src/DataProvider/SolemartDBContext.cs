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
        #endregion

        public bool ClearTableData(Type tableType)
        {
            string sql = string.Format("truncate table {0}s", tableType.Name.ToLower());
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
            var q = this.Database.SqlQuery<int>("select count(1) from useritems where username=@un and PASSWORD=@pwd",
                new MySqlParameter("@un", username), new MySqlParameter("@pwd", password));
            return q.FirstOrDefault() > 0;
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
            return this.Database.ExecuteSqlCommand("update useritems set PASSWORD=@pwd where userid=@userid",
                new MySqlParameter("@pwd", newPassword), new MySqlParameter("@userid", userID)) > 0;
        }

        /// <summary>
        /// Clear the user's cart
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool ClearCartForUser(int userID)
        {
            return this.Database.ExecuteSqlCommand("delete cartitems where UserID=@userid", new MySqlParameter("@userid", userID)) > 0;
        }
    }
}
