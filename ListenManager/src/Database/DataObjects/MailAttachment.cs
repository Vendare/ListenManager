using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ListenManager.Database.DataObjects
{
    public class MailAttachment : BaseDataObject
    {
        private FileInfo _attchmentFileInfo;

        public ImageSource AttachmentIcon { get; private set; }


        public string Name => _attchmentFileInfo.Name;

        public FileInfo AttachmentFileInfo
        {
            get => _attchmentFileInfo;
            set
            {
                _attchmentFileInfo = value;

                var icon = Icon.ExtractAssociatedIcon(_attchmentFileInfo.FullName);

                if (icon == null) return;

                using (var bmp = icon.ToBitmap())
                {
                    var stream = new MemoryStream();
                    bmp.Save(stream, ImageFormat.Png);
                    AttachmentIcon = BitmapFrame.Create(stream);
                }

                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(AttachmentIcon));
            }
        }
    }
}
