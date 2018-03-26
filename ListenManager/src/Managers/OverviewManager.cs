using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ListenManager.Database.DataObjects;
using ListenManager.Database.Handlers;
using ListenManager.Interfaces;
using ListenManager.Managers.Messages;
using ListenManager.Managers.Util;

namespace ListenManager.Managers
{
    internal class OverviewManager : BaseManager
    {
        private ICommand _addCommand;
        private ICommand _updateCommand;
        private VereinsMitglied _vereinsMitglied;
        private readonly IWindowService _windowService;
        private readonly VerzeichnisHandler _handler;

       public OverviewManager()
        {
            _handler = VerzeichnisHandler.Instance;
            Mitglieder = _handler.GetAllMitglieder();
            _windowService = new WindowService();
            Messenger.Default.Register<ReloadMemberMessage>(this, OnHandleReloadMessage);
        }
        
        public ICommand AddCommand => _addCommand ?? (_addCommand = new RelayCommand(OpenAddMemberDialog));
        public ICommand UpdateCommand => _updateCommand ?? (_updateCommand = new RelayCommand(OpenEditMemberDialog));

        public ObservableCollection<VereinsMitglied> Mitglieder { get; set; }

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

        private void OpenEditMemberDialog()
        {
            var shared = SharedProperties.Instance;
            shared.IsEditMemberDialog = true;
            shared.MemberToEdit = SelectedVereinsMitglied;
            _windowService.ShowDialog("EditMemberPage");
        }

        private void OpenAddMemberDialog()
        {
            var shared = SharedProperties.Instance;
            shared.IsEditMemberDialog = false;
            shared.MitgliederCollection = Mitglieder;
            _windowService.ShowDialog("EditMemberPage");
        }

        private void OnHandleReloadMessage(ReloadMemberMessage msg)
        {
            if (msg.ReloadAllMembers)
            {
                Mitglieder = _handler.GetAllMitglieder();
            }
        }
    }
}
