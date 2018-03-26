using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using ListenManager.Database.DataObjects;
using ListenManager.Database.Handlers;
using ListenManager.src.Managers.Messages;

namespace ListenManager.Managers
{
    public class MainWindowManager : BaseManager
    {
        private readonly VerzeichnisHandler _handler;
        private ObservableCollection<VereinsMitglied> _birthdayMitglieder;
        public SolidColorBrush CurrentBorderBrush { get; set; }

        public ObservableCollection<VereinsMitglied> BirthdayMitglieder
        {
            get => _birthdayMitglieder;
            set
            {
                _birthdayMitglieder = value;
                OnPropertyChanged(nameof(BirthdayMitglieder));
            }
        }

        public MainWindowManager()
        {
            _handler = VerzeichnisHandler.Instance;
            CurrentBorderBrush = SystemColors.WindowBrush;
            BirthdayMitglieder = _handler.GetMitgliederWithBirthdayForCurrentMonth();
            Messenger.Default.Register<ReloadMemberMessage>(this, OnHandleReloadMessage);
        }

        private void OnHandleReloadMessage(ReloadMemberMessage msg)
        {
            if (msg.ReloadBirthdays)
            {
                BirthdayMitglieder = _handler.GetMitgliederWithBirthdayForCurrentMonth();
            }
        }
    }
}
