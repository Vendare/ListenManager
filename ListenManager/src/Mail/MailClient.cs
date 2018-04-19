using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using ListenManager.Config;
using ListenManager.Database.DataObjects;

namespace ListenManager.Mail
{
    public class MailClient
    {
        private static MailClient _instance;

        public static MailClient Instance => _instance ?? (_instance = new MailClient());
        private readonly ConfigHandler _configHandler;

        public List<VereinsMitglied> MailToListe { get; set; }
        public string Betreff { get; set; }
        public string Message { get; set; }
        public List<FileInfo> Anhaenge { get; set; }

        private MailClient()
        {
            _configHandler = ConfigHandler.Instance;
        }

        private MailMessage CreateMail()
        {
            var mail = new MailMessage();

            foreach (var member in MailToListe)
            {
                mail.To.Add(member.Email);
            }

            mail.From = new MailAddress(_configHandler.SmtpUser, "Kanninchenzucht Verein H5"); 

            mail.Subject = Betreff;

            mail.Body = Message;

            foreach (var file in Anhaenge)
            {
                mail.Attachments.Add(new Attachment(file.FullName) { Name = file.Name });
            }

            return mail;
        }

        public async void SendMessageAsync()
        {
            var message = CreateMail();
            var pw = _configHandler.SmtpPassword;

            var tmp = _configHandler.SmtpAdress.Split(':');
            var adress = tmp[0];
            var port = tmp.Length > 1 ? Convert.ToInt32(tmp[1]) : 25; //25 is the base smtp port

            var client = new SmtpClient
            {
                Host = adress,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential()
                {
                    UserName = _configHandler.SmtpUser,
                    SecurePassword = pw
                }
            };

            await client.SendMailAsync(message);
            pw.Dispose();
        }
    }
}
