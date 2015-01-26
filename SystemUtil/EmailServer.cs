using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Solemart.SystemUtil
{
    public class EmailServer
    {
        /// <summary>
        /// 发送密码重置邮件
        /// </summary>
        /// <param name="email"></param>
        public static void SendEmail(string email, string title, string content)
        {
            string from = "account@moborobo.com";

            SmtpClient client = new SmtpClient("mail.moborobo.com");
            client.Credentials = new NetworkCredential("account@moborobo.com", "moborobo001");
            //client.EnableSsl = true;

            client.Send(from, email, title, content);
        }

        /// <summary>
        /// 根据email地址，获取该email的链接
        /// </summary>
        /// <param name="email"></param>
        public static string GetMailLink(string email)
        {
            string suffix = email.Substring(email.IndexOf('@') + 1);
            if (suffix == "gmail.com")
                return "https://mail.google.com/mail/";
            if (suffix == "hotmail.com")
                return "https://mail.live.com/";

            return string.Format("http://mail.{0}", email.Substring(email.IndexOf('@') + 1));
        }
    }
}
