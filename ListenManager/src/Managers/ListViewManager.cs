using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ListenManager.Database.DataObjects;
using ListenManager.Database.Handlers;
using ListenManager.Enums;
using ListenManager.Managers.Messages;
using ListenManager.Managers.Util;
using MahApps.Metro.Controls.Dialogs;

namespace ListenManager.Managers
{
    public class ListViewManager : BaseManager
    {
        private readonly IDialogCoordinator _coordinator;
        private readonly VerzeichnisHandler _handler;
        private readonly Visibilities _defaultVisibilities;

        private Visibilities _fieldVisibility;
        private MitgliedsListe _selectedListe;
        private ObservableCollection<MitgliedsListe> _listen;

        private ICommand _editListCommand;
        private ICommand _addListCommand;
        private ICommand _deleteListCommand;

        public ListViewManager()
        {
            _coordinator = DialogCoordinator.Instance;
            _handler = VerzeichnisHandler.Instance;

            _defaultVisibilities = _handler.DefaultVisibilities;

            Listen = _handler.GetAllVerzeichnisse();
            SelectedListe = Listen.FirstOrDefault();

            if (SelectedListe == null) { return; }
            VisibleFields = _handler.GetFieldVisibilitiesForGivenList(SelectedListe.SourceVerzeichnis.ID);

            Messenger.Default.Register<UpdateListMessage>(this, HandleUpdateMessage);
        }

        public Visibilities VisibleFields
        {
            get => _fieldVisibility;
            set
            {
                if(value == null) return;
                if(value.Equals(_fieldVisibility)) return;
                _fieldVisibility = value;
                OnPropertyChanged(nameof(VisibleFields));
            }
        }

        public ObservableCollection<MitgliedsListe> Listen
        {
            get => _listen;
            set
            {
                if(value == null) return;
                if(value.Equals(_listen)) return;
                _listen = value;
                OnPropertyChanged(nameof(Listen));
            }
        }

        public MitgliedsListe SelectedListe
        {
            get => _selectedListe;
            set
            {
                if(value == null) return;
                if(value.Equals(_selectedListe)) return;
                _selectedListe = value;
                UpdateListe();
                OnPropertyChanged(nameof(SelectedListe));
            }
        }

        public ICommand EditListCommand => _editListCommand ?? (_editListCommand = new RelayCommand(ShowEditListDialog));
        public ICommand AddListCommand => _addListCommand ?? (_addListCommand = new RelayCommand(ShowAddListDialog));
        public ICommand DeleteListCommand => _deleteListCommand ?? (_deleteListCommand = new RelayCommand(DeleteSelectedList));

        private void UpdateListe ()
        {
            if (SelectedListe.Type != ListType.UserCreated)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (SelectedListe.Type)
                {
                    case ListType.Jugend:
                        Mitglieder = _handler.GetJugend();
                        break;
                    case ListType.Erwachsene:
                        Mitglieder = _handler.GetErwachsene();
                        break;
                }

                VisibleFields = _defaultVisibilities;
            }
            else
            {
                Mitglieder = _handler.GetMitgliederInVerzeichnis(SelectedListe.SourceVerzeichnis.ID);
                VisibleFields = _handler.GetFieldVisibilitiesForGivenList(SelectedListe.SourceVerzeichnis.ID);
            }
            
        }

        private void ShowEditListDialog()
        {
            if (SelectedListe.Type != ListType.UserCreated) return;

            var shared = SharedProperties.Instance;
            shared.IsEditListDialog = true;
            shared.ListeToEdit = SelectedListe;
            WindowService.ShowDialog("EditListPage");
        }

        private static void ShowAddListDialog()
        {
            var shared = SharedProperties.Instance;
            shared.IsEditListDialog = false;
            shared.ListeToEdit = null;
            WindowService.ShowDialog("EditListPage");
        }

        private void DeleteSelectedList()
        {
            if (SelectedListe.Type != ListType.UserCreated)
            {
                _coordinator.ShowMessageAsync(this, "Warnung", "Die gewählte Liste kann nicht gelöscht werden weil sie " +
                                                               "vom der Anwendung automatisch angelegt wurde. Nur selbst erstellte " +
                                                               "Listen können gelöscht werden.");
                return;
            }

            _handler.RemoveVerzeichnis(SelectedListe.SourceVerzeichnis);
        }

        private void HandleUpdateMessage(UpdateListMessage msg)
        {
            if (msg.ReloadAllData)
            {
                Listen = _handler.GetAllVerzeichnisse();
            }
            else
            {
                Mitglieder = _handler.GetMitgliederInVerzeichnis(msg.IdOfItemToUpdate);
                VisibleFields = _handler.GetFieldVisibilitiesForGivenList(msg.IdOfItemToUpdate);
            }
        }
    }
}