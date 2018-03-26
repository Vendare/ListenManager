using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ListenManager.Database.DataObjects;
using ListenManager.Database.Handlers;
using ListenManager.Enums;
using ListenManager.Mail;

namespace ListenManager.Managers
{
    public class MailViewManager : BaseManager
    {
        private readonly VerzeichnisHandler _handler;

        private ObservableCollection<VereinsMitglied> _mitgliederOhneEmail;
        private ObservableCollection<MitgliedsListe> _listen;
        private ObservableCollection<MailAttachment> _attachments;
        private MitgliedsListe _selectedListe;
        private string _subject;
        private string _body;

        private List<VereinsMitglied> _mailListe;

        public MailViewManager()
        {
            _handler = VerzeichnisHandler.Instance;
            var alle = new MitgliedsListe() { Name = "Alle", SourceVerzeichnis = null, Type = ListType.Alle };
            var erwa = new MitgliedsListe() { Name = "Erwachsene", SourceVerzeichnis = null, Type = ListType.Erwachsene };
            var jugd = new MitgliedsListe() { Name = "Jugend", SourceVerzeichnis = null, Type = ListType.Jugend };
            Listen = _handler.GetAllVerzeichnisse();
            Listen.Insert(0, alle);
            Listen.Insert(1, erwa);
            Listen.Insert(2, jugd);

            SelectedListe = alle;
        }

        public ObservableCollection<MailAttachment> Attachments
        {
            get => _attachments;
            set
            {
                _attachments = value;
                OnPropertyChanged(nameof(Attachments));
            }
        }

        public ObservableCollection<VereinsMitglied> MitgliederOhneEmail
        {
            get => _mitgliederOhneEmail;
            set
            {
                _mitgliederOhneEmail = value;
                OnPropertyChanged(nameof(MitgliederOhneEmail));
            }
        }

        public ObservableCollection<MitgliedsListe> Listen
        {
            get => _listen;
            set
            {
                _listen = value;
                OnPropertyChanged(nameof(Listen));
            }
        }

        public MitgliedsListe SelectedListe
        {
            get => _selectedListe;
            set
            {
                _selectedListe = value;
                LoadMitglieder();
                OnPropertyChanged(nameof(SelectedListe));
            }
        }

        public string Subject
        {
            get => _subject;
            set
            {
                _subject = value;
                OnPropertyChanged(nameof(Subject));
            }
        }

        public string Body
        {
            get => _body;
            set
            {
                _body = value;
                OnPropertyChanged(nameof(Body));
            }
        }

        private void LoadMitglieder()
        {
            if (SelectedListe.Type == ListType.UserCreated)
            {
                var tmp = _handler.GetMitgliederInVerzeichnis(SelectedListe.SourceVerzeichnis.ID);
                SetupMitgliederListen(tmp);
            }
            else
            {
                ObservableCollection<VereinsMitglied> tmp;
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (SelectedListe.Type)
                {
                        
                    case ListType.Jugend:
                        tmp = _handler.GetJugend();
                        SetupMitgliederListen(tmp);
                        break;
                    case ListType.Erwachsene:
                        tmp = _handler.GetErwachsene();
                        SetupMitgliederListen(tmp);
                        break;
                    case ListType.Alle:
                        tmp = _handler.GetAllMitglieder();
                        SetupMitgliederListen(tmp);
                        break;
                }
            }
        }

        private void SetupMitgliederListen(IReadOnlyCollection<VereinsMitglied> tmp)
        {
            var withoutMail = (from m in tmp
                where m.Email.Equals(string.Empty)
                select m).ToList();

            var data = new ObservableCollection<VereinsMitglied>();

            foreach (var m in withoutMail)
            {
                data.Add(m);
            }

            MitgliederOhneEmail = data;

            _mailListe = (from m in tmp
                where !m.Email.Equals(string.Empty)
                select m).ToList();
        }

        public void SendMail()
        {
            var client = MailClient.Instance;

            client.Anhaenge = (from m in _attachments select m.AttachmentFileInfo).ToList();
            client.Betreff = Subject;
            client.MailToListe = _mailListe;
            client.Message = Body;

            client.SendMessageAsync();
        }
    }
}