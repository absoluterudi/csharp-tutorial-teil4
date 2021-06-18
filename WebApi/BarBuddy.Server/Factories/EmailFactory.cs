using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;

namespace BarBuddy.Server.Factories
{
    public class EmailFactory
    {
        private readonly ILogger<EmailFactory> _logger;
        private readonly IConfiguration _configuration;

        public EmailFactory(ILogger<EmailFactory> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        #region config

        private string Host
        {
            get { return _configuration["Email:Host"]; }
        }

        private int Port
        {
            get { return _configuration.GetValue<int>("Email:Port"); }
        }

        private string User
        {
            get { return _configuration["Email:User"]; }
        }

        private string Password
        {
            get { return _configuration["Email:Password"]; }
        }

        private bool EnableSsl
        {
            get { return _configuration.GetValue<bool>("Email:EnableSsl"); }
        }

        private string From
        {
            get { return _configuration["Email:From"]; }
        }

        private bool TestMode
        {
            get { return _configuration.GetValue<bool>("Email:TestMode"); }
        }

        private string TestModeRecipients
        {
            get { return _configuration["Email:TestMode_Recipients"]; }
        }

        #endregion

        public bool SendEmail(string recipient, string subject, string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(recipient))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(body))
                {
                    return false;
                }

                var msg = new MailMessage();
                msg.From = new MailAddress(From);

                if (TestMode)
                {
                    foreach (var item in TestModeRecipients.Split(new char[] { ';', ',' }))
                    {
                        msg.To.Add(item);
                    }
                }
                else
                {
                    msg.To.Add(recipient);
                }

                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient(Host, Port);
                smtpClient.EnableSsl = EnableSsl;
                if (!string.IsNullOrWhiteSpace(User))
                {
                    smtpClient.Credentials = new NetworkCredential(User, Password);
                }

                smtpClient.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }
    }
}
