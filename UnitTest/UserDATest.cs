using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xxx.DataAccessLib;

namespace UnitTest {
    [TestClass]
    public class UserDATest {
        private UserDA user_da = null;
        private UserInfoDA ur_da = null;

        [TestInitialize]
        public void CreateDBConnection() {
            SystemConnection.ConnectString = "server=localhost;user id=root;pwd=;database=ilivemini;";
        }

        //[TestMethod]
        //public void TestCreateNewUser() {
        //    int res = user_da.RegisterNewUser("admin", "changepwd", "adon_hua@hotmail.com");
        //    Assert.AreEqual<int>(res, 1);
        //}

        [TestMethod]
        public void TestIsUserNameDuplicate() {
            bool res = user_da.IsUserNameDuplicate("admin");
            Assert.AreEqual<bool>(res, true);
            res = user_da.IsUserNameDuplicate("somebody");
            Assert.AreEqual<bool>(res, false);
        }

        [TestMethod]
        public void TestIsEmailDuplicate() {
            bool res = user_da.IsEmailDuplicate("adon_hua@hotmail.com");
            Assert.AreEqual<bool>(res, true);
            res = user_da.IsEmailDuplicate("somebody@hotmail.com");
            Assert.AreEqual<bool>(res, false);
        }

        [TestMethod]
        public void TestUserLogin() {
            int userid = user_da.Login("admin", "changepwd");
            Assert.AreEqual<int>(userid, 1);
        }
    }
}
