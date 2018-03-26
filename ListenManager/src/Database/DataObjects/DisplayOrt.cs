using ListenManager.src.Database.Model;

namespace ListenManager.Database.DataObjects
{
    public class DisplayOrt : BaseDataObject
    {
        public Ort SourceOrt { get; set; }
        public long Plz => SourceOrt.PLZ ?? 0;
        public string Ort => SourceOrt.Name ?? "";
        public string Bundesland => SourceOrt.Bundesland ?? "";
    }
}
