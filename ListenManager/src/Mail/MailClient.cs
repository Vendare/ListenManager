using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security;
using ListenManager.Database.DataObjects;

namespace ListenManager.Mail
{
    public class MailClient
    {
        private static MailClient _instance;

        public static MailClient Instance => _instance ?? (_instance = new MailClient());

        public List<VereinsMitglied> MailToListe { get; set; }
        public string Betreff { get; set; }
        public string Message { get; set; }
        public List<FileInfo> Anhaenge { get; set; }
        public string MailServerAdress { get; set; }
        public string User { get; set; }
        public SecureString Passwort { get; set; }

        private MailClient()
        {

        }

        private MailMessage CreateMail()
        {
            var mail = new MailMessage();

            foreach (var member in MailToListe)
            {
                mail.To.Add(member.Email);
            }

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

            var client = new SmtpClient
            {
                Host = MailServerAdress,
                EnableSsl = true,
                Credentials = new NetworkCredential()
                {
                    UserName = User,
                    SecurePassword = Passwort
                }
            };

            await client.SendMailAsync(message);
        }
    }
}
