using ListenManager.Enums;
using ListenManager.src.Database.Model;

namespace ListenManager.Database.DataObjects
{


    public class MitgliedsListe : BaseDataObject
    {
        private Verzeichnis _sourceVerzeichnis;
        private string _name;
        public Verzeichnis SourceVerzeichnis
        {
            get => _sourceVerzeichnis;
            set
            {
                if(value.Equals(_sourceVerzeichnis)) return;
                _sourceVerzeichnis = value;
                OnPropertyChanged(nameof(SourceVerzeichnis));
            }
        }
        public string Name
        {
            get => SourceVerzeichnis == null ? _name : SourceVerzeichnis.Name;
            set
            {
                if (value.Equals(SourceVerzeichnis?.Name)) return;
                if (SourceVerzeichnis == null)
                {
                    _name = value;
                }
                else
                {
                    SourceVerzeichnis.Name = value;
                }
                
                OnPropertyChanged(nameof(Name));
            }
        }

        public ListType Type { get; set; }
    }
}
