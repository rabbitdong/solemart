using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MySql.Data.MySqlClient;
using Solemart.SystemUtil;

namespace UserTool
{
    class Program
    {
        private static string ConnectionString = EncryptUtil.DecryptString(ConfigurationManager.ConnectionStrings["solemart-mysql"].ConnectionString);

        static void Main(string[] args)
        {
            if (args.Length < 1 || args[0] == "/?" || args[0] == "/h")
            {
                Console.WriteLine("usage: usertool [options] parameters");
                return;
            }

            if (args[0] == "/a" && args.Length == 4)
            {
                CreateAdminAccount(args[1], args[2], args[3]);
                return;
            }
            else if (args[0] == "/s")
            {
                ShowAdminAccount();
                return;
            }

            Console.WriteLine("usage: usertool /a[s] username, email, password");
        }

        private static void CreateAdminAccount(string username, string email, string password)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);

#if TEST
            string createUserSql = "insert into TestUserItems(UserName, Email, Password, LoginType, RegTime, Roles) values(@UserName, @Email, @Password, 0, @RegTime, '4')";
#else
            string createUserSql = "insert into UserItems(UserName, Email, Password, LoginType, RegTime, Roles) values(@UserName, @Email, @Password, 0, @RegTime, '4')";
#endif
            MySqlParameter[] parameters = new MySqlParameter[4];
            parameters[0] = new MySqlParameter("@UserName", username);
            parameters[1] = new MySqlParameter("@Email", email);
            parameters[2] = new MySqlParameter("@Password", EncryptUtil.GetHashPwd(password));
            parameters[3] = new MySqlParameter("@RegTime", DateTime.Now);

            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(createUserSql, connection);
                cmd.Parameters.AddRange(parameters);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    Console.WriteLine("create the admin user successfully!");
                }
                else
                {
                    Console.WriteLine("create the admin user failed!");
                }
            }
            finally
            {
                connection.Close();
            }
        }

        private static void ShowAdminAccount()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
#if TEST
            string queryUserSql = "select * from TestUserItems where Roles='4'";
#else
            string queryUserSql = "select * from UserItems where Roles='4'";
#endif
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(queryUserSql, connection);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("username[{0}]", reader["UserName"]);
                    }
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
