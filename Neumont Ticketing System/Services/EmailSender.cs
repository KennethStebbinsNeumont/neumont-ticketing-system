using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using Neumont_Ticketing_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Neumont_Ticketing_System.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly IWebHostEnvironment _env;

        public EmailSender(
            IOptions<EmailSettings> emailSettings,
            IWebHostEnvironment env)
        {
            _emailSettings = emailSettings.Value;
            _env = env;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));

                message.To.Add(new MailboxAddress(email));

                message.Subject = subject;

                message.Body = new TextPart("html")
                {
                    Text = htmlMessage
                };

                using (var client = new SmtpClient())
                {
                    // TODO: Don't accept just any certificate
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    if(_env.IsDevelopment())
                    {
                        await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, true);
                    } else
                    {
                        await client.ConnectAsync(_emailSettings.MailServer);
                    }

                    await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);

                    await client.SendAsync(message);

                    await client.DisconnectAsync(true);
                }
            } catch (Exception ex)
            {
                // TODO: Handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
