using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ListenManager.Database.DataObjects;
using ListenManager.Database.Handlers;
using ListenManager.Managers.Util;
using ListenManager.src.Database.Model;
using ListenManager.Interfaces;
using ListenManager.src.Managers.Messages;

namespace ListenManager.Managers
{
    public class EditMemberManager : BaseManager
    {
        private readonly bool _isEditDialog;
        private readonly VerzeichnisHandler _handler;
        private readonly Mitglied _originalMitglied;

        private VereinsMitglied _member;
        private DisplayOrt _selectedOrt;
        private DisplayOrt _selectedFilteredOrt;
        private ObservableCollection<DisplayOrt> _alleOrte;
        private ObservableCollection<DisplayOrt> _orteMitSelberPlz;
        private ObservableCollection<string> _anreden;

        private ICommand _saveCommand;
        private ICommand _closeCommand;

        public EditMemberManager()
        {
            Anreden = new ObservableCollection<string>() {"Dr.", "Frau", "Herr", "Prof.", "Prof. Dr."};
            _handler = VerzeichnisHandler.Instance;

            var shared = SharedProperties.Instance;
            _isEditDialog = shared.IsEditMemberDialog;
            Orte = _handler.GetAllOrte();

            if (_isEditDialog)
            {
                Member = shared.MemberToEdit;
                SelectedOrt = (from o in Orte where o.Plz == Member.Postleitzahl select o).First();
                _originalMitglied = Member.SourceMitglied;
                Member.SourceMitglied = new Mitglied()
                {
                    Mitgliedsnr = _originalMitglied.Mitgliedsnr,
                    Anrede = _originalMitglied.Anrede,
                    Vorname = _originalMitglied.Vorname,
                    Name = _originalMitglied.Name,
                    Straße = _originalMitglied.Straße,
                    ID_Ort = _originalMitglied.ID_Ort,
                    Telefon = _originalMitglied.Telefon,
                    Mobil = _originalMitglied.Mobil,
                    eMail = _originalMitglied.eMail,
                    Eintrittsdatum = _originalMitglied.Eintrittsdatum,
                    Geburtsdatum = _originalMitglied.Geburtsdatum,
                    IBAN = _originalMitglied.IBAN,
                    BIC = _originalMitglied.BIC,
                    Kreditinstitut = _originalMitglied.Kreditinstitut
                };
            }
            else
            {
                SelectedOrt = (from o in Orte where o.Plz == 61350 select o).First();
                var newMnr = _handler.GetHighestMitgliedsnr() + 1;
                Member = new VereinsMitglied
                {
                    SourceMitglied = new Mitglied() { Mitgliedsnr = newMnr, Eintrittsdatum = DateTime.Today, Geburtsdatum = DateTime.Today},
                    SourceOrt = new Ort()
                };
            }
        }

        public VereinsMitglied Member
        {
            get => _member;
            set
            {
                _member = value;
                OnPropertyChanged(nameof(Member));
            }
        }

        public DisplayOrt SelectedOrt
        {
            get => _selectedOrt;
            set
            {
                if (value == null) return;
                if (value.Equals(SelectedOrt)) return;
                _selectedOrt = value;
                FilteredOrte = _handler.GetOrtForPlz(_selectedOrt.Plz);
                SelectedFilteredOrt = FilteredOrte.First();
                OnPropertyChanged(nameof(SelectedOrt));
            }
        }
        public DisplayOrt SelectedFilteredOrt
        {
            get => _selectedFilteredOrt;
            set
            {
                if (value == null) return;
                if (value.Equals(SelectedFilteredOrt)) return;
                _selectedFilteredOrt = value;
                OnPropertyChanged(nameof(SelectedOrt));
            }
        }

        public ObservableCollection<DisplayOrt> Orte
        {
            get => _alleOrte;
            set
            {
                _alleOrte = value;
                OnPropertyChanged(nameof(Orte));
            }
        }
        public ObservableCollection<DisplayOrt> FilteredOrte
        {
            get => _orteMitSelberPlz;
            set
            {
                _orteMitSelberPlz = value;
                OnPropertyChanged(nameof(FilteredOrte));
            }
        }

        public ObservableCollection<string> Anreden
        {
            get => _anreden;
            set
            {
                _anreden = value;
                OnPropertyChanged(nameof(Anreden));
            }
        }

        public ICommand SaveCommand => _saveCommand ??
                       (_saveCommand = new RelayCommand<IClosable>(SaveAction));

        public ICommand CloseCommand => _closeCommand ??
                       (_closeCommand = new RelayCommand<IClosable>(ClosePage));

        private void SaveAction(IClosable p)
        {
            if (_isEditDialog)
            {
                UpdateMember();
                Messenger.Default.Send(new ReloadMemberMessage { ReloadAllMembers = true, ReloadBirthdays = false });
            }
            else
            {
                AddNewMember();
                Messenger.Default.Send(new ReloadMemberMessage { ReloadAllMembers = true, ReloadBirthdays = true });
            }
            p?.Close();
        }

        private void AddNewMember()
        {
            Member.SourceMitglied.ID_Ort = SelectedFilteredOrt.SourceOrt.ID;
            _handler.AddMitglied(Member.SourceMitglied);
            Member.SourceOrt = SelectedFilteredOrt.SourceOrt;
            var shared = SharedProperties.Instance;
            shared.MitgliederCollection.Add(Member);
        }

        private void UpdateMember()
        {
            _originalMitglied.Mitgliedsnr = Member.Mitgliedsnr;
            _originalMitglied.Anrede = Member.Anrede;
            _originalMitglied.Vorname = Member.Vorname;
            _originalMitglied.Name = Member.Name;
            _originalMitglied.Straße = Member.Straße;
            _originalMitglied.ID_Ort = SelectedFilteredOrt.SourceOrt.ID;
            _originalMitglied.Telefon = Member.Telefon;
            _originalMitglied.Mobil = Member.Mobil;
            _originalMitglied.eMail = Member.Email;
            _originalMitglied.Eintrittsdatum = Member.Eintrittsdatum;
            _originalMitglied.Geburtsdatum = Member.Geburtsdatum;
            _originalMitglied.IBAN = Member.IBAN;
            _originalMitglied.BIC = Member.BIC;
            _originalMitglied.Kreditinstitut = Member.Kreditinstitut;

            _handler.UpdateData();
            Member.SourceMitglied = _originalMitglied;
            Member.SourceOrt = SelectedFilteredOrt.SourceOrt;
        }

        private void ClosePage(IClosable p)
        {
            if (_isEditDialog)
            {
                Member.SourceMitglied = _originalMitglied;
            }
            p?.Close();
        }
    }
}
