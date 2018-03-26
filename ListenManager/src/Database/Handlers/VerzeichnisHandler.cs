using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ListenManager.Database.DataObjects;
using ListenManager.Enums;
using ListenManager.src.Database.Model;

namespace ListenManager.Database.Handlers
{
    public class VerzeichnisHandler
    {
        private static VerzeichnisHandler _instance;
        private static Visibilities _defaultVisibilities;
        private readonly Entities _context;
        private readonly DateTime _compareMinDate;
        private readonly DateTime _compareMaxDate;
        public static VerzeichnisHandler Instance => _instance ?? (_instance = new VerzeichnisHandler());
        public Visibilities DefaultVisibilities => _defaultVisibilities ?? (_defaultVisibilities = new Visibilities()
        {
            SourceFieldvisibility = new Fieldvisibility()
            {
                Anrede = true,
                BIC = true,
                Bundesland = true,
                Eintrittsdatum = true,
                eMail = true,
                Geburtsdatum = true,
                IBAN = true,
                Kreditinstitut = true,
                Mitgliedsnr = true,
                Mobil = true,
                Name = true,
                Ort = true,
                Plz = true,
                Straße = true,
                Telefon = true,
                Vorname = true
            }
        });

        private VerzeichnisHandler()
        {
            _context = new Entities();
            _compareMinDate = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0));
            _compareMaxDate = DateTime.Now.AddDays(38);
        }

        public void AddMitglied(Mitglied mitglied)
        {
            _context.Mitglieder.Add(mitglied);
            _context.SaveChanges();
        }

        public void AddVerzeichnis(Verzeichnis verzeichnis, Fieldvisibility visibilities, ICollection<VereinsMitglied> mitglieder)
        {
            var insert = mitglieder.Select(row => row.SourceMitglied).ToList();

            verzeichnis.Fieldvisibilities = new List<Fieldvisibility>() { visibilities };
            verzeichnis.Mitglieder = insert;

            _context.Verzeichnisse.Add(verzeichnis);
            _context.Fieldvisibilities.Add(visibilities);
            _context.SaveChanges();
        }

        public ObservableCollection<VereinsMitglied> GetAllMitglieder()
        {
            var tmp = (from m in _context.Mitglieder
                orderby m.Mitgliedsnr
                select m).ToList();

            return CreateObservableCollection(tmp);
        }

        public ObservableCollection<VereinsMitglied> GetMitgliederInVerzeichnis(long id)
        {
            var tmp = (from m in _context.Mitglieder
                where m.Verzeichnisse.Any(p => p.ID == id)
                select m).ToList();

            return CreateObservableCollection(tmp);
        }

        public ObservableCollection<VereinsMitglied> GetMitgliederAvailableForList(long id)
        {
            var tmp = (from m in _context.Mitglieder
                       where m.Verzeichnisse.Any(p => p.ID != id) || !m.Verzeichnisse.Any()
                       select m).ToList();

            return CreateObservableCollection(tmp);
        }

        public ObservableCollection<VereinsMitglied> GetMitgliederWithBirthdayForCurrentMonth()
        {
            var tmp = (from bm in _context.Mitglieder
                select bm).AsEnumerable();

            tmp = (from bm in tmp
                where IsInTimeConstrains(bm)
                select bm).ToList();

            return CreateObservableCollection(tmp);
        }

        public ObservableCollection<VereinsMitglied> GetJugend()
        {
            var tmp = (from j in _context.Mitglieder
                where j.Geburtsdatum >= DateTime.Today.AddYears(-18)
                          select j).ToList();

            return CreateObservableCollection(tmp);
        }

        public ObservableCollection<VereinsMitglied> GetErwachsene()
        {
            var tmp = (from j in _context.Mitglieder
                where j.Geburtsdatum < DateTime.Today.AddYears(-18)
                select j).ToList();

            return CreateObservableCollection(tmp);
        }

        public void UpdateData()
        {
            _context.SaveChanges();
        }

        public ObservableCollection<DisplayOrt> GetAllOrte()
        {
            var data = (from o in _context.Orte
                        where o.Bundesland.Equals("Hessen")
                        orderby o.PLZ
                        select o).ToList();

            return CreateObservableCollection(data);
        }

        public ObservableCollection<DisplayOrt> GetOrtForPlz(long? selectedOrtPlz)
        {
            var data = (from o in _context.Orte
                where o.PLZ == selectedOrtPlz
                select o).ToList();

            return CreateObservableCollection(data);
        }

        public ObservableCollection<MitgliedsListe> GetAllVerzeichnisse()
        {
            var verz = (from v in _context.Verzeichnisse
                orderby v.Name
                select v).ToList();

            return CreateObservableCollection(verz);
        }

        public MitgliedsListe GetVerzeichnis(long id)
        {
            var ml = (from v in _context.Verzeichnisse
                where v.ID == id
                select v).FirstOrDefault();
            return new MitgliedsListe() { SourceVerzeichnis = ml, Type = ListType.UserCreated };
        }

        public MitgliedsListe CreateNewListe()
        {
            return new MitgliedsListe() { SourceVerzeichnis = new Verzeichnis(), Type = ListType.UserCreated };
        }

        public Visibilities GetFieldVisibilitiesForGivenList(long id)
        {
            var v = (from fv in _context.Fieldvisibilities
                where fv.ID_Verzeichnis == id
                select fv).FirstOrDefault();
            return new Visibilities() { SourceFieldvisibility =  v };
        }

        public long GetHighestMitgliedsnr()
        {
            var max = _context.Mitglieder.Max(p => p.Mitgliedsnr);
            return max ?? -1;
        }

        private bool IsInTimeConstrains(Mitglied bm)
        {
            if (bm.Geburtsdatum == null) return false;
            var birthday = new DateTime(DateTime.Now.Year, bm.Geburtsdatum.Value.Month, bm.Geburtsdatum.Value.Day);
            return birthday >= _compareMinDate && birthday <= _compareMaxDate;
        }

        private ObservableCollection<VereinsMitglied> CreateObservableCollection(IEnumerable<Mitglied> mitglieder)
        {
            var rueck = new ObservableCollection<VereinsMitglied>();

            foreach (var m in mitglieder)
            {
                var ort = GetOrtForMitglied(m.ID_Ort ?? 567); // 567 ist 61350 Bad Homburg
                var vm = new VereinsMitglied
                {
                    SourceMitglied = m,
                    SourceOrt = ort
                };
                rueck.Add(vm);
            }

            return rueck;
        }

        private static ObservableCollection<DisplayOrt> CreateObservableCollection(IEnumerable<Ort> orte)
        {
            var rueck = new ObservableCollection<DisplayOrt>();

            foreach (var ort in orte)
            {
                rueck.Add(new DisplayOrt() { SourceOrt = ort });
            }

            return rueck;
        }

        private static ObservableCollection<MitgliedsListe> CreateObservableCollection(IEnumerable<Verzeichnis> verz)
        {
            var rueck = new ObservableCollection<MitgliedsListe>();

            foreach (var row in verz)
            {
                var ml = new MitgliedsListe() { SourceVerzeichnis = row, Type = ListType.UserCreated };
                rueck.Add(ml);
            }

            return rueck;
        }

        private Ort GetOrtForMitglied(long idOrt)
        {
            return (from o in _context.Orte
                where o.ID == idOrt
                select o).ToList().FirstOrDefault();
        }

        public SortedDictionary<ConfigType, string> GetConfig()
        {
            var data = (from c in _context.Config
                        select c).ToList();

            var rueck = new SortedDictionary<ConfigType, string>();

            foreach (var row in data)
            {
                Enum.TryParse(row.Key, out ConfigType con);
                rueck.Add(con, row.VALUE);
            }

            return rueck;
        }
    }
}