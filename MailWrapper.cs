using System.Net.Mail;
using System.Net;

namespace homefinderYad2
{
    public class MailWrapper
    {
        private SmtpClient _mailer;

        public string subject = "";
        public string to = "your_personal_email@gmail.com";
        public string host = "smtp.gmail.com";
        public string username = "yourUserName";
        public string password = "password";
        public string from = "youremail@gmail.com";
        public MailWrapper()
        {
            bool isGmail = host.Contains("gmail");
            _mailer = new SmtpClient(host, isGmail ? 587 : 443);
            _mailer.Credentials = new NetworkCredential(username, password);
            if (isGmail)
            {
                _mailer.EnableSsl = true;
            }
        }
        public MailWrapper(string smtpaddress)
        {
            _mailer = new SmtpClient(smtpaddress);
        }
        public MailWrapper(string smtpaddress, int port) : this(smtpaddress)
        {
            _mailer.Port = port;
        }
        public MailWrapper(string smtpaddress, int port, string username, string password) : this(smtpaddress, port)
        {
            _mailer.Credentials = new NetworkCredential(username, password);
        }
        public void sendMail(Home home)
        {
            string body = createBody(home);
            string subject = "new home in " + home.Neighborhood;
            MailMessage msg = new MailMessage(from, to, subject, body);
            msg.IsBodyHtml = true;

            _mailer.Send(msg);
        }
        private string createBody(Home home)
        {
            return @"new home in " + home.Neighborhood + ".Address:" + home.Street + ".Price:" + home.Price + ". Link:" + new yad2Layer().getHouseUrl(home);
        }
    }
}
