using ListenManager.Managers;
using ListenManager.src.Database.Model;

namespace ListenManager.Database.DataObjects
{
    public class Visibilities : BaseManager
    {
        private Fieldvisibility _fv;

        public Fieldvisibility SourceFieldvisibility
        {
            get => _fv;
            set
            {
                if(value==null || value.Equals(_fv)) return;
                _fv = value;
                OnPropertyChanged(nameof(MitgliednrVisible));
                OnPropertyChanged(nameof(AnredeVisible));
                OnPropertyChanged(nameof(VornameVisible));
                OnPropertyChanged(nameof(NameVisible));
                OnPropertyChanged(nameof(StraßeVisible));
                OnPropertyChanged(nameof(PostleitzahlVisible));
                OnPropertyChanged(nameof(OrtVisible));
                OnPropertyChanged(nameof(BundeslandVisible));
                OnPropertyChanged(nameof(EintrittsdatumVisible));
                OnPropertyChanged(nameof(GeburtsdatumVisible));
                OnPropertyChanged(nameof(EmailVisible));
                OnPropertyChanged(nameof(TelefonVisible));
                OnPropertyChanged(nameof(MobilVisible));
                OnPropertyChanged(nameof(IbanVisible));
                OnPropertyChanged(nameof(BicVisible));
                OnPropertyChanged(nameof(KreditinstitutVisible));
            }
        }

        public bool MitgliednrVisible
        {
            get => _fv.Mitgliedsnr ?? true;
            set
            {
                if (value.Equals(_fv.Mitgliedsnr)) return;
                _fv.Mitgliedsnr = value;
                OnPropertyChanged(nameof(MitgliednrVisible));
            }
        }

        public bool AnredeVisible
        {
            get => _fv.Anrede ?? true;
            set
            {
                _fv.Anrede = value;
                OnPropertyChanged(nameof(AnredeVisible));
            }
        }

        public bool VornameVisible
        {
            get => _fv.Vorname ?? true;
            set
            {
                if(value.Equals(_fv.Vorname)) return;
                _fv.Vorname = value;
                OnPropertyChanged(nameof(VornameVisible));
            }
        }

        public bool NameVisible
        {
            get => _fv.Name ?? true;
            set
            {
                if(value.Equals(_fv.Name)) return;
                _fv.Name = value;
                OnPropertyChanged(nameof(NameVisible));
            }
        }

        public bool StraßeVisible
        {
            get => _fv.Straße ?? true;
            set
            {
                if(value.Equals(_fv.Straße)) return;
                _fv.Straße = value;
                OnPropertyChanged(nameof(StraßeVisible));
            }
        }

        public bool PostleitzahlVisible
        {
            get => _fv.Plz ?? true;
            set
            {
                if (value.Equals(_fv.Plz)) return;
                _fv.Plz = value;
                OnPropertyChanged(nameof(PostleitzahlVisible));
            }
        }

        public bool OrtVisible
        {
            get => _fv.Ort ?? true;
            set
            {
                if(value.Equals(_fv.Ort)) return;
                _fv.Ort = value;
                OnPropertyChanged(nameof(OrtVisible));
            }
        }

        public bool BundeslandVisible
        {
            get => _fv.Bundesland ?? true;
            set
            {
                if (value.Equals(_fv.Bundesland)) return;
                _fv.Bundesland = value;
                OnPropertyChanged(nameof(BundeslandVisible));
            }
        }

        public bool EintrittsdatumVisible
        {
            get => _fv.Eintrittsdatum ?? true;
            set
            {
                if(value.Equals(_fv.Eintrittsdatum)) return;
                _fv.Eintrittsdatum = value;
                OnPropertyChanged(nameof(EintrittsdatumVisible));
            }
        }

        public bool GeburtsdatumVisible
        {
            get => _fv.Geburtsdatum ?? true;
            set
            {
                if (value.Equals(_fv.Geburtsdatum)) return;
                _fv.Geburtsdatum = value;
                OnPropertyChanged(nameof(GeburtsdatumVisible));
            }
        }

        public bool EmailVisible
        {
            get => _fv.eMail ?? true;
            set
            {
                if(value.Equals(_fv.eMail)) return;
                _fv.eMail = value;
                OnPropertyChanged(nameof(EmailVisible));
            }
        }

        public bool TelefonVisible
        {
            get => _fv.Telefon ?? true;
            set
            {
                if(value.Equals(_fv.Telefon)) return;
                _fv.Telefon = value;
                OnPropertyChanged(nameof(TelefonVisible));
            }
        }

        public bool MobilVisible
        {
            get => _fv.Mobil ?? true;
            set
            {
                if(value.Equals(_fv.Mobil)) return;
                _fv.Mobil = value;
                OnPropertyChanged(nameof(MobilVisible));
            }
        }

        public bool IbanVisible
        {
            get => _fv.IBAN ?? true;
            set
            {
                if(value.Equals(_fv.IBAN)) return;
                _fv.IBAN = value;
                OnPropertyChanged(nameof(IbanVisible));
            }
        }

        public bool BicVisible
        {
            get => _fv.BIC ?? true;
            set
            {
                if(value.Equals(_fv.BIC)) return;
                _fv.BIC = value;
                OnPropertyChanged(nameof(BicVisible));
            }
        }

        public bool KreditinstitutVisible
        {
            get => _fv.Kreditinstitut ?? true;
            set
            {
                if(value.Equals(_fv.Kreditinstitut)) return;
                _fv.Kreditinstitut = value;
                OnPropertyChanged(nameof(KreditinstitutVisible));
            }
        }
    }
}