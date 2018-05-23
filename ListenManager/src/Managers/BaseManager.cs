using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ListenManager.Database.DataObjects;
using ListenManager.Database.Handlers;
using ListenManager.Interfaces;
using ListenManager.Managers.Messages;
using ListenManager.Managers.Util;
using ListenManager.Properties;
using MahApps.Metro.Controls.Dialogs;

namespace ListenManager.Managers
{
    public class BaseManager : INotifyPropertyChanged
    {
        protected readonly IDialogCoordinator Coordinator;

        private VereinsMitglied _vereinsMitglied;
        protected static readonly IWindowService WindowService = new WindowService();

        private ICommand _addCommand;
        private ICommand _updateCommand;
        private ICommand _deleteMemberCommand;

        public BaseManager()
        {
            Coordinator = DialogCoordinator.Instance;
        }

        public ICommand AddMemberCommand => _addCommand ?? (_addCommand = new RelayCommand(OpenAddMemberDialog));
        public ICommand UpdateMemberCommand => _updateCommand ?? (_updateCommand = new RelayCommand(OpenEditMemberDialog));

        public ICommand DeleteMemberCommand =>
            _deleteMemberCommand ?? (_deleteMemberCommand = new RelayCommand(DeleteSelectedMember));

        public VereinsMitglied SelectedVereinsMitglied
        {
            get => _vereinsMitglied;
            set
            {
                if (value.Equals(_vereinsMitglied)) return;
                _vereinsMitglied = value;
                OnPropertyChanged(nameof(SelectedVereinsMitglied));
            }
        }

        public ObservableCollection<VereinsMitglied> Mitglieder { get; set; }

        private void OpenEditMemberDialog()
        {
            var shared = SharedProperties.Instance;
            shared.IsEditMemberDialog = true;
            shared.MemberToEdit = SelectedVereinsMitglied;
            WindowService.ShowDialog("EditMemberPage");
        }

        private void OpenAddMemberDialog()
        {
            var shared = SharedProperties.Instance;
            shared.IsEditMemberDialog = false;
            shared.MitgliederCollection = Mitglieder;
            WindowService.ShowDialog("EditMemberPage");
        }

        private async void DeleteSelectedMember()
        {
            var dialogResult = await Coordinator.ShowMessageAsync(this,"Warnung","Sind sie sicher das sie dieses Mitglied entgültig löschen möchten ?", MessageDialogStyle.AffirmativeAndNegative);
            if (dialogResult == MessageDialogResult.Negative) return;

            var handler = VerzeichnisHandler.Instance;
            handler.DeleteMitglied(SelectedVereinsMitglied.SourceMitglied);

            Messenger.Default.Send(new ReloadMemberMessage { ReloadAllMembers = true, ReloadBirthdays = true });
            Messenger.Default.Send(new UpdateListMessage { ReloadAllData = true, IdOfItemToUpdate = -1});
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
