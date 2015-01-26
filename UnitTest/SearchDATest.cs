using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xxx.DataAccessLib;

namespace UnitTest {
    [TestClass]
    public class SearcherDATest {
        [TestInitialize]
        public void InitializeConnection() {
            SystemConnection.ConnectString = "server=localhost;user id=root;pwd=;database=ilivemini;";
        }

        [TestMethod]
        public void TestSaveSearchResult() {
            SearcherDA sda = SearcherDA.Instance;
            Assert.IsTrue(sda.SaveSearchResult("abc", "3,5"));
        }

        [TestMethod]
        public void TestGetProductIdsByKey(){
            SearcherDA sda = SearcherDA.Instance;
            Assert.IsTrue(sda.SaveSearchResult("abc", "3,5"));
            int[] ids = sda.GetProductIdsByKey("e");
            Assert.IsNull(ids);

            ids = sda.GetProductIdsByKey("abc");
            Assert.AreEqual<int>(ids.Length, 2);
            Assert.AreEqual<int>(ids[0], 3);
            Assert.AreEqual<int>(ids[1], 5);
        }
    }
}
