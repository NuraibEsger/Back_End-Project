using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
namespace BackEndProject.Services
{
    public class SendEmail
    {
        private readonly IConfiguration _configuration;

        public SendEmail(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> EmailSend(string userEmail, string confirmationLink)
        {
            bool status = false;
            try
            {
                string hostAddress = _configuration["smtpSettings:Host"]!;
                bool enableSsl = _configuration.GetValue<bool>("smtpSettings:EnableSsl");
                string fromEmailId = _configuration["EmailSettings:FromAddress"]!;
                bool enableSslEmail = _configuration.GetValue<bool>("EmailSettings:EnableSsl");
                string password = _configuration["EmailSettings:Password"]!;
                string port = _configuration["EmailSettings:Port"]!;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(fromEmailId);
                mailMessage.To.Add(new MailAddress(userEmail));

                mailMessage.Subject = "Confirm your email";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = confirmationLink;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = hostAddress;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(mailMessage.From.Address, password);
                smtp.Port = Convert.ToInt32(port);


                NetworkCredential networkCredential = new NetworkCredential();

                networkCredential.UserName = mailMessage.From.Address;
                networkCredential.Password = password;


                await smtp.SendMailAsync(mailMessage);

                status = true;
                return status;
            }
            catch (Exception e)
            {
                return status;
            }
        }
    }
}
