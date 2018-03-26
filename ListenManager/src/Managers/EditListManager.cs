using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ListenManager.Database.DataObjects;
using ListenManager.Database.Handlers;
using ListenManager.Interfaces;
using ListenManager.Managers.Messages;
using ListenManager.Managers.Util;
using ListenManager.src.Database.Model;

namespace ListenManager.Managers
{
    public class EditListManager : BaseManager
    {
        private Visibilities _visibilities;
        private MitgliedsListe _liste;
        private ObservableCollection<VereinsMitglied> _mitgliederInList;
        private ObservableCollection<VereinsMitglied> _availableMitglieder;

        private readonly VerzeichnisHandler _handler;
        private readonly bool _isEditDialog;

        private readonly Verzeichnis _originalVerzeichnis;
        private readonly Fieldvisibility _originalFieldvisibility;

        private ICommand _saveCommand;
        private ICommand _closeCommand;

        public EditListManager()
        {
            var shared = SharedProperties.Instance;
            _isEditDialog = shared.IsEditListDialog;
            _handler = VerzeichnisHandler.Instance;

            if (_isEditDialog)
            {
                Liste = shared.ListeToEdit;
                _originalVerzeichnis = Liste.SourceVerzeichnis;
                Liste.SourceVerzeichnis = new Verzeichnis()
                {
                    Name = _originalVerzeichnis.Name
                };

                VisibleFields = _handler.GetFieldVisibilitiesForGivenList(_originalVerzeichnis.ID);
                _originalFieldvisibility = VisibleFields.SourceFieldvisibility;
                VisibleFields.SourceFieldvisibility = new Fieldvisibility()
                {
                    Anrede = _originalFieldvisibility.Anrede,
                    BIC = _originalFieldvisibility.BIC,
                    Bundesland = _originalFieldvisibility.Bundesland,
                    Eintrittsdatum = _originalFieldvisibility.Eintrittsdatum,
                    eMail = _originalFieldvisibility.eMail,
                    Geburtsdatum = _originalFieldvisibility.Geburtsdatum,
                    IBAN = _originalFieldvisibility.IBAN,
                    Kreditinstitut = _originalFieldvisibility.Kreditinstitut,
                    Mitgliedsnr = _originalFieldvisibility.Mitgliedsnr,
                    Mobil = _originalFieldvisibility.Mobil,
                    Name = _originalFieldvisibility.Name,
                    Ort = _originalFieldvisibility.Ort,
                    Plz = _originalFieldvisibility.Plz,
                    Straße = _originalFieldvisibility.Straße,
                    Telefon = _originalFieldvisibility.Telefon,
                    Vorname = _originalFieldvisibility.Vorname
                };

                MitgliederInList = _handler.GetMitgliederInVerzeichnis(_originalVerzeichnis.ID);
                AvailableMitglieder = _handler.GetMitgliederAvailableForList(_originalVerzeichnis.ID);
            }
            else
            {
                VisibleFields = _handler.DefaultVisibilities;
                Liste = _handler.CreateNewListe();
                MitgliederInList = new ObservableCollection<VereinsMitglied>();
                AvailableMitglieder = _handler.GetAllMitglieder();
            }
            
        }

        public Visibilities VisibleFields
        {
            get => _visibilities;
            set
            {
                _visibilities = value;
                OnPropertyChanged(nameof(VisibleFields));
            }
        }

        public MitgliedsListe Liste
        {
            get => _liste;
            set
            {
                _liste = value;
                OnPropertyChanged(nameof(Liste));
            }
        }

        public ObservableCollection<VereinsMitglied> MitgliederInList
        {
            get => _mitgliederInList;
            set
            {
                _mitgliederInList = value;
                OnPropertyChanged(nameof(MitgliederInList));
            }
        }

        public ObservableCollection<VereinsMitglied> AvailableMitglieder
        {
            get => _availableMitglieder;
            set
            {
                _availableMitglieder = value;
                OnPropertyChanged(nameof(AvailableMitglieder));
            }
        }

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand<IClosable>(SaveChanges));
        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand<IClosable>(CloseDialog));


        private void SaveChanges(IClosable p)
        {
            if (_isEditDialog)
            {
                UpdateList();
                Messenger.Default.Send(new UpdateListMessage() { IdOfItemToUpdate = Liste.SourceVerzeichnis.ID, ReloadAllData = false });
            }
            else
            {
                AddNewList();
                Messenger.Default.Send(new UpdateListMessage() { IdOfItemToUpdate = -1, ReloadAllData = true });
            }
            p?.Close();
        }

        private void CloseDialog(IClosable p)
        {
            if (_isEditDialog)
            {
                CancelEdit();
            }
            p?.Close();
        }

        private void CancelEdit()
        {
            Liste.SourceVerzeichnis = _originalVerzeichnis;
        }

        private void AddNewList()
        {
            _handler.AddVerzeichnis(Liste.SourceVerzeichnis, VisibleFields.SourceFieldvisibility, MitgliederInList);
        }

        private void UpdateList()
        {
            var vz = Liste.SourceVerzeichnis;

            _originalVerzeichnis.Name = vz.Name;

            foreach (var mil in MitgliederInList)
            {
                if (!_originalVerzeichnis.Mitglied.Contains(mil.SourceMitglied))
                {
                    _originalVerzeichnis.Mitglied.Add(mil.SourceMitglied);
                }
            }

            foreach (var mil in AvailableMitglieder)
            {
                if (_originalVerzeichnis.Mitglied.Contains(mil.SourceMitglied))
                {
                    _originalVerzeichnis.Mitglied.Remove(mil.SourceMitglied);
                }
            }

            var fv = VisibleFields.SourceFieldvisibility;

            _originalFieldvisibility.Mitgliedsnr = fv.Mitgliedsnr;
            _originalFieldvisibility.Anrede = fv.Anrede;
            _originalFieldvisibility.Vorname = fv.Vorname;
            _originalFieldvisibility.Name = fv.Name;
            _originalFieldvisibility.Eintrittsdatum = fv.Eintrittsdatum;
            _originalFieldvisibility.Geburtsdatum = fv.Geburtsdatum;
            _originalFieldvisibility.Straße = fv.Straße;
            _originalFieldvisibility.Ort = fv.Ort;
            _originalFieldvisibility.Bundesland = fv.Bundesland;
            _originalFieldvisibility.eMail = fv.eMail;
            _originalFieldvisibility.Telefon = fv.Telefon;
            _originalFieldvisibility.Mobil = fv.Mobil;
            _originalFieldvisibility.Kreditinstitut = fv.Kreditinstitut;
            _originalFieldvisibility.IBAN = fv.IBAN;
            _originalFieldvisibility.BIC = fv.BIC;

            Liste.SourceVerzeichnis = _originalVerzeichnis;

            _handler.UpdateData();
        }
    }
}
