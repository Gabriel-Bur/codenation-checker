using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using codenation.checker.Api.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.Configuration;
using codenation.checker.Api.Models;
using codenation.checker.Api.Context;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using codenation.checker.Api.Domain;

namespace codenation.checker.Api.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseContext _context;
        private readonly AppSettings _appSettings;

        public EmailSenderService(
            IConfiguration configuration,
            DatabaseContext context)
        {
            _context = context;
            _configuration = configuration;
            _appSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();

        }

        public async Task SendEmailToAll()
        {
            try
            {
                var email = await BuildEmail();

                using var client = new SmtpClient();
                client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate(_appSettings.EmailSender, _appSettings.EmailPassword);

                await client.SendAsync(email);

                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<MimeMessage> BuildEmail()
        {

            var listOfEmails = await GetEmails();
            var message = new MimeMessage();
            
            message.From.Add(new MailboxAddress("Codenation", "mailtosender10@gmail.com"));

            message.To.AddRange(listOfEmails.Select(x =>
            {
                return new MailboxAddress("Dev", x.Email);
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

        private async Task<List<CodenationUser>> GetEmails()
        {
            return await _context.CodenationUsers.AsNoTracking().ToListAsync();
        }

    }
}
