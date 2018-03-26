using System.Collections.ObjectModel;
using ListenManager.Database.DataObjects;

namespace ListenManager.Managers.Util
{
    public class SharedProperties
    {
        private static SharedProperties _instance;
        public static SharedProperties Instance => _instance ?? (_instance = new SharedProperties());
        public bool IsEditMemberDialog { get; set; }
        public VereinsMitglied MemberToEdit { get; set; }
        public ObservableCollection<VereinsMitglied> MitgliederCollection { get; set; }
        public bool IsEditListDialog { get; set; }
        public MitgliedsListe ListeToEdit { get; set; }
    }
}
