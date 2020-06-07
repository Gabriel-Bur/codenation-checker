using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using codenation.checker.Api.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Extensions.Configuration;
using codenation.checker.Api.Models;

namespace codenation.checker.Api.Services
{
    public class EmailSenderService : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSenderService> _logger;
        private readonly AppSettings _appSettings;

        public EmailSenderService(
            ILogger<EmailSenderService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _appSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();

        }

        public async Task SendEmailTo(List<string> sendTo)
        {
            try
            {
                var email = BuildEmail(sendTo);

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                    client.Authenticate(_appSettings.EmailSender, _appSettings.EmailPassword);
                    
                    await client.SendAsync(email);

                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private MimeMessage BuildEmail(List<string> addressList)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Codenation", "mailtosender10@gmail.com"));

            message.To.AddRange(addressList.Select(x =>
            {
                return new MailboxAddress("Dev", x);
            }));


            message.Body = new TextPart("plain")
            {
                Text = @"Fala Dev,
                    Um novo modulo do codenation foi liberado, Confere lá:
                    https://www.codenation.dev/dashboard/

                    Teste"
            };

            return message;
        }

    }
}
