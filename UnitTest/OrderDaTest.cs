using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xxx.EntityLib;
using Xxx.DataAccessLib;

namespace UnitTest {
    [TestClass]
    public class OrderDaTest {
        private OrderDA prod_da = null;

        [TestInitialize]
        public void CreateDBConnection() {
            SystemConnection.ConnectString = "server=localhost;user id=root;pwd=;database=ILiveMini;";
            prod_da = OrderDA.Instance;
        }

        [TestMethod]
        public void TestSaveOrderInfo() {
            OrderItem oi = new OrderItem();            
        }
    }
}
