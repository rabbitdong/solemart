using System;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solemart.DataProvider;
using System.Linq;
using MySql.Data.MySqlClient;
using Solemart.DataProvider.Entity;

namespace Solemart.UnitTest
{
    [TestClass]
    public class TestDataProvider
    {
        [TestMethod]
        public void DbInitializeTest()
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {   
                Assert.IsNotNull(context.AdviserItems);
                Assert.IsNotNull(context.BrandItems);
                Assert.IsNotNull(context.BulletinItems);
                Assert.IsNotNull(context.CartItems);
                Assert.IsNotNull(context.CategoryItems);
                Assert.IsNotNull(context.FavoriteItems);
                Assert.IsNotNull(context.InStockItems);
                Assert.IsNotNull(context.OrderDetailItems);
                Assert.IsNotNull(context.OrderItems);
                Assert.IsNotNull(context.PriceHistoryItems);
                Assert.IsNotNull(context.ProductCommentItems);
                Assert.IsNotNull(context.ProductImageItems);
                Assert.IsNotNull(context.ProductItems);
                Assert.IsNotNull(context.SaledProductItems);
                Assert.IsNotNull(context.SendAddressItems);
                Assert.IsNotNull(context.UserAppendInfoItems);
                Assert.IsNotNull(context.UserItems);
                Assert.IsNotNull(context.VendorItems);
            }
        }

        public void VendorItemTest()
        {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                context.ClearTableData(typeof(VendorItem));
                VendorItem vendor = new VendorItem { VendorName = "vendor_01", Address = "address_01", ContactName = "测试名", Evaluation = "", ExtContent = "", VendorEmail = "test@email.com", VendorPhone = "88888888", VendorUrl = "http://test.com" };
                context.VendorItems.Add(vendor);
                Assert.IsTrue(context.SaveChanges() > 0);
                vendor = context.VendorItems.Find(1);
                Assert.IsNotNull(vendor);
                Assert.AreEqual<string>("vendor_01", vendor.VendorName);
                Assert.AreEqual<string>("测试名", vendor.ContactName);
                context.ClearTableData(typeof(VendorItem));
                Assert.AreEqual<int>(0, context.VendorItems.Count());
            }
        }
    }
}
