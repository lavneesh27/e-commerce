using System.Net.Mail;
using System.Net;
using Org.BouncyCastle.Security;

namespace WebApplication1.Models.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private string _otp;
        public string GenerateRandomOTP(int iOTPLength, string[] saAllowedCharacters)
        {
            string sOTP = String.Empty;

            string sTempChars = String.Empty;

            Random rand = new Random();

            for (int i = 0; i < iOTPLength; i++)

            {
                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];

                sOTP += sTempChars;
            }

            return sOTP;
        }

        public Boolean VerifyOTP(string otp)
        {
            if(otp != null && otp == _otp)
            {
                _otp = string.Empty;
                return true;
            }
            return false;
        }

        Task IEmailSender.SendEmailAsync(string email, string subject, string message)
        {
            var mail = "aspnet-dev@outlook.com";
            var pw = "Outlookpassword@123";

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

            _otp =  GenerateRandomOTP(6, saAllowedCharacters);

            return client.SendMailAsync(
                new MailMessage(from: mail, to: email, subject, message+" "+_otp));
        }
    }
}
