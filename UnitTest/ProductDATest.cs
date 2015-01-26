using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xxx.DataAccessLib;
using Xxx.EntityLib;
using System.Collections.Generic;

namespace UnitTest {
    [TestClass]
    public class TestMysqlConnection {
        private ProductDA prod_da = null;

        [TestInitialize]
        public void CreateDBConnection() {
            SystemConnection.ConnectString = "server=localhost;user id=root;pwd=;database=ILiveMini;";
            prod_da = ProductDA.Instance;
        }

        [TestMethod]
        public void TestTotalProductCount() {
            Assert.AreEqual<int>(1, prod_da.TotalProductCount);
        }

        [TestMethod]
        public void TestCategories() {
            Assert.AreEqual<int>(prod_da.AddNewCategory("电子类", "电子产品", 0), 1);
            Assert.AreEqual<int>(prod_da.AddNewCategory("手机配件", "手机配件产品，如手机贴膜、移动电源等", 1), 2);

            /*
            Assert.AreEqual<bool>(prod_da.AddNewCategory("Toy", 0), true);

            Assert.AreEqual<bool>(prod_da.AddNewCategory("新能源", 1), true);
            Assert.AreEqual<bool>(prod_da.AddNewCategory("BeiBei", 2), true);
            Assert.AreEqual<bool>(prod_da.AddNewCategory("Specal New Energy", 3), true);
            Assert.AreEqual<bool>(prod_da.AddNewCategory("Green Batt", 1), true);
            */
            /*
            List<Category> categories = prod_da.GetCategoryList();
            Assert.AreEqual<int>(categories.Count, 6);
            Assert.AreEqual<string>(categories[0].CateName, "电子类");
            Assert.AreEqual<int>(categories[0].SubCategories.Count, 2);
            Assert.AreEqual<string>(categories[1].CateName, "Toy");
            Assert.IsNull(categories[1].SupCategory);
             */
        }

        [TestMethod]
        public void TestChangeCateToOtherSuperCate() {
            Assert.IsTrue(prod_da.ChangeCateToOtherSuperCate(3, 1));
        }

        [TestMethod]
        public void TestGetTopNProduct() {
            List<Product> products = prod_da.GetTopNProducts(2);
            Assert.AreEqual<int>(1, products.Count);
            Assert.AreEqual<string>("章鱼手机垫", products[0].Name);
            Assert.AreEqual<int>(products[0].OwnedCategory.CateID, 3);
        }

        [TestMethod]
        public void TestInStock() {
            Assert.AreEqual<int>(prod_da.InStockNewProduct("test1", 0, 0, "plastic", 0, 5.8m, 10, "件"), 2);
        }

        [TestMethod]
        public void TestGetLastStockPrice() {
            Assert.AreEqual<decimal>(prod_da.GetLastStockPrice(2), 73.00m);
            Assert.AreEqual<decimal>(prod_da.GetLastStockPrice(10), 0.0m);
        }
    }
}
