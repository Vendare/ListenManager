using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using ListenManager.Config;
using ListenManager.Database.DataObjects;
using MahApps.Metro.Controls.Dialogs;

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

            var modMessage = Message.Replace("color:#FFFFFF", "color:#000000");

            mail.Body = modMessage;
            mail.IsBodyHtml = Message.StartsWith("<HTML");

            foreach (var file in Anhaenge)
            {
                mail.Attachments.Add(new Attachment(file.FullName) { Name = file.Name });
            }

            return mail;
        }

        public async void SendMessage()
        {
            var message = CreateMail();
            var pw = _configHandler.SmtpPassword;

            var tmp = _configHandler.SmtpAdress.Split(':');
            var adress = tmp[0];
            var port = tmp.Length > 1 ? Convert.ToInt32(tmp[1]) : 25; //25 is the base smtp port

            using (var client = new SmtpClient
            {
                Host = adress,
                Port = port,
                EnableSsl = true,
                DeliveryFormat = SmtpDeliveryFormat.International,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000,
                Credentials = new NetworkCredential()
                {
                    UserName = _configHandler.SmtpUser,
                    SecurePassword = pw
                }
            })

            {
                try
                {
                    client.Send(message);
                }
                catch (SmtpException smtpException)
                {
                    var dialog = DialogCoordinator.Instance;
                    await dialog.ShowMessageAsync(this, "Fehler",
                        "Es ist ein Fehler beim übertragen der E-Mail aufgetreten.\n\n Original Fehlermeldung :\n" +
                        smtpException.Message);
                }
                catch (Exception e)
                {
                    var dialog = DialogCoordinator.Instance;
                    await dialog.ShowMessageAsync(this, "Fehler",
                        "Das senden.\n\n Original Fehlermeldung :\n" +
                        e.Message);
                }
                
            }


            pw.Dispose();
        }
    }
}
