using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ListenManager.Config;
using ListenManager.Database.DataObjects;
using ListenManager.Database.Handlers;
using ListenManager.Enums;
using ListenManager.Mail;
using Microsoft.Win32;

namespace ListenManager.Managers
{
    public class MailViewManager : BaseManager
    {
        private readonly VerzeichnisHandler _handler;
        private readonly ConfigHandler _config;

        private ObservableCollection<VereinsMitglied> _mitgliederOhneEmail;
        private ObservableCollection<MitgliedsListe> _listen;
        private ObservableCollection<MailAttachment> _attachments;
        private MitgliedsListe _selectedListe;
        private MailAttachment _selectedAttachment;
        private string _subject;
        private string _body;

        private ICommand _addAttachmentCommand;
        private ICommand _deleteAttachmentCommand;
        private ICommand _clearUserInputCommand;
        private ICommand _sendMailCommand;
        private ICommand _showAttachmentFileCommand;

        private List<VereinsMitglied> _mailListe;

        public MailViewManager()
        {
            Attachments = new ObservableCollection<MailAttachment>();
            _handler = VerzeichnisHandler.Instance;
            _config = ConfigHandler.Instance;
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

        public MailAttachment SelectedAttachment
        {
            get => _selectedAttachment;
            set
            {
                _selectedAttachment = value;
                OnPropertyChanged(nameof(SelectedAttachment));
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

        public ICommand AddAttachmetCommand =>
            _addAttachmentCommand ?? (_addAttachmentCommand = new RelayCommand(AddAttachment));

        public ICommand DeleteAttachmetCommand =>
            _deleteAttachmentCommand ?? (_deleteAttachmentCommand = new RelayCommand(DeleteAttachment));

        public ICommand ClearUserInputCommand =>
            _clearUserInputCommand ?? (_clearUserInputCommand = new RelayCommand(ClearUserInput));

        public ICommand ShowAttachmentFileCommand =>
            _showAttachmentFileCommand ?? (_showAttachmentFileCommand = new RelayCommand(ShowAttachmentFile));

        public ICommand SendMailCommand => _sendMailCommand ?? (_sendMailCommand = new RelayCommand(SendMail));

        private void ClearUserInput()
        {
            Subject = string.Empty;
            Body = string.Empty;
            SelectedAttachment = null;
            Attachments.Clear();
        }

        private void DeleteAttachment()
        {
            Attachments.Remove(SelectedAttachment);
            SelectedAttachment = Attachments.FirstOrDefault();
        }

        private void AddAttachment()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() != true) return;

            var selectedAttachments = dialog.FileNames;
            foreach (var filePath in selectedAttachments)
            {
                var attachment = new MailAttachment()
                {
                    AttachmentFileInfo = new FileInfo(filePath)
                };
                Attachments.Add(attachment);
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

        private void ShowAttachmentFile()
        {
            var processStartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                FileName = SelectedAttachment.AttachmentFileInfo.FullName,
            };
            Process.Start(processStartInfo);
        }

        private void SendMail()
        {
            var client = MailClient.Instance;

            client.Anhaenge = (from m in _attachments select m.AttachmentFileInfo).ToList();
            client.Betreff = Subject;
            client.MailToListe = _mailListe;
            client.Message = Body;

            client.SendMessage();
        }
    }
}