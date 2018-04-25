using System;
using ListenManager.src.Database.Model;

namespace ListenManager.Database.DataObjects
{
    public class VereinsMitglied : BaseDataObject
    {
        private Mitglied _srcMitglied;
        private Ort _srcOrt;

        public Mitglied SourceMitglied
        {
            get => _srcMitglied;
            set
            {
                _srcMitglied = value;
                OnPropertyChanged(nameof(Mitgliedsnr));
                OnPropertyChanged(nameof(Vorname));
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Straße));
                OnPropertyChanged(nameof(Telefon));
                OnPropertyChanged(nameof(Mobil));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(Eintrittsdatum));
                OnPropertyChanged(nameof(Geburtsdatum));
                OnPropertyChanged(nameof(Iban));
                OnPropertyChanged(nameof(Bic));
                OnPropertyChanged(nameof(Kreditinstitut));
            }
        }

        public Ort SourceOrt
        {
            get => _srcOrt;
            set
            {
                _srcOrt = value;
                OnPropertyChanged(nameof(Postleitzahl));
                OnPropertyChanged(nameof(Ort));
                OnPropertyChanged(nameof(Bundesland));
            }
        }

        public long Mitgliedsnr {
            get => SourceMitglied.Mitgliedsnr ?? 0;
            set
            {
                if (value.Equals(SourceMitglied.Mitgliedsnr)) return;
                SourceMitglied.Mitgliedsnr = value;
                OnPropertyChanged(nameof(Mitgliedsnr));
            }
        }

        public string Anrede
        {
            get => SourceMitglied.Anrede ?? "";
            set
            {
                if(value != null && value.Equals(Anrede)) return;
                SourceMitglied.Anrede = value;
                OnPropertyChanged(nameof(Anrede));
            }
        }

        public string Vorname
        {
            get => SourceMitglied.Vorname ?? "";
            set
            {
                if (value.Equals(SourceMitglied.Vorname)) return;
                SourceMitglied.Vorname = value;
                OnPropertyChanged(nameof(Vorname));
            }
        }

        public string Name
        {
            get => SourceMitglied.Name ?? "";
            set
            {
                if (value.Equals(SourceMitglied.Name)) return;
                SourceMitglied.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Straße
        {
            get => SourceMitglied.Straße ?? "";
            set
            {
                if (value.Equals(SourceMitglied.Straße)) return;
                SourceMitglied.Straße = value;
                OnPropertyChanged(nameof(Straße));
            }
        }
        public long Postleitzahl => SourceOrt.PLZ ?? 0;
        public string Ort => SourceOrt.Name ?? "";
        public string Bundesland => SourceOrt.Bundesland ?? "";
        public string Telefon
        {
            get => SourceMitglied.Telefon ?? "";
            set
            {
                if (value.Equals(SourceMitglied.Telefon)) return;
                SourceMitglied.Telefon = value;
                OnPropertyChanged(nameof(Telefon));
            }
        }
        public string Mobil
        {
            get => SourceMitglied.Mobil ?? "";
            set
            {
                if (value.Equals(SourceMitglied.Mobil)) return;
                SourceMitglied.Mobil = value;
                OnPropertyChanged(nameof(Mobil));
            }
        }
        public string Email
        {
            get => SourceMitglied.eMail ?? "";
            set
            {
                if (value.Equals(SourceMitglied.eMail)) return;
                SourceMitglied.eMail = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public DateTime Eintrittsdatum
        {
            get => SourceMitglied.Eintrittsdatum ?? DateTime.MinValue;
            set
            {
                if (value.Equals(SourceMitglied.Eintrittsdatum)) return;
                SourceMitglied.Eintrittsdatum = value;
                OnPropertyChanged(nameof(Eintrittsdatum));
            }
        }
        public DateTime Geburtsdatum
        {
            get => SourceMitglied.Geburtsdatum ?? DateTime.MinValue;
            set
            {
                if (value.Equals(SourceMitglied.Geburtsdatum)) return;
                SourceMitglied.Geburtsdatum = value;
                OnPropertyChanged(nameof(Geburtsdatum));
            }
        }
        public string Iban
        {
            get => SourceMitglied.IBAN ?? "";
            set
            {
                if (value.Equals(SourceMitglied.IBAN)) return;
                SourceMitglied.IBAN = value;
                OnPropertyChanged(nameof(Iban));
            }
        }
        public string Bic
        {
            get => SourceMitglied.BIC ?? "";
            set
            {
                if (value.Equals(SourceMitglied.BIC)) return;
                SourceMitglied.BIC = value;
                OnPropertyChanged(nameof(Bic));
            }
        }
        public string Kreditinstitut
        {
            get => SourceMitglied.Kreditinstitut ?? "";
            set
            {
                if (value.Equals(SourceMitglied.Kreditinstitut)) return;
                SourceMitglied.Kreditinstitut = value;
                OnPropertyChanged(nameof(Kreditinstitut));
            }
        }
    }
}
