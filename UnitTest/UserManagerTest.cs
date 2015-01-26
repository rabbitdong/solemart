using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xxx.EntityLib;
using Xxx.BusinessLib;
using Xxx.DataAccessLib;

namespace UnitTest {
    [TestClass]
    public class UserManagerTest {
        [TestInitialize]
        public void CreateDBConnection() {
            SystemConnection.ConnectString = "server=localhost;user id=root;pwd=;database=ilivemini;";
        }

        /// <summary>空密码的登录测试
        /// </summary>
        [TestMethod]
        public void TestOnLogin() {
            User user = UserManager.Instance.OnLogin("nullpwd", "");
            Assert.IsNotNull(user);
        }
    }
}
