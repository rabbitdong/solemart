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
        static void Main(string[] args)
        {
            if (args.Length < 1 || args[0] == "/?" || args[0] == "/h")
            {
                Console.WriteLine("usage: usertool [options] parameters");
                return;
            }

            if (args[0] == "/a" && args.Length < 4)
            {
                Console.WriteLine("usage: usertool /a username, email, password");
                return;
            }

            string connectionString = EncryptUtil.DecryptString(ConfigurationManager.ConnectionStrings["solemart-mysql"].ConnectionString);

            MySqlConnection connection = new MySqlConnection(connectionString);

            string createUserSql = "insert into useritems(UserName, Email, Password, LoginType, RegTime, Roles) values(@UserName, @Email, @Password, 0, @RegTime, '4')";
            MySqlParameter[] parameters = new MySqlParameter[4];
            parameters[0] = new MySqlParameter("@UserName", args[1]);
            parameters[1] = new MySqlParameter("@Email", args[2]);
            parameters[2] = new MySqlParameter("@Password", EncryptUtil.GetHashPwd(args[3]));
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
    }
}
