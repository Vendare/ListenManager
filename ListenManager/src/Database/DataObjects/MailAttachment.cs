using System.Drawing;
using System.IO;

namespace ListenManager.Database.DataObjects
{
    public class MailAttachment : BaseDataObject
    {
        private FileInfo _attchmentFileInfo;

        public Icon AttachmentIcon => Icon.ExtractAssociatedIcon(_attchmentFileInfo.FullName);

        public string Name => _attchmentFileInfo.Name;

        public FileInfo AttachmentFileInfo
        {
            get => _attchmentFileInfo;
            set
            {
                _attchmentFileInfo = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(AttachmentIcon));
            }
        }
    }
}
