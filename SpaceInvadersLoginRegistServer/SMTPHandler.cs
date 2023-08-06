using System.Net;
using System.Net.Mail;

namespace LoginRegisterServer
{
    /// <summary>
    /// Simple Mail Transfer Protocol implementation 
    /// </summary>
    internal static class SMTPHandler
    {
        private const string SMTPEmailAddress = "Email goes here";
        private const string SMTPEmailPassword = "Password goes here";

        /// <summary>
        /// Sends an email from an hardcoded account
        /// </summary>
        /// <param name="email"> The recipient's email address </param>
        /// <param name="title"> The title of the message </param>
        /// <param name="content"> The content of the message </param>
        public static void SendEmail(string email, string title, string content)
        {
            MailMessage message = new MailMessage();

            message.From = new MailAddress(SMTPEmailAddress);
            message.To.Add(new MailAddress(email));

            message.Subject = title;
            message.Body = content;

            SmtpClient client = new SmtpClient();

            client.Host = "smtp.gmail.com";
            client.Port = 587;

            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(SMTPEmailAddress, SMTPEmailPassword);

            client.EnableSsl = true;

            client.Send(message);
        }
    }
}